using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxStringHandle : StringHandle
    {
        protected override void FreeString(nint handle)
        {
            free_string(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_string(nint handle);
    }
}
