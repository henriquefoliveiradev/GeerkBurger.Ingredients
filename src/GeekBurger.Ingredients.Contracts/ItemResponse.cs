using System;
using System.Collections.Generic;

namespace GeekBurger.Ingredients.Contracts
{
    /// <summary>
    /// Exposes item information and its of ingredients
    /// </summary>
    public class ItemResponse
    {
        /// <summary>
        /// Item Identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List of Ingredients
        /// </summary>
        public IEnumerable<string> Ingredients { get; set; }
    }
}
