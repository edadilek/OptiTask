using DataAccessLayer.Interface;
using DataAccessLayer.Repository;
using Microsoft.AspNetCore.Mvc;
using OptiTask.Services;

namespace OptiTask.Controllers
{
    //workload için endpoint işlemleri

    [ApiController]
    [Route("api/[controller]")]
    public class WorkloadController : ControllerBase
    {
        private readonly IWorkloadRepository workloadRepository;

        public WorkloadController(IWorkloadRepository workload)
        {
            workloadRepository = workload;
        }

        //kullanıcının workloadını getirme
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWorkload(int userId)
        {
            var workload = await workloadRepository.GetByUserId(userId);

            if (workload == null)
            {
                return NotFound();
            }

            return Ok(workload);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllWorkloads()
        {
            var workloads = await workloadRepository.GetAll();

            if (workloads == null)
            {
                return NotFound("No workloads found.");
            }

            return Ok(workloads);
        }
    }
}

