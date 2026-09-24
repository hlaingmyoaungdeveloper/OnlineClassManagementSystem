using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.Enrollment;
using OnlineClassManagementSystem.Domain.models.Enrollment;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentService _service;

        public EnrollmentController(EnrollmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollmentsAsync()
        {
            var result = await _service.GetEnrollmentsAsync(new EnrollmentListRequestModel());
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollmentAsync([FromBody] EnrollmentCreateRequestModel model)
        {
            var result = await _service.CreateEnrollmentAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{EnrollmentId}")]
        public async Task<IActionResult> GetEnrollmentAsync([FromRoute] EnrollmentEditRequestModel model)
        {
            var result = await _service.GetEnrollmentAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEnrollmentAsync(int id, [FromBody] EnrollmentPatchRequestModel model)
        {
            var result = await _service.PatchEnrollmentAsync(id, model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateEnrollmentStatusAsync([FromBody] EnrollmentUpdateStatusResquestModel model)
        {
            var result = await _service.UpdateEnrollmentStatusAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
