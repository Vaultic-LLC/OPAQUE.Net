using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;


namespace OPAQUE.Net.Types.Handles
{
    internal class StartClientLoginResultHandle : BaseHandle<StartClientLoginResult>
    {
        protected override void DoRelease()
        {
            free_start_client_login_result(handle);
        }

        protected override StartClientLoginResult? GetValue()
        {
            string? state = get_start_client_login_result_state(handle).GetAndRelease();
            string? request = get_start_client_login_result_request(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(request))
            {
                return new StartClientLoginResult(state, request);
            }

            return null;
        }

        [DllImport("opaque")]
        private static extern void free_start_client_login_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern StringHandle get_start_client_login_result_state(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_start_client_login_result_request(IntPtr handle);
    }
}
