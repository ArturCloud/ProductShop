using System.Configuration;

namespace ProjectX_Utility
{
    public class MailJetSettings  // create a class to get keys from configuration 
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}
