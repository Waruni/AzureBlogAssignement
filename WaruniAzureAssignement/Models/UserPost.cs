using System.Collections.Generic;
using Newtonsoft.Json;

namespace WaruniAzureAssignement.Models
{
    public class UserPost
    {
        [JsonProperty(PropertyName = "id")]
        public string PostId { get; set; }

        [JsonProperty(PropertyName = "heading")]
        public string Heading { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "createdOn")]
        public string CreatedOn { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public List<PostComment> Comments { get; set; }
    }
}