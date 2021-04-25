using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contract.Request;
using Contract.Response;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ChatClient
{
    public class AsyncApiService
    {
        private readonly AsyncApiOptions _apiOptions;

        
        public AsyncApiService(IOptionsSnapshot<AsyncApiOptions> apiOptions)
        {
            _apiOptions = apiOptions.Value;
        }
        
        
        public async Task<HttpStatusCode> Connect(string name)
        {
            var url = $"{_apiOptions.BaseUrl}/Chat/Connect";
            var data = new ConnectRequest
            {
                Name = name
            };

            return await Post(url, data);
        }

        public async Task<ClientsResponse> GetUsers()
        {
            var url = $"{_apiOptions.BaseUrl}/Chat/Clients";
            return await Get<ClientsResponse>(url);
        }

        public async Task<bool> SendMessage(string user, string text, string sender)
        {
            var url = $"{_apiOptions.BaseUrl}/Chat/SendMessage";
            var data = new MessageSendRequest
            {
                To = user,
                Text = text,
                Sender = sender
            };

            return await Post<MessageSendRequest, bool>(url, data);
        }

        private async Task<TResult> Post<TData, TResult>(string url, TData data)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResult>(body);
            }
        }

        private async Task<HttpStatusCode> Post<TData>(string url, TData data)
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(url, content);

                return response.StatusCode;
            }
        }

        private async Task<TResult> Get<TResult>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<TResult>(body);
            }
        }
    }
}