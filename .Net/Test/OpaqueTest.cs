using OPAQUE.Net;
using OPAQUE.Net.Types;

namespace Test
{
    [TestClass]
    public class OpaqueTest
    {
        private void SetupAndRegister(string userIdentifier, string password, CustomIdentifiers? identifiers = null)
        {
            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            string? serverSecret = server.SetupServer();
            //var clientStartRegistration = client.StartClientRegistration(new StartClientRegistrationParams(password));
            //var serverStartRegistration = server.CreateServerRegistrationResponse(new CreateServerRegistrationResponseParams(serverSecret, userIdentifier, clientStartRegistration));

            Console.WriteLine(serverSecret);
        }


        [TestMethod]
        public void TestMethod1()
        {
            SetupAndRegister("test", "test");
        }
    }
}