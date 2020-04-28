using System;
using System.Reflection;

namespace LuxEngine.ECS
{
    // TODO: Make sure all the component arrays are next to each other in memory

    public class Systems
    {
        private readonly ASystem[] _systems;
        public int Count { get; private set; }

        internal Systems()
        {
            _systems = new ASystem[HardCodedConfig.MAX_SYSTEMS]; // TODO: Have an option for the user to change this at compile time
            Count = 0;
        }

        internal void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            for (int i = 0; i < Count; i++)
            {
                _systems[i].Invoke(world, entity, filter);
            }
        }

        internal void Register(World world)
        {
            for (int i = 0; i < Count; i++)
            {
                _systems[i].Register(world);
            }
        }

        private void Add(ASystem system)
        {
            _systems[Count] = system;
            Count++;
        }

        #region Systems Add Declerations
        /*  We must do it manually because C#  */

        public Systems Add(Action system)
        {
            Add(new System(system));
            return this;
        }

        /// <summary>
        /// Adds a system to the list
        /// </summary>
        /// <typeparam name="T1">Component that the system method operates on</typeparam>
        /// <param name="system">The system method that will be invoked</param>
        /// <returns>This instance. Enables chaining Add calls.</returns>
        public Systems Add<T1>(Action<T1> system)
            where T1 : AComponent<T1>
        {
            Add(new System<T1>(system));
            return this;
        }

