using System.Runtime.InteropServices;

namespace OPAQUE.Net.Types.Handles
{
    abstract public class BaseHandle<T> : SafeHandle
    {
        public override bool IsInvalid => this.handle == IntPtr.Zero;

        private T? _value;
        public T? Value
        {
            get => CheckGet();
            private set => _value = value;
        }

        protected BaseHandle() : base(IntPtr.Zero, true) { }

        private T CheckGet()
        {
            if (_value != null)
            {
                return _value;
            }

            _value = GetValue();
            return _value;
        }

        abstract protected T GetValue();
    }
}
