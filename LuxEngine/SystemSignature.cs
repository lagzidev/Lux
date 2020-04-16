namespace LuxEngine
{
    /// <summary>
    /// Represents a system's signature - what components and behaviours
    /// it's interested in.
    /// </summary>
    public class SystemSignature
    {
        public readonly ComponentMask ComponentMask;
        public readonly ComponentMask SingletonComponentMask;
        public bool SingletonMatches { get; set; }

        /// <summary>
        /// A reference to the world that owns the system of which signature this is.
        /// The reason for this backwards behaviour is to simplify the signature
        /// setting for the user, so that they could simply signature.Require<T>()
        /// </summary>
        private readonly World _world;

        public SystemSignature(World world)
        {
            ComponentMask = new ComponentMask();
            SingletonComponentMask = new ComponentMask();
            SingletonMatches = true;

            _world = world;
        }

        /// <summary>
        /// Registers the component to the world so that the system could use it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Using<T>() where T : AComponent<T>
        {
            _world.RegisterComponent<T>();
        }

        /// <summary>
        /// Requires any registered entities to have the given component.
        /// This registers the component to the world and adds it to the
        /// system's component mask.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Require<T>() where T : AComponent<T>
        {
            _world.RegisterComponent<T>();
            ComponentMask.AddComponent<T>();
        }

        /// <summary>
        /// Requires the singleton component to be present for the system to
        /// function. IMPORTANT: Whenever the required singleton component doesn't
        /// exist the system won't run.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RequireSingleton<T>() where T : AComponent<T>
        {
            _world.RegisterComponent<T>();

            // At this stage there aren't any components created, so this is false for sure
            SingletonMatches = false;
            SingletonComponentMask.AddComponent<T>();
        }

        /// <summary>
        /// Keeps the previous state of the component which will be available
        /// through World.UnpackPrevious<T>()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void KeepPreviousState<T>() where T : AComponent<T>
        {
            //_world.KeepPreviousState<T>();
        }
    }
}
