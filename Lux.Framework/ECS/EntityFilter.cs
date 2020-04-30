using System;
namespace Lux.Framework.ECS
{
    public interface IEntityFilter
    {
        /// <summary>
        /// Decides if a system should run on an entity based on that entity's
        /// set of components.
        /// </summary>
        /// <param name="components">The list of components</param>
        /// <returns><c>true</c> if the system should run, <c>false</c> otherwise.</returns>
        /// todo: remove World and Entity parameters if they are not used
        bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, Guid[] componentTypes, params AInternalComponent[] components);
    }

    /// <summary>
    /// Filters entities based on if they have all the components the system
    /// requested in the parameters list.
    /// </summary>
    public class DefaultEntityFilter : IEntityFilter
    {
        public bool Filter(World world, Entity entity, ASystemAttribute[] systemAttributes, Guid[] componentTypes, params AInternalComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                // If requested component is not null, move on to the next component
                if (components[i] != null)
                {
                    continue;
                }

                // Check if the component is optional, thus allowed to be null
                bool optionalFound = false;
                for (int s = 0; s < systemAttributes.Length && !optionalFound; s++)
                {
                    if (systemAttributes[s] is Optional optional)
                    {
                        if (optional.ComponentTypeGuid == componentTypes[i])
                        {
                            optionalFound = true;
                        }
                    }
                }

                // Component is optional, move on
                if (optionalFound)
                {
                    continue;
                }

                return false;
            }

            return true;
        }
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


}
