using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ProjectX_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public MailJetSettings _mailjets { get; set; }
        public EmailSender(IConfiguration configuration)
        {
            _config = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailjets = _config.GetSection("MailJet").Get<MailJetSettings>();


            MailjetClient client = new MailjetClient(_mailjets.ApiKey,_mailjets.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
     new JObject {
      {
       "From",
       new JObject {
        {"Email", "lans.grimmo@gmail.com"},
        {"Name", "Art"}
       }
      }, {
       "To",
       new JArray {
        new JObject {
         {
          "Email",
          email
         }, {
          "Name",
          "ArtGopnik"
         }
        }
       }
      }, {
       "Subject",
       "Greetings from Mailjet."
      }, {
       "TextPart",
       "My first Mailjet email"
      }, {
       "HTMLPart",
       body
      }
     }
             });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
