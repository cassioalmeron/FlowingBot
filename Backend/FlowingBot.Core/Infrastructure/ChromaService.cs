namespace FlowingBot.Core.Infrastructure
{
    public record ChromaQueryResult
    {
        public string Text { get; set; }
        public float Distance { get; set; }
    }

    /// <summary>
    /// Service for interacting with ChromaDB using Python scripts
    /// </summary>
    public class ChromaService
    {
        /// <summary>
        /// Queries the ChromaDB collection for similar texts
        /// </summary>
        /// <param name="queryText">The query text</param>
        /// <returns>Query results containing documents, IDs and distances</returns>
        public static async Task<ChromaQueryResult[]> QueryAsync(string queryText)
        {
            var runner = new PythonRunner(@"C:\Temp\Python\chroma_query.py");
            return await runner.Run<ChromaQueryResult[]>(queryText);
        }
    }
}