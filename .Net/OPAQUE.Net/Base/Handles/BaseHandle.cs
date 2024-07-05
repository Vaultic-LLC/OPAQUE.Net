using System.Runtime.InteropServices;

namespace OPAQUE.Net.Base.Handles
{
    abstract public class BaseHandle<T> : SafeHandle
    {
        public override bool IsInvalid => handle == nint.Zero;

        protected BaseHandle() : base(nint.Zero, true) { }

        public T? GetAndRelease()
        {
            T? value = GetValue();
            Dispose();

            return value;
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                DoRelease();
            }

            return true;
        }

        abstract protected void DoRelease();
        abstract protected T? GetValue();
    }
}
