using System.Runtime.InteropServices;
using System.Text;

namespace OPAQUE.Net.Types.Handles
{
    public class StringHandle : BaseHandle<string>
    {
        public StringHandle() : base() { }

        protected override void DoRelease()
        {
            free_string(handle);
        }

        protected override string GetValue()
        {
            int len = 0;
            while (Marshal.ReadByte(handle, len) != 0)
            {
                ++len;
            }

            byte[] buffer = new byte[len];
            Marshal.Copy(handle, buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer);
        }

        [DllImport("opaque.dll")]
        private static extern void free_string(IntPtr handle);
    }
}
