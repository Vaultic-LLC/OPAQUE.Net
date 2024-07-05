namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        static private Func<IOpaqueClient> _clientConstructor;
        static private Func<IOpaqueServer> _serverConstructor;

        static OpaqueFactory()
        {
            if (OperatingSystem.IsWindows())
            {
                _clientConstructor = () => new WindowsOpaqueClient();
                _serverConstructor = () => new WindowsOpaqueServer();
            }
            else if (OperatingSystem.IsLinux())
            {
                _clientConstructor = () => new LinuxOpaqueClient();
                _serverConstructor = () => new LinuxOpaqueServer();
            }
            else
            {
                throw new Exception($"Operation system not supported");
            }
        }

        public static IOpaqueClient CreateClient() => _clientConstructor.Invoke();
        public static IOpaqueServer CreateServer() => _serverConstructor.Invoke();
    }
}
