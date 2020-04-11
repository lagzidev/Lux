using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class Camera : BaseComponent<Camera>
    {
        public float Zoom;
        public float Rotation;
        public Rectangle VisibleArea;

        public Camera(float zoom)
        {
            Zoom = zoom;
            Rotation = 0.0f;
            VisibleArea = default;
        }
    }

    public class CameraSystem : BaseSystem<CameraSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<Parent>();
            signature.Require<Camera>();
            signature.RequireSingleton<TransformMatrixSingleton>();
            signature.RequireSingleton<VirtualResolutionSingleton>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            World.AddComponent(entity, new Transform(0, 0));
        }

        //protected override void Update(GameTime gameTime)
        //{
        //    foreach (var entity in RegisteredEntities)
        //    {
        //        var camera = World.Unpack<Camera>(entity);
        //        camera.Zoom += 0.01f;
        //    }
        //}

        protected override void PrePreDraw(GameTime gameTime)
        {
            if (RegisteredEntities.Count > 1)
            {
                // Multiple cameras not supported
                LuxCommon.Assert(false);
            }

            var transformMatrixSingleton = World.UnpackSingleton<TransformMatrixSingleton>();
            var virtualResolution = World.UnpackSingleton<VirtualResolutionSingleton>();

            foreach (var entity in RegisteredEntities)
            {
                var camera = World.Unpack<Camera>(entity);
                var transform = World.Unpack<Transform>(entity);

                Entity parentEntity = World.Unpack<Parent>(entity).ParentEntity;
                var parentTransform = World.Unpack<Transform>(parentEntity);

                // Calculate the camera transform matrix

                // Positioning
                Matrix translation = Matrix.CreateTranslation(
                    new Vector3(-new Vector2(transform.X + parentTransform.X, transform.Y + parentTransform.Y), 0f));

                // Rotating
                Matrix rotation = Matrix.CreateRotationZ(camera.Rotation);

                // Zooming
                Matrix scale = Matrix.CreateScale(new Vector3(camera.Zoom, camera.Zoom, 1f));

                // Centering
                Matrix translation2 = Matrix.CreateTranslation(new Vector3(
                    (int)(virtualResolution.VWidth * 0.5f),
                    (int)(virtualResolution.VHeight * 0.5f),
                    0f));

                Matrix transformMatrix = translation * rotation * scale * translation2;

                // Combine the camera's transform matrix with the scale matrix
                transformMatrix *= virtualResolution.ScaleMatrix;

                //Round the X and Y translation so the camera doesn't jerk as it moves:
                //transformMatrix.M22 = (float)Math.Round(transformMatrix.M22, 0);
                //transformMatrix.M11 = (float)Math.Round(transformMatrix.M11, 0);
                //transformMatrix.M41 = (float)Math.Round(transformMatrix.M41, 0);
                //transformMatrix.M42 = (float)Math.Round(transformMatrix.M42, 0);

                // Update the values
                transformMatrixSingleton.TransformMatrix = transformMatrix;
                camera.VisibleArea = CalculateVisibleArea(camera, virtualResolution, transformMatrix);
            }
        }

        /// <summary>
        /// Calculates the screenRect based on where the camera currently is.
        /// </summary>
        private Rectangle CalculateVisibleArea(
            Camera camera,
            VirtualResolutionSingleton virtualResolution,
            Matrix finalScaleMatrix)
        {
            Matrix inverseViewMatrix = Matrix.Invert(finalScaleMatrix);

            Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            Vector2 tr = Vector2.Transform(new Vector2(virtualResolution.VWidth, 0), inverseViewMatrix);
            Vector2 bl = Vector2.Transform(new Vector2(0, virtualResolution.VHeight), inverseViewMatrix);
            Vector2 br = Vector2.Transform(new Vector2(virtualResolution.VWidth, virtualResolution.VHeight), inverseViewMatrix);
            Vector2 min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            Vector2 max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

            //return new Rectangle(
            //    (int)min.X,
            //    (int)min.Y,
            //    (int)(virtualResolution.VWidth / camera.Zoom),
            //    (int)(virtualResolution.VHeight / camera.Zoom));
        }
    }
}
