using Microsoft.AspNetCore.Mvc;
using FlowingBot.Api.Filters;

namespace FlowingBot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LoggingActionFilter))]
    public class StreamingController : ControllerBase
    {
        private string _content =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis sed lorem sed mauris iaculis rutrum vel sed elit. Mauris condimentum mollis dignissim. Phasellus sollicitudin euismod ante, a viverra nibh. Nulla elit nunc, luctus eleifend tristique at, sodales ac velit. Morbi risus libero, imperdiet et lacus non, fringilla cursus felis. Ut tristique, metus vel dignissim vestibulum, magna nulla dapibus quam, rhoncus aliquam purus nulla at augue. Curabitur scelerisque fringilla nisl vitae molestie. Aenean in maximus metus. Donec luctus rhoncus ex, eget fermentum nisi venenatis vitae.\n\nEtiam aliquam pellentesque orci ut lacinia. Vestibulum malesuada quam velit, vitae tempus orci condimentum ut. Curabitur elementum erat nulla, facilisis finibus enim commodo vel. Mauris in luctus augue. Ut at nunc eu mi elementum sodales. Nulla eleifend facilisis nisl. Pellentesque quis elementum justo, ac egestas eros. In commodo purus non facilisis dictum. Etiam venenatis tempus efficitur.\n\nNunc at libero imperdiet, convallis tortor sed, lacinia ante. Phasellus varius aliquam ante id rutrum. Curabitur lobortis justo eu malesuada posuere. Ut at aliquam mi. Mauris ut aliquam libero. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Morbi vitae pharetra purus. Suspendisse ut viverra neque, sit amet sagittis ante. Proin elit urna, tempor sit amet lacus eget, congue dapibus mi. Mauris commodo odio ac efficitur molestie. Duis maximus augue id purus tempus, vel fringilla nibh vestibulum.\n\nMorbi blandit orci mollis, varius dui vitae, porttitor quam. Integer cursus enim quam, vel blandit enim blandit non. Cras feugiat vestibulum eros sed pulvinar. Suspendisse mi felis, finibus a malesuada non, gravida a arcu. Aenean non sem pulvinar, aliquam ex et, ultricies justo. Ut tristique, dolor non tincidunt pharetra, tellus ipsum feugiat mauris, et ultricies nisi mauris sit amet erat. Donec ultricies ornare sapien, hendrerit luctus nisi accumsan sed. Praesent ornare maximus sem vel ultrices. Curabitur leo risus, scelerisque ac magna ut, dapibus consequat sapien. Morbi velit ante, consectetur a sodales ut, eleifend in purus.\n\nNam condimentum, ex a eleifend cursus, lacus urna ultrices sem, a auctor metus ante suscipit justo. Nunc ac ipsum eu massa vestibulum ultricies vitae mollis eros. Pellentesque blandit fermentum risus vitae cursus. Praesent diam ipsum, placerat vel augue non, tristique porttitor nibh. Morbi sollicitudin nec arcu vitae gravida. In sit amet purus ligula. Aenean eget dictum metus. Pellentesque vehicula rhoncus mauris eget porttitor. Aenean blandit augue at turpis condimentum, sit amet fermentum neque luctus. Donec sit amet orci mauris. Quisque vel aliquet elit, sit amet scelerisque nibh. Mauris consequat sem eu eros rutrum, non cursus libero congue. Aenean mauris nulla, eleifend vitae accumsan a, lobortis quis diam. In auctor est et feugiat fringilla. Integer venenatis nunc faucibus ligula consequat, et suscipit turpis gravida. Vestibulum hendrerit sollicitudin libero sed semper.";

        [HttpPost]
        public async IAsyncEnumerable<string> GetStreamedData()
        {
            var words = _content.Split(' ');

            foreach (var word in words)
            {
                yield return $"{word} ";
                await Task.Delay(50); // Simulate processing time
            }
        }
    }
}