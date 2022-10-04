using Loup.DotNet.Challenge.FunctionApp.Models;
using Loup.DotNet.Challenge.TestFramework;
using Moq;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Loup.DotNet.Challenge.Tests
{
    /// <summary>
    /// Add your tests to this class.
    /// </summary>
    [Collection(nameof(FunctionAppTestHost))]
    public class Tests
    {
        private readonly ITestOutputHelper _helper;
        private readonly FunctionAppTestHost _testHost;

        public Tests(ITestOutputHelper helper, FunctionAppTestHost testHost)
        {
            _helper = helper;
            _testHost = testHost;
        }

        /// <summary>
        /// Strips the sample data of new lines in the same way the data has been serialized to JSON on GetRecipeFunction
        /// </summary>
        /// <typeparam name="T">Expected result type</typeparam>
        /// <param name="testData">Sample data string</param>
        /// <returns></returns>
        private string GetStrippedTestDataJson<T>(string testData)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject<T>(testData));
        }

        [Fact]
        public async Task When_UserContext_Is_Authenticated_False_Expect_Unauthorized_Template()
        {
            // Arrange
            var expectedSampleData = GetStrippedTestDataJson<ErrorResult>(_testHost.Entities["5143_401"]);
            using (var client = _testHost.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, "http://test/api/recipes/5143").SetupUserContext(id: "1234", isAuthenticated: false, isSubscribed: false, firstName: "John", lastName: "Citizen"))
            // Act
            using (var response = await client.SendAsync(request))
            {
                // Assert
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                Assert.Equal(expectedSampleData, await response.Content.ReadAsStringAsync());
            }
        }

        [Fact]
        public async Task When_UserContext_Is_Authenticated_True_Expect_No_Unauthorized_Response()
        {
            using (var client = _testHost.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, "http://test/api/recipes/5143").SetupUserContext(id: "1234", isAuthenticated: true, isSubscribed: false, firstName: "John", lastName: "Citizen"))
            using (var response = await client.SendAsync(request))
            {
                Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task When_Content_Id_Invalid_Integer_Expect_Bad_Request_Template()
        {
            var expectedSampleData = GetStrippedTestDataJson<ErrorResult>(_testHost.Entities["5143_400"]);
            using (var client = _testHost.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, "http://test/api/recipes/-5143").SetupUserContext(id: "1234", isAuthenticated: true, isSubscribed: false, firstName: "John", lastName: "Citizen"))
            using (var response = await client.SendAsync(request))
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.Equal(expectedSampleData, await response.Content.ReadAsStringAsync());
            }
        }

        [Fact]
        public async Task When_Content_Id_In_Repository_Expect_Recipe_Template()
        {
            var mockRepository = Mock.Get(_testHost.Repository);
            mockRepository
                .Setup(x => x.Get<Recipe>(5143))
                .Returns(new Recipe()
                {
                    ContentId = 5143,
                    ContentType = 1,
                    Name = "Green Super Smoothie",
                    Summary = "This is a Super Smoothie, specifically created for men and women on the Build Muscle goal. Our post-workout Super Smoothies make it easier to meet your daily energy needs for training and muscle gain. To view the full selection or swap to a different flavor for your post-workout meal, search “super smoothie” in Meals.",
                    UrlPartial = "green-super-smoothie",
                    ServingSize = 0,
                    Energy = 1750.93,
                    Calories = 418.1929,
                    Carbs = 43.668,
                    Protein = 33.07,
                    DietryFibre = 9.9745,
                    Fat = 39.038,
                    SatFat = 2.372,
                    Sugar = 39.038
                });

            var expectedSampleData = GetStrippedTestDataJson<Recipe>(_testHost.Entities["5143_200"]);

            using (var client = _testHost.CreateClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, "http://test/api/recipes/5143").SetupUserContext(id: "1234", isAuthenticated: true, isSubscribed: false, firstName: "John", lastName: "Citizen"))
            using (var response = await client.SendAsync(request))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(expectedSampleData, await response.Content.ReadAsStringAsync());
            }
        }
    }
}