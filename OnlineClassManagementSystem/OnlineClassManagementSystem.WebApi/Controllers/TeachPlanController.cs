using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.TeachPlan;
using OnlineClassManagementSystem.Domain.models.TeachPlan;

namespace OnlineClassManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachPlanController : ControllerBase
    {
        private readonly TeachPlanService _service;

        public TeachPlanController(TeachPlanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachPlansAsync()
        {
            var result = await _service.GetTeachPlansAsync(new TeachPlanListRequestModel());
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeachPlanAsync([FromBody] TeachPlanCreateRequestModel model)
        {
            var result = await _service.CreateTeachPlanAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeachPlanAsync([FromRoute] TeachPlanEditRequestModel model)
        {
            var result = await _service.GetTeachPlanAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTeachPlanAsync(int id, [FromBody] TeachPlanPatchRequestModel model)
        {
            var result = await _service.PatchTeachPlanAsync(id, model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeachPlanAsync([FromRoute] TeachPlanDeleteRequestModel model)
        {
            var result = await _service.DeleteTeachPlanAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
