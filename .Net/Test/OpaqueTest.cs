using OPAQUE.Net;
using OPAQUE.Net.Types.Results;

namespace Test
{
    [TestClass]
    public class OpaqueTest
    {
        private void SetupAndRegister(string userIdentifier, string password, string? clientIdentifier, string? serverIdentifier, 
            out string serverSetup, out string registrationRecord, out string exportKey, out string serverStaticPublicKey)
        {
            serverSetup = "";
            registrationRecord = "";
            exportKey = "";
            serverStaticPublicKey = "";

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!server.CreateSetup(out string? serverSecret))
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

            serverSetup = serverSecret!;
            registrationRecord = finishRegistrationResult!.RegistrationRecord;
            exportKey = finishRegistrationResult.ExportKey;
            serverStaticPublicKey = finishRegistrationResult.ServerStaicPublicKey;
        }


        [TestMethod]
        public void FullRegistrationAndLoginFlow()
        {
            string userIdentifier = "user123";
            string password = "hunter42";

            SetupAndRegister(userIdentifier, password, null, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);         

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord, null, null, out StartServerLoginResult? serverLoginResult))
            {
                throw new Exception();
            }

            if (!client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password, null, null, out FinishClientLoginResult? finishClientLoginResult))
            {
                throw new Exception();
            }

            if (!server.GetPublicKey(serverSetup, out string? serverPublicKey))
            {
                throw new Exception();
            }

            Assert.AreEqual(exportKey, finishClientLoginResult!.ExportKey);
            Assert.AreEqual(serverStaticPublicKey, finishClientLoginResult.ServerStaticPublicKey);
            Assert.AreEqual(finishClientLoginResult.ServerStaticPublicKey, serverPublicKey);

            if (!server.FinishLogin(serverLoginResult.ServerLoginState, finishClientLoginResult.FinishLoginRequest, out string? sessionKey))
            {
                throw new Exception();
            }

            Assert.AreEqual(sessionKey, finishClientLoginResult.SessionKey);
        }
    }
}