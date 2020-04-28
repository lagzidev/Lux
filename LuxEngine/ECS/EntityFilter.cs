using System;
namespace LuxEngine.ECS
{
    public abstract class ASystemAttribute : Attribute
    {
    }

    public interface IEntityFilter
    {
        /// <summary>
        /// Decides if a system should run on an entity based on that entity's
        /// set of components.
        /// </summary>
        /// <param name="components">The list of components</param>
        /// <returns><c>true</c> if the system should run, <c>false</c> otherwise.</returns>
        bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, params AInternalComponent[] components);
    }

    //public class ExcludeFilter : Attribute, IEntityFilter
    //{
    //    public IEntityFilter[] UnderlyingFilters { get; set; }
    //    private Type[] _typesToExclude;

    //    public ExcludeFilter(params Type[] typesToExclude)
    //    {
    //        UnderlyingFilters = null;
    //        _typesToExclude = typesToExclude;
    //    }

    //    /// <summary>
    //    /// TODO: The problem here is that the components array is filled with
    //    /// components that are requested in the parameter list of the method.
    //    /// So say we want to exclude entities with Transform. The method
    //    /// is going to have to put it in it's components list, which will then
    //    /// cause it to fail anyways because the DefaultEntityFilter filters all
    //    /// entities in the parameters list that are null.
    //    /// The solution to this is to give the filter components that aren't
    //    /// in the components list.
    //    /// </summary>
    //    /// <param name="components"></param>
    //    /// <returns></returns>
    //    public bool Filter(params AInternalComponent[] components)
    //    {
    //        for (int i = 0; i < components.Length; i++)
    //        {
    //            if (components[i] == null)
    //            {
    //                continue;
    //            }

    //            for (int s = 0; s < _typesToExclude.Length; s++)
    //            {
    //                if (components[i].GetType() == _typesToExclude[s])
    //                {

    //                }
    //            }
    //        }

    //        return true;
    //    }
    //}

    /// <summary>
    /// Filters entities based on if they have all the components the system
    /// requested in the parameters list.
    /// </summary>
    public class DefaultEntityFilter : IEntityFilter
    {
        public bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, params AInternalComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                // If requested component is null, don't run the system
                if (components[i] == null)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class OnAddComponent : ASystemAttribute, IEntityFilter
    {
        private static readonly DefaultEntityFilter _defaultEntity = new DefaultEntityFilter();
        public Type AddedComponentType;

        public OnAddComponent(Type addedType)
        {
            AddedComponentType = addedType;
        }

        public bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, params AInternalComponent[] components)
        {
            // Run the default filter first
            if (!_defaultEntity.Filter(world, entity, systemAttributes, components))
            {
                return false;
            }

            // Look for the OnAddComponent system attribute
            for (int i = 0; i < systemAttributes.Length; i++)
            {
                if (systemAttributes[i] is OnAddComponent onAddComponent)
                {
                    // If the added component type matches the one the system is
                    // looking for, let the system run
                    if (onAddComponent.AddedComponentType == AddedComponentType)
                    {
                        return true;
                    }
                }
            }

            // If system doesn't have OnAddComponent attribute, don't run it
            return false;
        }
    }
}
