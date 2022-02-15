using Newtonsoft.Json;

namespace Loup.DotNet.Challenge.FunctionApp.Models
{
    public class Recipe
    {
        [JsonProperty("contentId")]
        public int ContentId { get; set; }

        [JsonProperty("contentType")]
        public int ContentType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("urlPartial")]
        public string UrlPartial { get; set; }

        [JsonProperty("servingSize")]
        public int ServingSize{ get; set; }

        [JsonProperty("energy")]
        public double Energy { get; set; }

        [JsonProperty("calories")]
        public double Calories { get; set; }

        [JsonProperty("Carbs")]
        public double Carbs { get; set; }

        [JsonProperty("Protein")]
        public double Protein { get; set; }

        [JsonProperty("DietryFibre")]
        public double DietryFibre { get; set; }

        [JsonProperty("Fat")]
        public double Fat { get; set; }

        [JsonProperty("SatFat")]
        public double SatFat { get; set; }

        [JsonProperty("Sugar")]
        public double Sugar { get; set; }
    }
}
