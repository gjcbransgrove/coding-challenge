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
            return new EmptyResult();
        }
    }
}