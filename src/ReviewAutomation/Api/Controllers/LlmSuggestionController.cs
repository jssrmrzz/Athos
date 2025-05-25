using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Athos.ReviewAutomation.Api.Controllers
{
    [ApiController]
    [Route("api/mock/llm")]
    public class LlmSuggestionController : ControllerBase
    {
        private readonly ILlmClient _llmClient;

        public LlmSuggestionController(ILlmClient llmClient)
        {
            _llmClient = llmClient;
        }

        public class SuggestionRequest
        {
            public string ReviewId { get; set; }
            public string Author { get; set; }
            public string Comment { get; set; }
        }

        [HttpPost("suggest")]
        public async Task<IActionResult> Suggest([FromBody] SuggestionRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Comment))
                return BadRequest("Comment is required");

            var suggestion = await _llmClient.GenerateResponseAsync(request.Comment); // ✅ just the comment

            return Ok(new { suggestion });
        }
    }
}