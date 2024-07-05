using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxStartClientLoginResultHandle : StartClientLoginResultHandle
    {
        protected override void FreeStateClientLoginResult(nint handle)
        {
            free_start_client_login_result(handle);
        }

        protected override LinuxStringHandle GetStartClientLoginResultState(nint handle)
        {
            return get_start_client_login_result_state(handle);
        }

        protected override LinuxStringHandle GetStartClientLoginResultRequest(nint handle)
        {
            return get_start_client_login_result_request(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_start_client_login_result(IntPtr handle);

        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_client_login_result_state(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_client_login_result_request(IntPtr handle);
    }
}
