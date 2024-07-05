using System.Runtime.InteropServices;
using System.Text;

namespace OPAQUE.Net.Base.Handles
{
    public class StringHandle : BaseHandle<string>
    {
        public StringHandle() : base() { }

        protected override void DoRelease()
        {
            FreeString(handle);
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

        protected virtual void FreeString(nint handle) { }
    }
}
