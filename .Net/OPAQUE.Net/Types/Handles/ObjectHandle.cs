namespace OPAQUE.Net.Types.Handles
{
    public class ObjectHandle<T> : BaseHandle<T>
    {
        public ObjectHandle() : base() { }

        public override bool IsInvalid => throw new NotImplementedException();

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                free_object(handle);
            }

            return true;
        }

        protected override T GetValue()
        {
            throw new NotImplementedException();
        }
    }
}
