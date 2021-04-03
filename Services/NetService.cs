using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using YouTubeChatBot.Models;

namespace YouTubeChatBot.Services
{
    partial class NetService
    {
        private readonly HttpClient client;

        public enum RequestMethod
        {
            DELETE,
            GET,
            PATCH,
            POST,
            PUT,
        }

        public async Task<NetResponse> Request(Uri url, RequestMethod method = RequestMethod.GET, IDictionary<string, string> headers = null, byte[] body = null)
        {

            client.DefaultRequestHeaders.Clear();

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            switch (method)
            {
                case RequestMethod.DELETE:
                    {
                        var res = await client.DeleteAsync(url);
                        
                        var bytes = await res.Content.ReadAsByteArrayAsync();
                        
                        return new NetResponse(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.GET:
                    {
                        var res = await client.GetAsync(url);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new NetResponse(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.PATCH:
                    {
                        ByteArrayContent byteContent = new ByteArrayContent(body);
                        
                        var res = await client.PatchAsync(url, byteContent);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new NetResponse(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.POST:
                    {
                        ByteArrayContent byteContent = new ByteArrayContent(body);

                        var res = await client.PostAsync(url, byteContent);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new NetResponse(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.PUT:
                    {
                        ByteArrayContent byteContent = new ByteArrayContent(body);

                        var res = await client.PutAsync(url, byteContent);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new NetResponse(bytes, (int)res.StatusCode);
                    }
                default:
                    throw new HttpRequestException($"{method} - Invalid Request");
            }
        }
    }
}
