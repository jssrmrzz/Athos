using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LlmTestController : ControllerBase
    {
        private readonly ILlmClient _llmClient;

        public LlmTestController(ILlmClient llmClient)
        {
            _llmClient = llmClient;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateResponse([FromBody] string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return BadRequest("Review text cannot be empty.");

            var reply = await _llmClient.GenerateResponseAsync(reviewText);
            return Ok(new { response = reply });
        }
    }
}