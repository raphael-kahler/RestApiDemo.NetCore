namespace RestApiDemo.Api.DTO.Response
{
    /// <summary>
    /// A link to another entry.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// The name of the entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The link to the entry.
        /// </summary>
        public string Href { get; set; }
    }
}
