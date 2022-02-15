using System;
using Newtonsoft.Json;

namespace Loup.DotNet.Challenge.TestFramework
{
    public class UserContext
    {
        public UserContext()
        { }

        public UserContext(string id,
                            bool isSubscribed,
                            bool isAuthenticated,
                            string firstName,
                            string lastName,
                            DateTimeOffset created)
        {
            Id = id;
            IsSubscribed = isSubscribed;
            IsAuthenticated = isAuthenticated;
            FirstName = firstName;
            LastName = lastName;
            Created = created;
        }

        [JsonProperty("isAuthenticated")]
        public bool IsAuthenticated { get; internal set; }

        [JsonProperty("id")]
        public string Id { get; internal set; }

        [JsonProperty("username")]
        public string Username { get; internal set; }

        [JsonProperty("isSubscribed")]
        public bool IsSubscribed { get; internal set; }

        [JsonProperty("firstName")]
        public string FirstName { get; internal set; }

        [JsonProperty("lastName")]
        public string LastName { get; internal set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; internal set; }
    }
}
