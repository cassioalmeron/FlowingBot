using System.Text;
using FlowingBot.Core.Infrastructure;
using UglyToad.PdfPig;

namespace FlowingBot.Core.Services
{
    public interface IVectorDatabaseService
    {
        Task<string[]> GetCollectionsAsync();
        Task CreateCollection(string collectionName);
        Task<float[]> GenerateEmbeddingAsync(string chunk);
        Task Upsert(string collectionName, FileContent file, string fileChunk, float[] chunkArray);
        Task<string[]> QueryAsync(string collectionName, string queryText);
    }

    public class VectorDatabaseService
    {
        public VectorDatabaseService(IVectorDatabaseService service) =>
            _service = service;

        private readonly IVectorDatabaseService _service;

        public async Task<string[]> GetCollectionsAsync()
        {
            var collections = await _service.GetCollectionsAsync();
            return collections;
        }

        public async Task<string[]> QueryAsync(string collection, string queryText)
        {
            var res = await _service.QueryAsync(collection, queryText);
            return res;
        }

        public async Task CreateCollection()
        {
            await _service.CreateCollection("");
        }

        public async Task GenerateEmbeddings(string collection, FileContent file) => 
            await GenerateEmbeddings(collection, new[] { file });

        public async Task GenerateEmbeddings(string collection, FileContent[] files)
        {
            await CreateCollection();

            foreach (var file in files)
            {
                foreach (var fileChunk in file.Chunks)
                {
                    var embedding = await _service.GenerateEmbeddingAsync(fileChunk);
                    var embeddingArray = embedding.ToArray();

                    await _service.Upsert("", file, fileChunk, embeddingArray);
                }

                Console.WriteLine($"Processed file: {file.FileName}");
            }
        }
    }

    public class FileContent
    {
        public FileContent(string fileName, Stream fileStream)
        {
            FileName = fileName;
            FileStream = fileStream;

            GetChunks();
        }

        public string FileName { get; }
        public Stream FileStream { get; }
        public List<string> Chunks { get; } = new ();

        private void GetChunks()
        {
            var extension = Path.GetExtension(FileName);
            switch (extension)
            {
                case ".md":
                {
                    using var reader = new StreamReader(FileStream);
                    var content = reader.ReadToEnd();
                    Chunks.Add(content);
                    break;
                }
                case ".pdf":
                {
                    var text = ExtractTextFromPdf(FileStream);
                    var splitter = new RecursiveCharacterTextSplitter(
                        separators: new List<string> { "\n\n", ". ", "! ", "? ", "\n", " ", "" }
                    );

                    var chunks = splitter
                        .SplitText(text)
                        .ToArray();

                    Chunks.AddRange(chunks);
                    break;
                }
                default:
                    throw new Exception("Invalid format!");
            }
        }

        private static string ExtractTextFromPdf(Stream stream)
        {
            var text = new StringBuilder();
            using (var document = PdfDocument.Open(stream))
                foreach (var page in document.GetPages())
                    text.AppendLine(page.Text);

            return text.ToString();
        }

        public static FileContent FromPath(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileContent(fileName, fileStream);
        }
    }
}