using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LvisBot.Domain.Enums;
using LvisBot.Domain.Interfaces;
using LvisBot.Domain.Models;

namespace LvisBot.BusinessLogic.Services
{
    public class NetService : INetService
    {
        public async Task<NetResponse> Request(Uri url, RequestMethod method = RequestMethod.GET, IDictionary<string, string> headers = null, byte[] body = null)
        {
            using (var client = new HttpClient())
            {
                if (headers != null && headers.Count > 0)
                {
                    foreach (var header in headers)
                    {
                        switch (header.Key.ToLower())
                        {
                            case "accept": client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(header.Value)); break;
                            case "host": client.DefaultRequestHeaders.Host = header.Value; break;
                            case "authorization": client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(header.Value); break;
                            case "useragent": client.DefaultRequestHeaders.UserAgent.ParseAdd(header.Value); break;
                            default: client.DefaultRequestHeaders.Add(header.Key, header.Value); break;
                        }
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
}
