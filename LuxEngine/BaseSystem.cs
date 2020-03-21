using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LuxEngine
{
    //public class LuxEnum : IEnumerable<KeyValuePair<string, int>>
    //{
    //    private SortedList<string, int> _elements;

    //    public LuxEnum(params string[] keys)
    //    {
    //        _elements = new SortedList<string, int>(keys.Length);
    //        foreach (var key in keys)
    //        {
    //            _elements[key] = 0;
    //        }
    //    }

    //    public IEnumerator<KeyValuePair<string, int>> GetEnumerator()
    //    {
    //        return _elements.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return _elements.GetEnumerator();
    //    }
    //}

    //public static LuxEnum SystemId = new LuxEnum(
    //    "DebugSystem",
    //    "RenderSystem",

    //    "SystemsCount" // Always last
    //);

    // TODO: Enable the user to expand this enum (and update SystemsCount accordingly)
    public enum SystemId
    {
        DebugSystem,
        InputSystem,
        RenderSystem,
        PlatformerPlayerControllerSystem,
        CameraSystem,

        SystemsCount // Always last
    }

    public abstract class IdentifiableSystem<T> : BaseSystem
    {
        public static SystemId SystemId { get; set; }

        public IdentifiableSystem(params ComponentType[] requiredComponentTypes) : base(requiredComponentTypes)
        {
        }
    }

    public abstract class BaseSystem
    {
        public World World { protected get; set; }
        public List<Entity> RegisteredEntities { get; protected set; }
        public ComponentMask ComponentMask { get; private set; }

        /// <param name="requiredComponentTypes">
        /// Specifies which components are required for the entities this
        /// system operates on.
        /// </param>
        public BaseSystem(params ComponentType[] requiredComponentTypes)
        {
            World = null;
            RegisteredEntities = new List<Entity>();
            ComponentMask = new ComponentMask(
                Array.ConvertAll(requiredComponentTypes, x => (int)x));
        }

        public void RegisterEntity(Entity entity)
        {
            RegisteredEntities.Add(entity);
        }

        public void UnregisterEntity(Entity entity)
        {
            RegisteredEntities.Remove(entity);
        }

        /// <summary>
        /// Automatically called when the game launches to initialize any non-graphic variables.
        /// </summary>
        public virtual void Init()
        {
            if (null == World)
            {
                throw new LuxException(LuxStatus.BASESYSTEM_INIT_WORLD_IS_NULL, 0); // TODO: Have an ID for each system and put it in extra_info
            }
        }

        /// <summary>
        /// Automatically called when the game launches to load any game assets (graphics, audio etc.)
        /// </summary>
        public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
        }

        /// <summary>
        /// Called each frame to update the game. Games usually runs 60 frames per second.
        /// Each frame the Update function will run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// This is called when the game is ready to draw to the screen, it's also called each frame.
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}
