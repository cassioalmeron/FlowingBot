using FlowingBot.Core.Services;

namespace FlowingBot.Tests.Mocks
{
    internal class MockVectorDatabaseService : IVectorDatabaseService
    {
        public Task<string[]> GetCollectionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task CreateCollection(string collectionName)
        {
            throw new NotImplementedException();
        }

        public Task<float[]> GenerateEmbeddingAsync(string chunk)
        {
            throw new NotImplementedException();
        }

        public Task Upsert(string collectionName, FileContent file, string fileChunk, float[] chunkArray)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> QueryAsync(string collectionName, string queryText) =>
            Array.Empty<string>();
    }
}
