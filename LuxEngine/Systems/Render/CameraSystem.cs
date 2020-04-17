using System;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public class Camera : AComponent<Camera>
    {
        public Vector2 Zoom;
        public float Rotation;
        public Rectangle? Limits;

        /// <summary>
        /// A fraction that represents the ease in which the camera follows
        /// its parent.
        /// </summary>
        public float Ease;

        public Matrix Matrix;

        public float Accumulator;

        public Camera(float zoom, float ease=0.05f)
        {
            Zoom = new Vector2(zoom, zoom);
            Rotation = 0.0f;
            Limits = null;
            Matrix = Matrix.Identity;
            Ease = ease;
            Accumulator = 0.0f;
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
            Unpack(entity, out Parent parent);

            float x = 0;
            float y = 0;

            // If parent has a transform, sync the camera with it
            if (Unpack(parent.ParentEntity, out Transform parentTransform))
            {
                x = parentTransform.X;
                y = parentTransform.Y;
            }

            // Set the camera's transform
            if (Unpack(entity, out Transform transform))
            {
                transform.X = x;
                transform.Y = y;
            }
            else
            {
                AddComponent(entity, new Transform(x, y));
            }
        }

        public override void PostUpdate()
        {
            foreach (var entity in RegisteredEntities)
            {
                Unpack(entity, out Camera camera);
                Unpack(entity, out Transform transform);
                Unpack(entity, out Parent parent);

                if (Unpack(parent.ParentEntity, out Transform parentTransform) &&
                    Unpack(parent.ParentEntity, out Moveable parentMoveable))
                {
                    /* TODO: There is a problem at slower speeds that as soon as the parent is
                     * moving at a steady speed and the camera has caught up, the character
                     * keeps moving and the camera follows it, but not immediately
                     * because of the ease. Meaning this is what happens:
                     * - Character moves a pixel as shown on the screen
                     * - The next frame the camera catches up and the character is
                     *   recentered: moved back.
                     * - This happens again and again because the character keeps
                     *   moving and the camera is one frame late when catching up.
                     *   This causes shaking. Solve it.
                     */
                    if (transform.X != parentTransform.X)
                    {
                        transform.X += (parentTransform.X - transform.X) * camera.Ease; ;
                    }

                    if (transform.Y != parentTransform.Y)
                    {
                        transform.Y += (parentTransform.Y - transform.Y) * camera.Ease;
                    }
                }
            }
        }

        public override void PreDraw()
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
                Matrix translation = Matrix.CreateTranslation(new Vector3(-CalcUtils.Round(transform.X, transform.Y), 0f));

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
