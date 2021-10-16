using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;

namespace GmailWittMVC.Utilities
{
    public class dsAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
    {
        public dsAuthorizationCodeFlow(Initializer initializer)
            : base(initializer)
        {
        }

        public override AuthorizationCodeRequestUrl
            CreateAuthorizationCodeRequest(string redirectUri)
        {
            return base.CreateAuthorizationCodeRequest(dsAuthorizationBroker.RedirectUri);
        }
    }
}