using System;
namespace LuxEngine.ECS
{
    public interface IEntityFilter
    {
        IEntityFilter[] UnderlyingFilters { get; set; }

        /// <summary>
        /// Decides if a system should run on an entity based on that entity's
        /// set of components.
        /// </summary>
        /// <param name="components">The list of components</param>
        /// <returns><c>true</c> if the system should run, <c>false</c> otherwise.</returns>
        bool Filter(params AInternalComponent[] components);
    }

    public interface IExclude
    {
    }

    /// <summary>
    /// Let's the API know that the real component is inside
    /// </summary>
    public interface IWrapper<T>
    {
        T Value { get; set; }
    }

    public interface ISingleton
    {
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
    //    /// TODO: The problem here is that the componennts array is filled with
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
    /// Doesn't run a system if one of the components is null
    /// </summary>
    public class DefaultEntityFilter : IEntityFilter
    {
        public IEntityFilter[] UnderlyingFilters { get; set; }

        public DefaultEntityFilter()
        {
            UnderlyingFilters = null;
        }

        public bool Filter(params AInternalComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                // If requested component is null, don't run the system
                if (components[i] == null)
                {
                    return false;
                }
            }

            // If exist let the underlying filter decide, otherwise return true.
            if (UnderlyingFilters != null)
            {
                for (int i = 0; i < UnderlyingFilters.Length; i++)
                {
                    if (!UnderlyingFilters[i].Filter(components))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
