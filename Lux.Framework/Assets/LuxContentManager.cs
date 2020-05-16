using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Lux.Framework
{
    /// <summary>
    /// Custom content manager for loading, reloading and disposing assets.
    /// </summary>
    public class LuxContentManager : ContentManager
    {

        public LuxContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        { }

        public LuxContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        // TODO: Test what happens without (Game)
        public LuxContentManager() : base(((Game)LuxGame.Instance).Services, ((Game)LuxGame.Instance).Content.RootDirectory)
        { }


    }
}
