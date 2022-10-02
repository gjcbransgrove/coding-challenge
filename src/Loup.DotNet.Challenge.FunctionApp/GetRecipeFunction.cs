using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Loup.DotNet.Challenge.TestFramework;
using Loup.DotNet.Challenge.FunctionApp.Models;

namespace Loup.DotNet.Challenge.FunctionApp
{
    public class GetRecipeFunction
    {
        private readonly IRepository _repository;

        public GetRecipeFunction(IRepository repository)
        {
            _repository = repository;
        }

        [FunctionName("GetRecipe")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "recipes/{recipeId}")] HttpRequest req,
                                             int recipeId,
                                             UserContext user)
        {
            try
            {
                user.Validate();
            }
            catch (ArgumentException)
            {
                return new UnauthorizedObjectResult(BuildErrorResultBody(HttpStatusCode.Unauthorized, "User must be authenticated."));
            }

            if (recipeId <= 0)
            {
                return new BadRequestObjectResult(BuildErrorResultBody(HttpStatusCode.BadRequest, "Invalid contentId. Expected contentId greater than zero."));
            }

            var recipe = _repository.Get<Recipe>(recipeId);

            if (recipe != null)
            {
                return new OkObjectResult(JsonConvert.SerializeObject(recipe));
            }

            return new NotFoundObjectResult(BuildErrorResultBody(HttpStatusCode.NotFound, "Recipe not found."));
        }

        private string BuildErrorResultBody(HttpStatusCode statusCode, string message)
        {
            return JsonConvert.SerializeObject(new ErrorResult()
            {
                HttpStatusCode = statusCode,
                Message = message
            });
        }
    }
}