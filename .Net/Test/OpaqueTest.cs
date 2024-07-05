using OPAQUE.Net.Base.Exceptions;
using OPAQUE.Net.Base.Results;
using OPAQUE.Net.Factory;
using static OPAQUE.Net.Factory.OpaqueFactory;

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

            IOpaqueServer server = OpaqueFactory.CreateServer();
            IOpaqueClient client = OpaqueFactory.CreateClient();

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
                clientRegistrationResult.ClientRegistrationState, clientIdentifier, serverIdentifier, 
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

            SetupAndRegister(userIdentifier, password, null, null, out string serverSetup, out string registrationRecord,
                out string exportKey, out string serverStaticPublicKey);

            IOpaqueServer server = OpaqueFactory.CreateServer();
            IOpaqueClient client = OpaqueFactory.CreateClient();

            if (!client.StartLogin(password, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord, 
                null, null, out StartServerLoginResult? serverLoginResult))
            {
                throw new Exception();
            }

            if (!client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, password, 
                null, null, out FinishClientLoginResult? finishClientLoginResult))
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
        public void IncorrectPasswordFailsLogin()
        {
            string userIdentifier = "user123";
            string rightPassword = "hunter42";
            string wrongPassword = "hunter43";

            SetupAndRegister(userIdentifier, rightPassword, null, null, out string serverSetup, 
                out string registrationRecord, out string exportKey, out string serverStaticPublicKey);

            IOpaqueServer server = OpaqueFactory.CreateServer();
            IOpaqueClient client = OpaqueFactory.CreateClient();

            if (!client.StartLogin(rightPassword, out StartClientLoginResult? clientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, clientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord, 
                null, null, out StartServerLoginResult? serverLoginResult))
            {
                throw new Exception();
            }

            client.FinishLogin(clientLoginResult.ClientLoginState, serverLoginResult!.LoginResponse, wrongPassword, null, 
                null, out FinishClientLoginResult? finishClientLoginResult);

            Assert.IsNull(finishClientLoginResult);
        }

        [TestMethod]
        public void IncorrectClientIdentifierFails()
        {
            string userIdentifier = "user123";
            string password = "hunter2";
            string clientIdentifier = "client123";

            SetupAndRegister(userIdentifier, password, clientIdentifier, null, out string serverSetup, out string registrationRecord, 
                out string exportKey, out string serverStaticPublicKey);

            IOpaqueServer server = OpaqueFactory.CreateServer();
            IOpaqueClient client = OpaqueFactory.CreateClient();

            if (!client.StartLogin(password, out StartClientLoginResult? startClientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, startClientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord,
                clientIdentifier, null, out StartServerLoginResult? startServerLoginResult))
            {
                throw new Exception();
            }

            client.FinishLogin(startClientLoginResult.ClientLoginState, startServerLoginResult!.LoginResponse, password, clientIdentifier + "abc", 
                null, out FinishClientLoginResult? finishClientLoginResult);

            Assert.IsNull(finishClientLoginResult);
        }

        [TestMethod]
        public void IncorrectServerIdentifierFails()
        {
            string userIdentifier = "user123";
            string password = "hunter2";
            string serverIdentifier = "server-ident";

            SetupAndRegister(userIdentifier, password, null, serverIdentifier, out string serverSetup, out string registrationRecord, 
                out string exportKey, out string serverStaticPublicKey);

            IOpaqueServer server = OpaqueFactory.CreateServer();
            IOpaqueClient client = OpaqueFactory.CreateClient();

            if (!client.StartLogin(password, out StartClientLoginResult? startClientLoginResult))
            {
                throw new Exception();
            }

            if (!server.StartLogin(serverSetup, startClientLoginResult!.StartLoginRequest, userIdentifier, registrationRecord, null, 
                serverIdentifier + "-abc", out StartServerLoginResult? startServerLoginResult))
            {
                throw new Exception();
            }

            client.FinishLogin(startClientLoginResult.ClientLoginState, startServerLoginResult!.LoginResponse, password, null, serverIdentifier, out FinishClientLoginResult? result);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ClientMethodsThrowsOnInvalidParams()
        {
            IOpaqueClient client = OpaqueFactory.CreateClient();

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.StartRegistration("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("", "Value", "Value", null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("Value", "", "Value", null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishRegistration("Value", "Value", "", null, null, out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.StartLogin("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("", "Value", "Value", null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("Value", "", "Value", null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => client.FinishLogin("Value", "Value", "", null, null, out _));
        }

        [TestMethod]
        public void ServerMethodsThrowsOnInvalidParams()
        {
            IOpaqueServer server = OpaqueFactory.CreateServer();

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.GetPublicKey("", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("", "Value", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("Value", "", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.CreateRegistrationResponse("Value", "Value", "", out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("", "Value", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("Value", "", "Value", null, null, null, out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.StartLogin("Value", "Value", "", null, null, null, out _));

            Assert.ThrowsException<StringParamIsEmptyException>(() => server.FinishLogin("", "Value", out _));
            Assert.ThrowsException<StringParamIsEmptyException>(() => server.FinishLogin("Value", "", out _));
        }

        [TestMethod]
        public void TestWithClientValues()
        {
            string userIdentifier = "e7fa739c-0b1d-4086-840c-b3089d68e5c84";

            string serverSetup = "Gg_F8C3hRO7p78KCeCscPGmpGlwyelbDuwVc-kdfHQDsg7fRSIujJtOx3hwsGpKPmIQn6PdEuYwwNLZWOeuLNrjEODbsCEoKSmORiKYY0LRrSm8GqvzQWqYSR0uPyDMFm53bG2i5IrFh6s_1Zi3LsmGRLPxq46IxPBUPAEuVrgE";
            string clientRegistrationResult = "wKuqxiZ_DF5qVDc6HYOLM4skqbWH6h2WM7mspaPGV0k";

            IOpaqueServer server = OpaqueFactory.CreateServer();

            if (!server.CreateRegistrationResponse(serverSetup, userIdentifier, clientRegistrationResult,
                out string? serverRegistrationResponse))
            {
                throw new Exception();
            }

            Assert.IsNotNull(serverRegistrationResponse);
        }

        [TestMethod]
        public void GenerateServerPublicKeyWorks()
        {
            IOpaqueServer server = OpaqueFactory.CreateServer();
            if (!server.CreateSetup(out string? serverSetup))
            {
                throw new Exception();
            }

            Assert.IsNotNull(serverSetup);
        }
    }
}