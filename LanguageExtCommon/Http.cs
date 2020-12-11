using System.Net.Http;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExtCommon
{
    public static class Http
    {
        public static HttpClient GetHttpClient(Uri serviceEndpoint)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.BaseAddress = serviceEndpoint;
            client.Timeout.Add(new TimeSpan(0, 1, 0));
            return client;
        }

        public static async Task<Either<Error, HttpResponse>> HttpGet(HttpClient client, string requestUri)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(requestUri);
            }
            catch (Exception e)
            {
                return Left<Error, HttpResponse>(Error.New($"HttpGet GetAsync failed. Message: {e.Message}", e));
            }
            return await ProcessHttpResponse(response);
        }

        public static async Task<Either<Error, HttpResponse>> HttpPost(HttpClient client, string requestUri, HttpContent body)
        {
            HttpResponseMessage response = null;
            try
            {
                response = await client.PostAsync(requestUri, body);
            }
            catch (Exception e)
            {
                return Left<Error, HttpResponse>(Error.New($"HttpPost PostAsync failed. Message: {e.Message}", e));
            }
            return await ProcessHttpResponse(response);
        }

        private static async Task<Either<Error, HttpResponse>> ProcessHttpResponse(HttpResponseMessage response)
        {
            Stream stream = null;
            try
            {
                stream = await response.Content.ReadAsStreamAsync();
                var body = await new StreamReader(stream).ReadToEndAsync();
                return Right<Error, HttpResponse>(new HttpResponse()
                {
                    StatusCode = (int)response.StatusCode,
                    Headers = response.Headers.ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value.Freeze()
                        ).ToMap(),
                    Body = body
                });
            }
            catch (Exception e)
            {
                return Left<Error, HttpResponse>(Error.New($"ProcessHttpResponse ReadAsStreamAsync failed. Message: {e.Message}", e));
            }
        }

        public static HttpContent CreateJsonRequest<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return body;
        }
    }

    public struct HttpResponse
    {
        public int StatusCode;
        public Map<string, Lst<string>> Headers;
        public string Body;
    }

}
