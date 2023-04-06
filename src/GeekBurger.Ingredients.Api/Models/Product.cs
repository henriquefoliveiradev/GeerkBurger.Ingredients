using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeekBurger.Ingredients.Api.Models
{
    public class Product
    {
        [BsonId]
        [BsonElement("_id")]
        public Guid ProductId { get; set; }

        [BsonElement("items")]
        public List<Item> Items { get; set; }

        public void ChangeIngredients(string itemName, IEnumerable<string> ingredients)
        {
            var item = Items.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower());
            item.Ingredients = ingredients;
        }

        public void RemoveItem(Guid itemId)
        {
            var item = Items.FirstOrDefault(i => i.Id == itemId);

            Items.Remove(item);
        }

        public void AddItem(Guid itemId, string itemName)
        {
            Items.Add(new Item()
            {
                Id = itemId,
                Name = itemName
            });
        }
    }
}
