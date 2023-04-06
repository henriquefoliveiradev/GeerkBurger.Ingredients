using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace GeekBurger.Ingredients.Api.Models
{
    public class Item
    {
        [BsonId]
        [BsonElement("_id")]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("ingredients")]
        public IEnumerable<string> Ingredients { get; set; }
    }
}
