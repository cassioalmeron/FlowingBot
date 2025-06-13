using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Text;
using UglyToad.PdfPig;

namespace FlowingBot.Core.Infrastructure
{
    public class QdrantService
    {
        public QdrantService(string collectionName = "")
        {
            _collectionName = collectionName;
            _qDrantClient = new QdrantClient("localhost");

            _kernel = Kernel.CreateBuilder()
                .AddOllamaTextEmbeddingGeneration(
                    modelId: "nomic-embed-text",
                    endpoint: new Uri("http://localhost:11434"))
                .Build();
        }

        private readonly string _collectionName;
        private readonly QdrantClient _qDrantClient;
        private readonly Kernel _kernel;

        public async Task<string[]> GetCollectionsAsync()
        {
            var collections = await _qDrantClient.ListCollectionsAsync();
            return collections.ToArray();
        }

        public async Task<string[]> QueryAsync(string queryText)
        {
            var embeddingGenerator = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
            var queryEmbedding = (await embeddingGenerator.GenerateEmbeddingsAsync(new[] { queryText }))[0];

            var searchParams = new SearchParams
            {
                HnswEf = 128,
                Exact = false
            };

            var result = await _qDrantClient.SearchAsync(
                collectionName: _collectionName,
                vector: queryEmbedding.ToArray(),
                limit: 3,
                scoreThreshold: 0.0f,
                searchParams: searchParams);

            Console.WriteLine($"Found {result.Count} results");

            var res = new List<string>();

            foreach (var item in result)
                res.Add(item.Payload["content"].StringValue);

            return res.ToArray();
        }

        public async Task CreateCollection()
        {
            await _qDrantClient.CreateCollectionAsync(
                collectionName: _collectionName,
                vectorsConfig: new VectorParams { Size = 768, Distance = Distance.Cosine });
        }

        public async Task GenerateEmbeddings(FileContent file) => 
            await GenerateEmbeddings(new[] { file });

        public async Task GenerateEmbeddings(FileContent[] files)
        {
            await CreateCollection();

            var embeddingGenerator = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();

            foreach (var file in files)
            {
                foreach (var fileChunk in file.Chunks)
                {
                    var embedding = await embeddingGenerator.GenerateEmbeddingAsync(fileChunk);
                    var embeddingArray = embedding.ToArray();

                    await _qDrantClient.UpsertAsync(
                        collectionName: _collectionName,
                        points: new List<PointStruct>
                        {
                            new()
                            {
                                Id = (ulong)file.GetHashCode() + (ulong)file.Chunks.IndexOf(fileChunk), // Unique ID based on filename
                                Vectors = embeddingArray,
                                Payload =
                                {
                                    ["content"] = fileChunk,
                                    ["filename"] = file.FileName,
                                }
                            }
                        });

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