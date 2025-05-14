using Athos.ReviewAutomation.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly ILlmClient _llm;

    public HealthController(ILlmClient llm)
    {
        _llm = llm;
    }

    [HttpGet("llm")]
    public async Task<IActionResult> CheckLlm()
    {
        try
        {
            var reply = await _llm.GenerateResponseAsync("Ping");
            return Ok(new { status = "Healthy", reply });
        }
        catch
        {
            return StatusCode(503, new { status = "Unhealthy", message = "LLM unavailable" });
        }
    }
}