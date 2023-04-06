using System.Collections.Generic;

namespace GeekBurger.Ingredients.Api.Models
{
    public class Label
    {
        public string ItemName { get; set; }

        public IEnumerable<string> Ingredients { get; set; }
    }
}
