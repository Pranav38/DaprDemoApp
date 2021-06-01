using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace DaprDemoApp.Data
{
    /// <summary>
    /// Service to handle user feedback
    /// </summary>
    public class FeedbackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public FeedbackService(IHttpClientFactory httpClientFactory, ILogger<FeedbackService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// SubmitFeedback - Accepts user feedback and publishes User
        //  message to DAPR Sidecar pub-sub endpoint
        /// </summary>
        /// <param name="userData"></param>
        /// <returns></returns>
        public async Task<bool> SubmitFeedback(UserFeedback userData)
        {
            var client = _httpClientFactory.CreateClient();

            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(userData));
            _logger.LogInformation(JsonSerializer.Serialize(userData));

            //DAPR PUB-SUB URL 
            //http://localhost:3500/v1.0/publish/<pub-sub-component-name>/<topic-name>
            try
            {
                 string daprPort= Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
                _logger.LogInformation("dapper port  " + daprPort);
                var responsePingService = await client.GetAsync("http://localhost:" + daprPort + "/v1.0/invoke/daprdemoapi/method/Feedback/Ping");
                //              ;
                 _logger.LogInformation(responsePingService.Content.ToString());
                //if (responsePingService.IsSuccessStatusCode)
                //{
                //    _logger.LogInformation(responsePingService.Content.ToString());
                //    return true;
                //}

                //var response = await client.PostAsync("http://localhost:" + daprPort + "/v1.0/publish/messagebus/ReceiveFeedback",
                //               httpContent);


                var response = await client.PostAsync("http://localhost:" + daprPort + "/v1.0/invoke/daprdemoapi/method/Feedback/ReceiveFeedback",
                               new StringContent(JsonSerializer.Serialize(userData.FirstName)));
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Posted message successfuly - {userData.FirstName}");
                    return true;
                }
            }
            catch (System.Exception e)
            {

                throw;
            }
            
           

            _logger.LogError("Something went wrong");
            return false;
        }
    }
}
