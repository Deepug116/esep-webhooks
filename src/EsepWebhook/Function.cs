using System;
using System.Net.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EsepWebhook
{
    public class Function
    {
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            dynamic json = JsonConvert.DeserializeObject<dynamic>(request.Body);
            string payload = $"{{\"text\":\"Issue Created: {json.issue.html_url}\"}}";


            var client = new HttpClient();
            var slackRequest = new HttpRequestMessage(HttpMethod.Post, Environment.GetEnvironmentVariable("SLACK_URL"))
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            var response = client.Send(slackRequest);
            using var reader = new StreamReader(response.Content.ReadAsStream());
            var result = reader.ReadToEnd();

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = result
            };
        }
    }
}
