using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    public class Camera : AComponent<Camera>
    {
        public Vector2 Zoom;
        public float Rotation;
        public Rectangle? Limits;

        public Matrix Matrix;

        public Camera(float zoom)
        {
            Zoom = new Vector2(zoom, zoom);
            Rotation = 0.0f;
            Limits = null;
            Matrix = Matrix.Identity;
        }
    }

    public class CameraSystem : ASystem<CameraSystem>
    {
        public override void SetSignature(SystemSignature signature)
        {
            signature.Require<Parent>();
            signature.Require<Camera>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            AddComponent(entity, new Transform(0, 0));
        }

        protected override void PreDraw()
        {
            if (RegisteredEntities.Count > 1)
            {
                // Multiple cameras not supported
                LuxCommon.Assert(false);
            }

            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Camera camera);
                Unpack(entity, out Transform transform);
                Unpack(entity, out Parent parent);

                float transformX = transform.X;
                float transformY = transform.Y;
                if (Unpack(parent.ParentEntity, out Transform parentTransform))
                {
                    transformX += parentTransform.X;
                    transformY += parentTransform.Y;
                }

                //if (camera.Limits != null)
                //{
                //    // Update transform according to limits
                //    Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(camera.Matrix)); // camera.Matrix correct?
                //    Vector2 cameraSize = new Vector2(LuxGame.Viewport.Width, LuxGame.Viewport.Height) / camera.Zoom;
                //    Vector2 limitWorldMin = new Vector2(camera.Limits.Value.Left, camera.Limits.Value.Top);
                //    Vector2 limitWorldMax = new Vector2(camera.Limits.Value.Right, camera.Limits.Value.Bottom);
                //    Vector2 positionOffset = new Vector2(transformX, transformY) - cameraWorldMin;
                //    Vector2 position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;

                //    transformX = (int)position.X;
                //    transformY = (int)position.Y;

                //    // Update zoom
                //    float minZoomX = (float)LuxGame.Viewport.Width / camera.Limits.Value.Width;
                //    float minZoomY = (float)LuxGame.Viewport.Height / camera.Limits.Value.Height;
                //    camera.Zoom.X = MathHelper.Max(camera.Zoom.X, minZoomX);
                //    camera.Zoom.Y = MathHelper.Max(camera.Zoom.Y, minZoomY);
                //}

                // Positioning
                Matrix translation = Matrix.CreateTranslation(new Vector3(-CalcUtils.Round(transformX, transformY), 0f));

                // Rotating
                Matrix rotation = Matrix.CreateRotationZ(camera.Rotation);

                // Zooming
                Matrix scale = Matrix.CreateScale(new Vector3(camera.Zoom.X, camera.Zoom.Y, 1f));

                // Centering
                Matrix translation2 = Matrix.CreateTranslation(new Vector3(
                    (int)(LuxGame.Width * 0.5f),
                    (int)(LuxGame.Height * 0.5f),
                    0f));

                camera.Matrix = translation * rotation * scale * translation2;
            }
        }
    }
}
