using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Graph;

namespace MSGraph_User_Addition
{
    public static class MSGraphUserAdded
    {
        [FunctionName("MSGraphUserAdded")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"request Body : {requestBody}");

            // handle notifications
            using (StreamReader reader = new StreamReader(req.Body))
            {
                string content = await reader.ReadToEndAsync();

                log.LogInformation($"Content is :{content}");
                var notifications = JsonSerializer.Deserialize<ChangeNotificationCollection>(content);

                if (notifications != null)
                {
                    foreach (var notification in notifications.Value)
                    {
                        log.LogInformation($"Received notification: '{notification.Resource}', {notification.ResourceData.AdditionalData["id"]}");
                    }
                }
            }
            return new OkObjectResult("All Good");
        }
    }
}