        public Systems Add<T1, T2>(Action<T1, T2> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
        {
            Add(new System<T1, T2>(system));
            return this;
        }

        public Systems Add<T1, T2, T3>(Action<T1, T2, T3> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
            where T3 : AComponent<T3>
        {
            Add(new System<T1, T2, T3>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4>(Action<T1, T2, T3, T4> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
            where T3 : AComponent<T3>
            where T4 : AComponent<T4>
        {
            Add(new System<T1, T2, T3, T4>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
            where T3 : AComponent<T3>
            where T4 : AComponent<T4>
            where T5 : AComponent<T5>
        {
            Add(new System<T1, T2, T3, T4, T5>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
            where T3 : AComponent<T3>
            where T4 : AComponent<T4>
            where T5 : AComponent<T5>
            where T6 : AComponent<T6>
        {
            Add(new System<T1, T2, T3, T4, T5, T6>(system));
            return this;
        }

        public Systems Add<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> system)
            where T1 : AComponent<T1>
            where T2 : AComponent<T2>
            where T3 : AComponent<T3>
            where T4 : AComponent<T4>
            where T5 : AComponent<T5>
            where T6 : AComponent<T6>
            where T7 : AComponent<T7>
        {
            Add(new System<T1, T2, T3, T4, T5, T6, T7>(system));
            return this;
        }

        #endregion
    }

    internal abstract class ASystem
    {
        public ASystemAttribute[] SystemAttributes;

        public ASystem()
        {
            SystemAttributes = null;
        }

        /// <summary>
        /// Invokes the system method
        /// </summary>
        /// <param name="world">World the system operates in</param>
        /// <param name="entity">Entity to run the system on</param>
        public abstract void Invoke(World world, Entity entity, IEntityFilter filter);

        /// <summary>
        /// Registers all components used by the system to the world
        /// </summary>
        /// <param name="world">World to register the component to</param>
        protected abstract void RegisterComponents(World world);
        public void Register(World world)
        {
            // Handle custom filters
            SystemAttributes = (ASystemAttribute[])GetMethodInfo().GetCustomAttributes(typeof(ASystemAttribute), false);

            // Register the components the system uses
            RegisterComponents(world);
        }

        /// <summary>
        /// Gets the method info of the underlying system method
        /// </summary>
        /// <returns>Method info of the system method</returns>
        protected abstract MethodInfo GetMethodInfo();
    }

    #region System Declerations

    internal class System : ASystem
    {
        private readonly Action system;

        public System(Action system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            if (filter.Filter(world, entity, SystemAttributes))
            {
                system();
            }
        }

        protected override void RegisterComponents(World world)
        {
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1> : ASystem
        where T1 : AComponent<T1>
    {
        private Action<T1> system;

        public System(Action<T1> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);

            if (filter.Filter(world, entity, SystemAttributes, c1))
            {
                system(c1);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
    {
        private Action<T1, T2> system;

        public System(Action<T1, T2> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2))
            {
                system(c1, c2);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
    {
        private Action<T1, T2, T3> system;

        public System(Action<T1, T2, T3> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);
            world.UnpackEntityOrSingleton(entity, out T3 c3);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2, c3))
            {
                system(c1, c2, c3);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
            world.Register<T3>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
    {
        private Action<T1, T2, T3, T4> system;

        public System(Action<T1, T2, T3, T4> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);
            world.UnpackEntityOrSingleton(entity, out T3 c3);
            world.UnpackEntityOrSingleton(entity, out T4 c4);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2, c3, c4))
            {
                system(c1, c2, c3, c4);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
            world.Register<T3>();
            world.Register<T4>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
        where T5 : AComponent<T5>
    {
        private Action<T1, T2, T3, T4, T5> system;

        public System(Action<T1, T2, T3, T4, T5> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);
            world.UnpackEntityOrSingleton(entity, out T3 c3);
            world.UnpackEntityOrSingleton(entity, out T4 c4);
            world.UnpackEntityOrSingleton(entity, out T5 c5);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2, c3, c4, c5))
            {
                system(c1, c2, c3, c4, c5);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
            world.Register<T3>();
            world.Register<T4>();
            world.Register<T5>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5, T6> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
        where T5 : AComponent<T5>
        where T6 : AComponent<T6>
    {
        private Action<T1, T2, T3, T4, T5, T6> system;

        public System(Action<T1, T2, T3, T4, T5, T6> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);
            world.UnpackEntityOrSingleton(entity, out T3 c3);
            world.UnpackEntityOrSingleton(entity, out T4 c4);
            world.UnpackEntityOrSingleton(entity, out T5 c5);
            world.UnpackEntityOrSingleton(entity, out T6 c6);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2, c3, c4, c5, c6))
            {
                system(c1, c2, c3, c4, c5, c6);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
            world.Register<T3>();
            world.Register<T4>();
            world.Register<T5>();
            world.Register<T6>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5, T6, T7> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
        where T5 : AComponent<T5>
        where T6 : AComponent<T6>
        where T7 : AComponent<T7>
    {
        private Action<T1, T2, T3, T4, T5, T6, T7> system;

        public System(Action<T1, T2, T3, T4, T5, T6, T7> system)
        {
            this.system = system;
        }

        public override void Invoke(World world, Entity entity, IEntityFilter filter)
        {
            world.UnpackEntityOrSingleton(entity, out T1 c1);
            world.UnpackEntityOrSingleton(entity, out T2 c2);
            world.UnpackEntityOrSingleton(entity, out T3 c3);
            world.UnpackEntityOrSingleton(entity, out T4 c4);
            world.UnpackEntityOrSingleton(entity, out T5 c5);
            world.UnpackEntityOrSingleton(entity, out T6 c6);
            world.UnpackEntityOrSingleton(entity, out T7 c7);

            if (filter.Filter(world, entity, SystemAttributes, c1, c2, c3, c4, c5, c6, c7))
            {
                system(c1, c2, c3, c4, c5, c6, c7);
            }
        }

        protected override void RegisterComponents(World world)
        {
            world.Register<T1>();
            world.Register<T2>();
            world.Register<T3>();
            world.Register<T4>();
            world.Register<T5>();
            world.Register<T6>();
            world.Register<T7>();
        }

        protected override MethodInfo GetMethodInfo()
        {
            return system.GetMethodInfo();
        }
    }

    #endregion
}
