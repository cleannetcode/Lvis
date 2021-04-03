using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace YouTubeChatBot.Services
{
    class NetService
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

        public async Task<Response> Request(Uri url, RequestMethod method = RequestMethod.GET, IDictionary<string, string> headers = null, byte[] body = null)
        {
            string finalUri;

            switch (method)
            {
                case RequestMethod.DELETE:
                    {
                        if (headers.Count > 0 && headers != null)
                        {
                            var builder = new StringBuilder($"{url}?");

                            foreach (var header in headers)
                            {
                                builder.Append($"{header.Key}={header.Value}&"); // Query string
                            }

                            builder.Remove(builder.Length - 1, 1); // Remove last &

                            finalUri = builder.ToString();
                        }
                        else
                        {
                            finalUri = url.ToString();
                        }

                        var res = await client.DeleteAsync(finalUri);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new Response(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.GET:
                    {
                        if (headers.Count > 0 && headers != null)
                        {
                            var builder = new StringBuilder($"{url}?");

                            foreach (var header in headers)
                            {
                                builder.Append($"{header.Key}={header.Value}&"); // Query string
                            }

                            builder.Remove(builder.Length - 1, 1); // Remove last &

                            finalUri = builder.ToString();
                        }
                        else
                        {
                            finalUri = url.ToString();
                        }

                        var res = await client.GetAsync(finalUri);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new Response(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.PATCH:
                    {
                        if (headers.Count > 0 && headers != null)
                        {
                            var builder = new StringBuilder($"{url}?");

                            foreach (var header in headers)
                            {
                                builder.Append($"{header.Key}={header.Value}&"); // Query string
                            }

                            builder.Remove(builder.Length - 1, 1); // Remove last &

                            finalUri = builder.ToString();
                        }
                        else
                        {
                            finalUri = url.ToString();
                        }

                        var res = await client.PatchAsync(finalUri,);

                        var bytes = await res.Content.ReadAsByteArrayAsync();

                        return new Response(bytes, (int)res.StatusCode);
                    }
                case RequestMethod.POST:

                    break;
                case RequestMethod.PUT:

                    break;
                default:
                    //throw new HttpRequestException("Invalid Request");
                    break;
            }
        }
    }
}
