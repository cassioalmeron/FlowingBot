using FlowingBot.Core.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace FlowingBot.Core.Infrastructure
{
    public class QdrantService : IVectorDatabaseService
    {
        public QdrantService()
        {
            _qDrantClient = new QdrantClient("localhost");

            _kernel = Kernel.CreateBuilder()
                .AddOllamaTextEmbeddingGeneration(
                    modelId: "nomic-embed-text",
                    endpoint: new Uri("http://localhost:11434"))
                .Build();
        }

        private readonly QdrantClient _qDrantClient;
        private readonly Kernel _kernel;

        public async Task<string[]> GetCollectionsAsync()
        {
            var collections = await _qDrantClient.ListCollectionsAsync();
            return collections.ToArray();
        }

        public async Task CreateCollection(string collectionName)
        {
            await _qDrantClient.CreateCollectionAsync(
                collectionName: collectionName,
                vectorsConfig: new VectorParams { Size = 768, Distance = Distance.Cosine });
        }

        public async Task<float[]> GenerateEmbeddingAsync(string chunk)
        {
            var embeddingGenerator = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
            var embedding = await embeddingGenerator.GenerateEmbeddingAsync(chunk);
            return embedding.ToArray();
        }

        public async Task Upsert(string collectionName, Services.FileContent file, string fileChunk, float[] embeddingArray)
        {
            await _qDrantClient.UpsertAsync(
                collectionName: collectionName,
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

        public async Task<string[]> QueryAsync(string collectionName, string queryText)
        {
            var embeddingGenerator = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
            var queryEmbedding = (await embeddingGenerator.GenerateEmbeddingsAsync(new[] { queryText }))[0];

            var searchParams = new SearchParams
            {
                HnswEf = 128,
                Exact = false
            };

            var result = await _qDrantClient.SearchAsync(
                collectionName: collectionName,
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
    }
}