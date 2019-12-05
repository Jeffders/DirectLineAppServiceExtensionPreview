using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EchoBot.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EchoBot.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var secret = Configuration.GetSection("ClientDirectLineSecret")?.Value;
            var endpoint = Configuration.GetSection("ClientDirectLineEndpoint")?.Value;

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}/tokens/generate");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", secret);

            var response = await client.SendAsync(request);
            string token = String.Empty;
            DirectLineToken dlToken = null;

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                dlToken = JsonConvert.DeserializeObject<DirectLineToken>(body);
            }

            var config = new ChatConfig()
            {
                Token = dlToken?.Token,
                Domain = endpoint,
                UserId = Guid.NewGuid().ToString("D"),
                ConversationId = dlToken?.ConversationId
            };
            return View(config);
        }
    }

    public class DirectLineToken
    {
        [JsonProperty("conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }


    public class Config
    {
        public string Secret { get; set; }

        public string Endpoint { get; set; }

        public string Token { get; set; }

        public string Error { get; set; }
    }
}