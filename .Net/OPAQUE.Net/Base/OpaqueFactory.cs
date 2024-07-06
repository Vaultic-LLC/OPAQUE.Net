using System.Reflection;
using System.Runtime.InteropServices;

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
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), ImportResolver);

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

        private const string linuxOpauqeName = "libopaque.so";
        private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (NativeLibrary.TryLoad(Path.Combine("lib/", linuxOpauqeName), out IntPtr libHandle))
            {
                return libHandle;
            }

            return IntPtr.Zero;
        }
    }
}
