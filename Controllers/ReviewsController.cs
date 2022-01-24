using Markov;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AutoReviewGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        public ReviewsController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult GetRandomReview()
        {
            try
            {
                var newReview = new Review();
                var random = new Random();
                var rating = random.Next(1, 6);

                newReview.Overall = rating;
                newReview.Reviewtext = string.Join(' ', _memoryCache.Get<MarkovChain<string>>($"Rating-{rating}").Chain(random));

                return Ok(newReview);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
