using OPAQUE.Net;
using OPAQUE.Net.Types.Exceptions;
using OPAQUE.Net.Types.Parameters;
using OPAQUE.Net.Types.Results;

namespace Test
{
    [TestClass]
    public class OpaqueTest
    {
        private void SetupAndRegister(string userIdentifier, string password, string? clientIdentifier, string? serverIdentifier, KSFConfig? config,
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

            if (!server.CreateRegistrationResponse(serverSecret!, userIdentifier, clientRegistrationResult!.RegistrationRequest,
                out string? serverRegistrationResponse))
            {
                throw new Exception();
            }

            if (!client.FinishRegistration(password, serverRegistrationResponse!,
                clientRegistrationResult.ClientRegistrationState, clientIdentifier, serverIdentifier, config,
                out FinishClientRegistrationResult? finishRegistrationResult))
            {
                throw new Exception();
            }

            serverSetup = serverSecret!;
            registrationRecord = finishRegistrationResult!.RegistrationRecord;
            exportKey = finishRegistrationResult.ExportKey;
            serverStaticPublicKey = finishRegistrationResult.ServerStaicPublicKey;
        }

        [TestMethod]
        public void FullRegistrationAndLoginSucceeds()
        {
            string userIdentifier = "user123";
            string password = "hunter42";

            SetupAndRegister(userIdentifier, password, null, null, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                null, null, out StartServerLoginResult? serverLoginResult, out _))
            {
                throw new Exception();
            }

            if (!client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password,
                null, null, null, out FinishClientLoginResult? finishClientLoginResult))
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

        [TestMethod]
        public void FullRegistrationAndLoginSucceedsWithRFCDraftRecommendKSF()
        {
            string userIdentifier = "user123";
            string password = "hunter42";

            KSFConfig config = KSFConfig.Create(KSFConfigType.RfcDraftRecommended);

            SetupAndRegister(userIdentifier, password, null, null, config, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                null, null, out StartServerLoginResult? serverLoginResult, out _))
            {
                throw new Exception();
            }

            if (!client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password,
                null, null, config, out FinishClientLoginResult? finishClientLoginResult))
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

        [TestMethod]
        public void FullRegistrationAndLoginSucceedsWithCustomKSFConfig()
        {
            string userIdentifier = "user123";
            string password = "hunter42";

            KSFConfig config = KSFConfig.Create(KSFConfigType.Custom, 1, 65536, 4);

            SetupAndRegister(userIdentifier, password, null, null, config, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                null, null, out StartServerLoginResult? serverLoginResult, out _))
            {
                throw new Exception();
            }

            if (!client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password,
                null, null, config, out FinishClientLoginResult? finishClientLoginResult))
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

        [TestMethod]
        public void MisMatchedConfigFailsLogin()
        {
            string userIdentifier = "user123";
            string password = "hunter42";

            // use default config (Recommended)
            SetupAndRegister(userIdentifier, password, null, null, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                null, null, out StartServerLoginResult? serverLoginResult, out _))
            {
                throw new Exception();
            }

            KSFConfig config = KSFConfig.Create(KSFConfigType.RfcDraftRecommended);

            bool finishLoginSucceeded = client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password,
                null, null, config, out FinishClientLoginResult? finishClientLoginResult);

            Assert.AreEqual(false, finishLoginSucceeded);
        }

        [TestMethod]
        public void IncorrectPasswordFailsLogin()
        {
            string userIdentifier = "user123";
            string rightPassword = "hunter42";
            string wrongPassword = "hunter43";

            SetupAndRegister(userIdentifier, rightPassword, null, null, null, out string serverSetup,
                out string registrationRecord, out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(rightPassword, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                null, null, out StartServerLoginResult? serverLoginResult, out _))
            {
                throw new Exception();
            }

            client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, wrongPassword, null,
                null, null, out FinishClientLoginResult? finishClientLoginResult);

            Assert.IsNull(finishClientLoginResult);
        }

        [TestMethod]
        public void IncorrectClientIdentifierFails()
        {
            string userIdentifier = "user123";
            string password = "hunter2";
            string clientIdentifier = "client123";

            SetupAndRegister(userIdentifier, password, clientIdentifier, null, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? startClientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, startClientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                clientIdentifier, null, out StartServerLoginResult? startServerLoginResult, out _))
            {
                throw new Exception();
            }

            client.FinishLogin(startClientLoginResult.ClientLoginState, startServerLoginResult!.LoginResponse, password, clientIdentifier + "abc",
                null, null, out FinishClientLoginResult? finishClientLoginResult);

            Assert.IsNull(finishClientLoginResult);
        }

        [TestMethod]
        public void IncorrectServerIdentifierFails()
        {
            string userIdentifier = "user123";
            string password = "hunter2";
            string serverIdentifier = "server-ident";

            SetupAndRegister(userIdentifier, password, null, serverIdentifier, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            OpaqueServer server = new OpaqueServer();
            OpaqueClient client = new OpaqueClient();

            if (!client.StartLogin(password, out StartClientLoginResult? startClientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, startClientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord, null,
                serverIdentifier + "-abc", out StartServerLoginResult? startServerLoginResult, out _))
            {
                throw new Exception();
            }

            client.FinishLogin(startClientLoginResult.ClientLoginState, startServerLoginResult!.LoginResponse, password, null, serverIdentifier, null, out FinishClientLoginResult? result);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ClientMethodsThrowsOnInvalidParams()
        {
            OpaqueClient client = new OpaqueClient();

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.StartRegistration("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("", "Value", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("Value", "", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("Value", "Value", "", null, null, null, out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.StartLogin("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("", "Value", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("Value", "", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("Value", "Value", "", null, null, null, out _));
        }

        [TestMethod]
        public void ServerMethodsThrowsOnInvalidParams()
        {
            OpaqueServer server = new OpaqueServer();

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.GetPublicKey("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("", "Value", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("Value", "", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("Value", "Value", "", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("", "Value", "Value", null, null, null, out _, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("Value", "", "Value", null, null, null, out _, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("Value", "Value", "", null, null, null, out _, out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.FinishLogin("", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.FinishLogin("Value", "", out _));
        }
    }
}