namespace LuxEngine
{
    /// <summary>
    /// Signals the ECS to run the system only when this component doesn't exist
    /// for the entities.
    /// </summary>
    /// <typeparam name="T">The component</typeparam>
    public class Not<T> : AComponent<Not<T>> where T : AComponent<T>
    {
        T Value;
    }

    /// <summary>
    /// Wrap this in a component to get their previous state.
    /// The system won't run if there's no previous state available
    /// (e.g. if the component was only just created.)
    /// </summary>
    /// <typeparam name="T">The component</typeparam>
    public class Previous<T> : AComponent<Previous<T>> where T : AComponent<T>
    {
        public T Value;
    }

    public abstract class ASuperSystem
    {
        public void RegisterTo(World world)
        {
            Init(world);
            LoadContent(world);
            Update(world);
            UpdateFixed(world);
            Draw(world);
        }

        public abstract void Init(World world);
        public abstract void LoadContent(World world);
        public abstract void Update(World world);
        public abstract void UpdateFixed(World world);
        public abstract void Draw(World world);
    }
}
