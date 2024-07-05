using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxStartClientRegistrationResultHandle : StartClientRegistrationResultHandle
    {
        protected override void FreeStartClientRegistrationResult(nint handle)
        {
            free_start_client_registration_result(handle);
        }

        protected override LinuxStringHandle GetStateClientRegistrationResultState(nint handle)
        {
            return get_start_client_registration_result_state(handle);
        }

        protected override LinuxStringHandle GetStartClientRegistrationResultRequest(nint handle)
        {
            return get_start_client_registration_result_request(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_start_client_registration_result(IntPtr handle);

        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_client_registration_result_state(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_client_registration_result_request(IntPtr handle);
    }
}
