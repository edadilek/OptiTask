using Microsoft.AspNetCore.Mvc;
using OptiTask.Services;

namespace OptiTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkloadController : ControllerBase
    {
        public WorkloadController()
        {

        }

        // Kullanıcı iş yüklerini hesaplama
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWorkload(int userId)
        {
            return Ok();
        }
    }
}

