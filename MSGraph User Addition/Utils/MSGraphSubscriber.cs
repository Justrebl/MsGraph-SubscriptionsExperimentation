using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace Twolefthandev.MSGraph
{
    public class MSGraphSubscriber
    {

        private readonly MyConfig config;

        public MSGraphSubscriber(MyConfig config)
        {
            this.config = config;
        }

        public MSGraphSubscriber(string appId, string appSecret, string tenantId, string notificationUrl, string secretState)
        {
            this.config = new MyConfig(){
                AppId =  appId,
                AppSecret = appSecret,
                TenantId = tenantId,
                NotificationUrl = notificationUrl,
                SecretState = secretState
            };
        }

        [HttpGet]
        public async Task<ActionResult<Microsoft.Graph.Subscription>> SubscribeToNotification(int daysToExpire = 3, string notificationType, string resource)
        {
            var graphServiceClient = GetGraphClient();

            var sub = new Microsoft.Graph.Subscription();
            sub.ChangeType = notificationType;
            sub.NotificationUrl = this.config.NotificationUrl;
            sub.Resource = resource;
            sub.ExpirationDateTime = DateTime.UtcNow.AddDays(daysToExpire);
            sub.ClientState = this.config.SecretState;

            return await graphServiceClient
              .Subscriptions
              .Request()
              .AddAsync(sub);

            //   return $"Subscribed. Id: {newSubscription.Id}, Expiration: {newSubscription.ExpirationDateTime}";
        }

        private GraphServiceClient GetGraphClient()
        {
            var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((requestMessage) =>
            {
                // get an access token for Graph
                var accessToken = GetAccessToken().Result;

                requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                return Task.FromResult(0);
            }));

            return graphClient;
        }

        private async Task<string> GetAccessToken()
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(config.AppId)
              .WithClientSecret(config.AppSecret)
              .WithAuthority($"https://login.microsoftonline.com/{config.TenantId}")
              .WithRedirectUri("https://daemon")
              .Build();

            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();

            return result.AccessToken;
        }
    }
}