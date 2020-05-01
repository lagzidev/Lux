using System;
using System.Reflection;

namespace Lux.Framework.ECS
{
    // TODO: Make sure all the component arrays are next to each other in memory
    // TODO: Reduce the seperation of Systems, if possible to only one Systems object that is filtered some other way during registration

    public class Systems
    {
        private readonly ASystem[] _systems;
        private bool _registrationDone;

        public int Count { get; private set; }

        internal Systems()
        {
            _registrationDone = false;
            _systems = new ASystem[HardCodedConfig.MAX_SYSTEMS]; // TODO: Have the user provide this
            Count = 0;
        }

        internal ASystem this[int i]
        {
            get { return _systems[i]; }
        }

        internal void Register(World world)
        {
            for (int i = 0; i < Count; i++)
            {
                _systems[i].Register(world);
            }

            _registrationDone = true;
        }

        private void Add(ASystem system)
        {
            _systems[Count] = system;
            Count++;
        }

        #region Systems Add Declerations

        public Systems Add(Action system)
        {
            if (_registrationDone)
            {
                LuxCommon.Assert(false); // Can't register more systems at this point
                return this;
            }

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

    /// <summary>
    /// The abstract system from which all systems inherit
    /// </summary>
    public abstract class ASystem
    {
        private readonly ComponentMask _componentsMask;
        private readonly ComponentMask _singletonMask;

        public ASystemAttribute[] SystemAttributes;
        public Type[] ComponentTypes;
        public bool IsSingletonSystem;

        public ASystem()
        {
            _componentsMask = new ComponentMask(HardCodedConfig.MAX_GAME_COMPONENT_TYPES);
            _singletonMask = new ComponentMask(HardCodedConfig.MAX_GAME_COMPONENT_TYPES);
            SystemAttributes = null;
            ComponentTypes = null;
            IsSingletonSystem = true;
        }

        public bool HasAttribute<T>() where T : ASystemAttribute
        {
            for (int i = 0; i < SystemAttributes.Length; i++)
            {
                if (SystemAttributes[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Invokes the system method
        /// </summary>
        /// <param name="world">World the system operates in</param>
        /// <param name="entities">All entities in the world</param>
        /// <param name="entityMask"></param>
        public abstract void Invoke(World world, Entity[] entities);

        protected bool CanRun(World world, Entity entity)
        {
            bool singletonMatches = _singletonMask.Matches(world.GetSingletonEntityMask());
            if (IsSingletonSystem)
            {
                return singletonMatches;
            }

            return _componentsMask.Matches(world.GetEntityMask(entity)) && singletonMatches;
        }

        //public void InvokeSingleton(World world, Entity[] entities, ComponentMask singletonEntityMask)
        //{
        //    if (_singletonMask.Matches(singletonEntityMask))
        //    {
        //        Invoke(world, entities);
        //    }
        //}

        /// <summary>
        /// Registers all components used by the system to the world
        /// </summary>
        /// <param name="world">World to register the component to</param>
        protected abstract void RegisterComponents(World world);
        public void Register(World world)
        {
            // Get custom attributes
            SystemAttributes = (ASystemAttribute[])GetMethodInfo().GetCustomAttributes(typeof(ASystemAttribute), false);

            // Register the components the system uses
            RegisterComponents(world);
        }

        /// <summary>
        /// Registers a single component to the world
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="world">System's world</param>
        protected void RegisterComponent<T>(World world) where T : AComponent<T>
        {
            // If component is singleton
            if (typeof(ISingleton).IsAssignableFrom(typeof(T)))
            {
                world.RegisterSingleton<T>();
            }
            else
            {
                world.Register<T>();
                IsSingletonSystem = false;
            }

            // Check if should add the component as required
            bool addRequiredComponent = true;
            for (int i = 0; i < SystemAttributes.Length; i++)
            {
                if (SystemAttributes[i] is Optional optional)
                {
                    if (optional.ComponentType == typeof(T))
                    {
                        addRequiredComponent = false;
                    }
                }
                //else if (SystemAttributes[i] is Exclude excluded)
                //{
                //    if (excluded.ComponentType == typeof(T))
                //    {
                //        addRequiredComponent = false;

                //    }
                //}
            }

            // If component is not optional, add it to mask
            if (addRequiredComponent)
            {
                // If component is a singleton type
                if (typeof(ISingleton).IsAssignableFrom(typeof(T)))
                {
                    _singletonMask.AddComponent<T>();
                }
                else
                {
                    IsSingletonSystem = false;
                    _componentsMask.AddComponent<T>();
                }
            }
        }

        /// <summary>
        /// Gets the method info of the underlying system method
        /// </summary>
        /// <returns>Method info of the system method</returns>
        protected abstract MethodInfo GetMethodInfo();
    }

    #region System Declerations

    public class System : ASystem
    {
        private readonly Action _system;

        public System(Action system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            _system();
        }

        protected override void RegisterComponents(World world)
        {
            ComponentTypes = new Type[0];
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    public class System<T1> : ASystem
        where T1 : AComponent<T1>
    {
        private readonly Action<T1> _system;

        public System(Action<T1> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                /*
                    FIGURE OUT WHY THE HELL THE PERFORMANCE IS BETTER
                    WITH UpdateSingleComponentSpan when it's practically the same

                    UpdateTwoComponentsSpan makes sense because it doesn't think
                    about the order, and is not functionally working.
                 */
                if (CanRun(world, c1[i].Entity))
                {
                    _system(c1[i]);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);

            ComponentTypes = new Type[]
            {
                typeof(T1)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
    {
        private readonly Action<T1, T2> _system;

        public System(Action<T1, T2> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            // TODO: Test this new way against the old entities registration way?

            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    _system(c1[i], c2);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
    {
        private readonly Action<T1, T2, T3> _system;

        public System(Action<T1, T2, T3> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    world.Unpack(c1[i].Entity, out T3 c3);
                    _system(c1[i], c2, c3);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
    {
        private readonly Action<T1, T2, T3, T4> _system;

        public System(Action<T1, T2, T3, T4> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    world.Unpack(c1[i].Entity, out T3 c3);
                    world.Unpack(c1[i].Entity, out T4 c4);
                    _system(c1[i], c2, c3, c4);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    internal class System<T1, T2, T3, T4, T5> : ASystem
        where T1 : AComponent<T1>
        where T2 : AComponent<T2>
        where T3 : AComponent<T3>
        where T4 : AComponent<T4>
        where T5 : AComponent<T5>
    {
        private readonly Action<T1, T2, T3, T4, T5> _system;

        public System(Action<T1, T2, T3, T4, T5> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    world.Unpack(c1[i].Entity, out T3 c3);
                    world.Unpack(c1[i].Entity, out T4 c4);
                    world.Unpack(c1[i].Entity, out T5 c5);
                    _system(c1[i], c2, c3, c4, c5);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
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
        private readonly Action<T1, T2, T3, T4, T5, T6> _system;

        public System(Action<T1, T2, T3, T4, T5, T6> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    world.Unpack(c1[i].Entity, out T3 c3);
                    world.Unpack(c1[i].Entity, out T4 c4);
                    world.Unpack(c1[i].Entity, out T5 c5);
                    world.Unpack(c1[i].Entity, out T6 c6);
                    _system(c1[i], c2, c3, c4, c5, c6);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
            RegisterComponent<T6>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
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
        private readonly Action<T1, T2, T3, T4, T5, T6, T7> _system;

        public System(Action<T1, T2, T3, T4, T5, T6, T7> system)
        {
            _system = system;
        }

        public override void Invoke(World world, Entity[] entities)
        {
            var c1 = world.GetAllReadonly<T1>();

            for (int i = 0; i < c1.Length; i++)
            {
                if (CanRun(world, c1[i].Entity))
                {
                    world.Unpack(c1[i].Entity, out T2 c2);
                    world.Unpack(c1[i].Entity, out T3 c3);
                    world.Unpack(c1[i].Entity, out T4 c4);
                    world.Unpack(c1[i].Entity, out T5 c5);
                    world.Unpack(c1[i].Entity, out T6 c6);
                    world.Unpack(c1[i].Entity, out T7 c7);
                    _system(c1[i], c2, c3, c4, c5, c6, c7);
                }
            }
        }

        protected override void RegisterComponents(World world)
        {
            RegisterComponent<T1>(world);
            RegisterComponent<T2>(world);
            RegisterComponent<T3>(world);
            RegisterComponent<T4>(world);
            RegisterComponent<T5>(world);
            RegisterComponent<T6>(world);
            RegisterComponent<T7>(world);
            ComponentTypes = new Type[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
            };
        }

        protected override MethodInfo GetMethodInfo()
        {
            return _system.GetMethodInfo();
        }
    }

    #endregion
}
