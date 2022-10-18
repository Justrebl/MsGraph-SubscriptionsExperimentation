using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twolefthandev.MSGraph;

namespace MSGraph_User_Addition
{
    public class TimerSubscriptionRefresher
    {
        [FunctionName("TimerSubscriptionRefresher")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            string AppId = Environment.GetEnvironmentVariable("GRAPH_AppId");
            string AppSecret = Environment.GetEnvironmentVariable("GRAPH_AppSecret");
            string TenantId = Environment.GetEnvironmentVariable("GRAPH_TenantID");
            string NotificationUrl = Environment.GetEnvironmentVariable("GRAPH_GuestUserAdditionSubscriberUrl");
            string SecrectState = Environment.GetEnvironmentVariable("GRAPH_SecretClientState");

            MSGraphSubscriber subscriber = new MSGraphSubscriber(AppId, AppSecret, TenantId, NotificationUrl, SecrectState);
    }
}
