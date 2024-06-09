using OPAQUE.Net;
using OPAQUE.Net.Types.Results;

namespace Test
{
    [TestClass]
    public class OpaqueTest
    {
        private void SetupAndRegister(string userIdentifier, string password, string? clientIdentifier, string? serverIdentifier)
        {
            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!server.SetupServer(out string? serverSecret))
            {
                throw new Exception();
            }

            if (!client.StartRegistration(password, out StartClientRegistrationResult? clientRegistrationResult))
            {
                throw new Exception();
            }

            if (!server.CreateRegistrationResponse(serverSecret!, userIdentifier, clientRegistrationResult!.RegistrationRequest, out string? serverRegistrationResponse))
            {
                throw new Exception();
            }

            if (!client.FinishRegistration(password, serverRegistrationResponse!,
                clientRegistrationResult.ClientRegistrationState, clientIdentifier, serverIdentifier, out FinishClientRegistrationResult? finishRegistrationResult))
            {
                throw new Exception();
            }

            Console.WriteLine(finishRegistrationResult!.ExportKey);
        }


        [TestMethod]
        public void TestMethod1()
        {
            SetupAndRegister("test", "test", null, null);
        }
    }
}