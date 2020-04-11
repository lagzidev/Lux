using System;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    [Serializable]
    public class ResolutionSettingsSingleton : BaseComponent<ResolutionSettingsSingleton>
    {
        // These are readonly to force recreating this component instead of
        // mutating it. This is to trigger OnRegisterEntity.
        public readonly int WindowScale;
        public readonly bool FullScreen;

        public ResolutionSettingsSingleton(int windowScale, bool fullscreen)
        {
            WindowScale = windowScale;
            FullScreen = fullscreen;
        }
    }

    /// <summary>
    /// Apply the resolution based on the ResolutionSettingsSingleton
    /// </summary>
    public class ResolutionSetterSystem : BaseSystem<ResolutionSetterSystem>
    {
        protected override void SetSignature(SystemSignature signature)
        {
            signature.Require<ResolutionSettingsSingleton>();
            signature.Require<VirtualResolutionSingleton>();
        }

        protected override void OnRegisterEntity(Entity entity)
        {
            var resolutionSettings = World.Unpack<ResolutionSettingsSingleton>(entity);
            var virtualResolution = World.Unpack<VirtualResolutionSingleton>(entity);
            var graphicsDeviceManager = LuxGame.Graphics;

            var preferredWidth = virtualResolution.VWidth * resolutionSettings.WindowScale;
            var preferredHeight = virtualResolution.VHeight * resolutionSettings.WindowScale;

            bool isSupported = false;

            // If we aren't full screen, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (!resolutionSettings.FullScreen)
            {
                isSupported = true;

                // If the preferred size is larger than the screen size
                if ((preferredWidth > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    || (preferredHeight > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    preferredWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    preferredHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate through the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == preferredWidth) && (dm.Height == preferredHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        isSupported = true;
                        break;
                    }
                }
            }

            // Apply changes if can support this resolution
            if (isSupported)
            {
                graphicsDeviceManager.PreferredBackBufferWidth = preferredWidth;
                graphicsDeviceManager.PreferredBackBufferHeight = preferredHeight;
                graphicsDeviceManager.IsFullScreen = resolutionSettings.FullScreen;
                graphicsDeviceManager.PreferMultiSampling = true;
                graphicsDeviceManager.ApplyChanges();
            }
        }
    }
}
