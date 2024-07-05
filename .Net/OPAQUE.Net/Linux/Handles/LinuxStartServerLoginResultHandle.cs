using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxStartServerLoginResultHandle : StartServerLoginResultHandle
    {
        protected override void FreeStartServerLoginResult(nint handle)
        {
            free_start_server_login_result(handle);
        }

        protected override LinuxStringHandle GetStartServerLoginResponseState(nint handle)
        {
            return get_start_server_login_response_state(handle);
        }

        protected override LinuxStringHandle GetStartServerLoginResponseResponse(nint handle)
        {
            return get_start_server_login_response_response(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_start_server_login_result(IntPtr handle);

        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_server_login_response_state(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_start_server_login_response_response(IntPtr handle);
    }
}
