using Newtonsoft.Json;
using System;
using System.Net;

namespace Loup.DotNet.Challenge.TestFramework
{
    public class ErrorResult
    {
        [JsonProperty("code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
