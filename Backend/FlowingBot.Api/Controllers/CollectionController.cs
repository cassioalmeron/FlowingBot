using Microsoft.AspNetCore.Mvc;
using FlowingBot.Core.Infrastructure;
using System.Text;
using FlowingBot.Core.Services;

namespace FlowingBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionController : ControllerBase
    {
        public CollectionController(VectorDatabaseService vectorDatabaseService) =>
            _vectorDatabaseService = vectorDatabaseService;

        private readonly VectorDatabaseService _vectorDatabaseService;

        [HttpGet]
        public async Task<string[]> Get()
        {
            var collections = await _vectorDatabaseService.GetCollectionsAsync();
            return collections;
        }

        [HttpGet("Query")]
        public async Task<string[]> Query(string query, string collection)
        {
            var queryResult = await _vectorDatabaseService.QueryAsync(collection, query);
            return queryResult;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CollectionRequestDto request)
        {
            if (request.Files == null || !request.Files.Any())
                return BadRequest("No files were uploaded");

            var files = new List<FileContent>();
            var streams = new List<MemoryStream>();

            try
            {
                foreach (var file in request.Files)
                {
                    if (file.Length > 0)
                    {
                        string content;
                        using (var reader = new StreamReader(file.OpenReadStream()))
                            content = await reader.ReadToEndAsync();

                        var memoryStream = new MemoryStream();
                        streams.Add(memoryStream);

                        using (var writer = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
                            await writer.WriteAsync(content);
                        
                        memoryStream.Position = 0;

                        var fileContent = new FileContent(file.FileName, memoryStream);
                        files.Add(fileContent);
                    }
                }

                await _vectorDatabaseService.GenerateEmbeddings(request.Name, files.ToArray());

                return Ok();
            }
            finally
            {
                foreach (var stream in streams)
                    stream.Dispose();
            }
        }
    }

    public class CollectionRequestDto
    {
        public string Name { get; set; }
        public List<IFormFile> Files { get; set; }
    }
}