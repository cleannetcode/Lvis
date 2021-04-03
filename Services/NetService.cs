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
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    switch (header.Key.ToString())
                    {
                        case "DELETE":

                            break;
                        case "GET":
                            var res = await client.GetAsync(header.Key);
                            Response response = new Response(body, (int)res.StatusCode);
                            return response;
                        case "PATCH":

                            break;
                        case "POST":

                            break;
                        case "PUT":

                            break;
                        default:
                            //throw new HttpRequestException("Invalid Request");
                            break;
                    }
                }
            }
        }


    }
}
