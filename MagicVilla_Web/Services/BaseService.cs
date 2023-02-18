using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse responseModel { get; set; }
        public IHttpClientFactory httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.responseModel = new();
            this.httpClient = httpClient;
        }

        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                //message.Headers.Add("Content-Type", "application/json");
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }
                switch (apiRequest.ApiType)
                {
                    case MagicVilla_Utility.SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case MagicVilla_Utility.SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case MagicVilla_Utility.SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }
                HttpResponseMessage apiResponse = null;

                if(!string.IsNullOrEmpty(apiRequest.Token)) {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
                }

                apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                try
                {
                    APIResponse ApiResponseObj = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(apiResponse.StatusCode == HttpStatusCode.BadRequest || apiResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        ApiResponseObj.StatusCode= HttpStatusCode.BadRequest;
                        ApiResponseObj.IsSuccess = false;

                        var res = JsonConvert.SerializeObject(ApiResponseObj);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch (Exception e)
                {

                    var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionResponse;
                }
                var APIResponseObj = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponseObj;

            }
            catch (Exception ex) 
            {
                var dto = new APIResponse()
                {
                    ErrorMessages = new List<string>() { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
                
            }
        }
    }
}
