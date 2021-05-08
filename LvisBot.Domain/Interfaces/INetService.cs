using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LvisBot.Domain.Enums;
using LvisBot.Domain.Models;

namespace LvisBot.Domain.Interfaces
{
    public interface INetService
    {
        Task<NetResponse> Request(Uri url, RequestMethod method = RequestMethod.GET, IDictionary<string, string> headers = null, byte[] body = null);
    }
}