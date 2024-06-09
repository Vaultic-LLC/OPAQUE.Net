namespace OPAQUE.Net.Types
{
    public class CreateServerRegistrationResponseParams
    {
        public string serverSetup { get; set; }
        public string userIdentifier { get; set; }
        public string registrationRequest { get; set; }

        public CreateServerRegistrationResponseParams(string ss, string ui, string rr)
        {
            serverSetup = ss;
            userIdentifier = ui;
            registrationRequest = rr;
        }
    }
}
