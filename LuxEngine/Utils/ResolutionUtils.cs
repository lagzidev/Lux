//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace LuxEngine
//{
//    public static class ResolutionUtils
//    {
//        public static Matrix GetScaleMatrix(ResolutionSingleton resolution, int viewportWidth)
//        {
//            if (null == resolution)
//            {
//                LuxCommon.Assert(false);
//                return Matrix.Identity;
//            }

//            // If virtual width/height changed, recalculate the matrix
//            if (resolution.DirtyMatrix)
//            {
//                resolution.DirtyMatrix = false;
//                resolution.ScaleMatrix = Matrix.CreateScale(
//                    (float)viewportWidth / resolution.VWidth,
//                    (float)viewportWidth / resolution.VWidth,
//                    1f);
//            }

//            return resolution.ScaleMatrix;
//        }

//        public static Viewport GetVirtualViewport(ResolutionSingleton resolution)
//        {
//            float targetAspectRatio = GetVirtualAspectRatio(resolution);

//            // Figure out the largest area that fits in this resolution at the desired aspect ratio
//            int width = resolution.Width;
//            int height = (int)(width / targetAspectRatio + .5f);

//            if (height > resolution.Height)
//            {
//                height = resolution.Height;
//                // PillarBox
//                width = (int)(height * targetAspectRatio + .5f);
//                resolution.DirtyMatrix = true;
//            }

//            // set up the new viewport centered in the backbuffer
//            Viewport viewport = new Viewport();

//            viewport.X = (resolution.Width / 2) - (width / 2);
//            viewport.Y = (resolution.Height / 2) - (height / 2);
//            //virtualViewportX = viewport.X;
//            //virtualViewportY = viewport.Y; // TODO: Make the virtual viewport available through the ResolutionSingleton
//            viewport.Width = width;
//            viewport.Height = height;
//            viewport.MinDepth = 0;
//            viewport.MaxDepth = 1;

//            return viewport;
//        }

//        private static float GetVirtualAspectRatio(ResolutionSingleton resolution)
//        {
//            return (float)resolution.VWidth / (float)resolution.VHeight;
//        }

//        //private static void SetVirtualResolution(ResolutionSingleton resolution, int virtualWidth, int virtualHeight)
//        //{
//        //    resolution.VWidth = virtualWidth;
//        //    resolution.VHeight = virtualHeight;
//        //    resolution.DirtyMatrix = true;
//        //}
//    }
//}
