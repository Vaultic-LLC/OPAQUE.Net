using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Types.Handles
{
    public class StartClientRegistrationResultHandle : BaseHandle<StartClientRegistrationResult>
    {
        protected override void DoRelease()
        {
            free_start_client_registration_result(handle);
        }

        protected override StartClientRegistrationResult? GetValue()
        {
            string? state = get_start_client_registration_result_state(handle).GetAndRelease();
            string? request = get_start_client_registration_result_request(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(request))
            {
                return new StartClientRegistrationResult(state, request);
            }

            return null;
        }

        [DllImport("opaque.dll")]
        private static extern void free_start_client_registration_result(IntPtr handle);

        [DllImport("opaque.dll")]
        private static extern StringHandle get_start_client_registration_result_state(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern StringHandle get_start_client_registration_result_request(IntPtr handle);
    }
}
