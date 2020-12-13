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
    /// <summary>
    /// Provides wrappers around `System.Net.Http`
    /// </summary>
    public static class Http
    {
        /// <summary>
        /// Create a new HttpClient
        /// </summary>
        public static HttpClient GetHttpClient(Uri serviceEndpoint)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.BaseAddress = serviceEndpoint;
            client.Timeout.Add(new TimeSpan(0, 1, 0));
            return client;
        }

        /// <summary>
        /// Make GET request
        /// </summary>
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

        /// <summary>
        /// Make POST request
        /// </summary>
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

        /// <summary>
        /// Serialize object to JSON, to be passed into an HttpClient
        /// </summary>
        public static HttpContent CreateJsonRequest<T>(T obj) =>
            new StringContent(JsonConvert.SerializeObject(obj), System.Text.Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Object containing data returned from Http Response
    /// </summary>
    public struct HttpResponse
    {
        /// <summary>
        /// Status Code of the Http Response
        /// </summary>
        public int StatusCode;
        /// <summary>
        /// Headers from Http Response
        /// </summary>
        public Map<string, Lst<string>> Headers;
        /// <summary>
        /// Body from Http Response
        /// </summary>
        public string Body;
    }

}
