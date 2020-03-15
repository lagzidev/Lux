using System;
namespace LuxEngine
{
    public enum LuxStatus
    {
        SUCCESS = 0,
        COMPONENTMANAGER_BASECOMPONENTMANAGER_GENERATECOMPONENTTYPE_TOO_MANY_COMPONENT_TYPES,
        BASESYSTEM_INIT_WORLD_IS_NULL,
    }

    public class LuxException : Exception
    {
        public LuxException(LuxStatus status, uint extra_info, string external_message = "") :
            base(_s_create_error_message(status, extra_info, external_message))
        {
        }

        private static string _s_create_error_message(LuxStatus status, uint extra_info, string external_message)
        {
            return $"LuxException: LuxStatus: {status}, extra_info: {extra_info}, external_message: {external_message}\n";
        }
    }
}
