using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsStartClientRegistrationResultHandle : StartClientRegistrationResultHandle
    {
        protected override void FreeStartClientRegistrationResult(nint handle)
        {
            free_start_client_registration_result(handle);
        }

        protected override WindowsStringHandle GetStateClientRegistrationResultState(nint handle)
        {
            return get_start_client_registration_result_state(handle);
        }

        protected override WindowsStringHandle GetStartClientRegistrationResultRequest(nint handle)
        {
            return get_start_client_registration_result_request(handle);
        }

        [DllImport("opaque")]
        private static extern void free_start_client_registration_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern WindowsStringHandle get_start_client_registration_result_state(IntPtr handle);
        [DllImport("opaque")]
        private static extern WindowsStringHandle get_start_client_registration_result_request(IntPtr handle);
    }
}
