using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Types.Handles
{
    public class StartServerLoginResultHandle : BaseHandle<StartServerLoginResult>
    {
        protected override void DoRelease()
        {
            free_start_server_login_result(handle);
        }

        protected override StartServerLoginResult? GetValue()
        {
            string? state = get_start_server_login_response_state(handle).GetAndRelease();
            string? response = get_start_server_login_response_response(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(response))
            {
                return new StartServerLoginResult(state, response);
            }

            return null;
        }

        [DllImport("opaque")]
        private static extern void free_start_server_login_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern StringHandle get_start_server_login_response_state(IntPtr handle); 
        [DllImport("opaque")]
        private static extern StringHandle get_start_server_login_response_response(IntPtr handle);
    }
}
