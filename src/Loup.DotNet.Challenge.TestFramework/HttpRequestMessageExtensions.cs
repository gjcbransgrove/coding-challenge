using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace Loup.DotNet.Challenge.TestFramework
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage SetupUserContext(this HttpRequestMessage requestMessage,
                                                          string id = null,
                                                          bool isAuthenticated = false,
                                                          bool isSubscribed = false,
                                                          string firstName = "Test",
                                                          string lastName = "User")
        {
            var userContext = new UserContext(id: id,
                                            isAuthenticated: isAuthenticated,
                                            isSubscribed: isSubscribed,
                                            firstName: firstName,
                                            lastName: lastName,
                                            created: DateTime.UtcNow);


            var json = JsonConvert.SerializeObject(userContext);

            var plainTextBytes = Encoding.UTF8.GetBytes(json);
            var encodedHeader = Convert.ToBase64String(plainTextBytes);

            requestMessage.Headers.Add("X-ClaimsContext", encodedHeader);

            return requestMessage;
        }

        public static void Validate(this UserContext user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!user.IsAuthenticated)
                throw new ArgumentException("isAuthenticated", "User must be authenticated.");
        }
    }
}
