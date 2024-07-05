using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsStringHandle : StringHandle
    {
        protected override void FreeString(nint handle)
        {
            free_string(handle);
        }

        [DllImport("opaque.dll")]
        private static extern void free_string(nint handle);
    }
}
