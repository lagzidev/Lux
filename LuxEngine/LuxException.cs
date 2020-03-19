using System;
namespace LuxEngine
{
    public enum LuxStatus
    {
        SUCCESS = 0,
        COMPONENTMANAGER_BASECOMPONENTMANAGER_GENERATECOMPONENTTYPE_TOO_MANY_COMPONENT_TYPES,
        BASESYSTEM_INIT_WORLD_IS_NULL,
        WORLD_TRYUNPACK_COMPONENT_MANAGER_NOT_FOUND,
        ENTITYMAP_GETCOMPONENTINSTANCE_COMPONENT_DOES_NOT_EXIST_FOR_THIS_ENTITY,
        WORLD_UNPACK_COMPONENT_NOT_FOUND_FOR_ENTITY,
    }

    public class LuxException : Exception
    {
        public LuxException(LuxStatus status, int extra_info, string external_message = "") :
            base(_s_create_error_message(status, extra_info, external_message))
        {
        }

        private static string _s_create_error_message(LuxStatus status, int extra_info, string external_message)
        {
            return $"LuxException: LuxStatus: {status}, extra_info: {extra_info}, external_message: '{external_message}'\n";
        }
    }
}
