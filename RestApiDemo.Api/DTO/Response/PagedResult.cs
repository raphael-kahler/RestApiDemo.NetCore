using System.Collections.Generic;

namespace RestApiDemo.Api.DTO.Response
{
    /// <summary>
    /// A paged collection of items.
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// The number of items in the collection.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// How many items where skipped before the items in the collection.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The total number of items that exist.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The items in the collection.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
