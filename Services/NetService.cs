using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace YouTubeChatBot.Services
{
    class NetService
    {
        public async Task<Response> Request(Uri url, RequestMethod method = RequestMethod.GET, IDictionary<string, string> headers = null, byte[] body = null)
        {

        }
        public class Response
        {
            public readonly byte[] Data;
            public readonly int StatusCode;
        }
        public enum RequestMethod
        {
            DELETE,
            GET,
            PATCH,
            POST,
            PUT
        }
    }
}
