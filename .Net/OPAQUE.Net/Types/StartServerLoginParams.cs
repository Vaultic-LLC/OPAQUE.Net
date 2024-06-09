namespace OPAQUE.Net.Types
{
    public class StartServerLoginParams
    {
        public string serverSetup { get; set; }
        public string startLoginRequest { get; set; }
        public string userIdentifier { get; set; }
        public string? registrationRecord { get; set; }
        public CustomIdentifiers? identifiers { get; set; }

        public StartServerLoginParams(string ss, string slr, string ui)
        {
            serverSetup = ss;
            startLoginRequest = slr;
            userIdentifier = ui;
        }

        public StartServerLoginParams(string ss, string slr, string ui, string rr, CustomIdentifiers? i) : this(ss, slr, ui)
        {
            registrationRecord = rr;
            identifiers = i;
        }
    }
}
