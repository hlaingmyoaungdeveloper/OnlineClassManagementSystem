using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.TeachingTracking;
using OnlineClassManagementSystem.Shared.models.TeachingTracking;

namespace OnlineClassManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachingTrackingController : ControllerBase
    {
        private readonly TeachingTrackingService _service;

        public TeachingTrackingController(TeachingTrackingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachingTrackingsAsync()
        {
            var result = await _service.GetTeachingTrackingsAsync(new TeachingTrackingListRequestModel());
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeachingTrackingAsync([FromBody] TeachingTrackingCreateRequestModel model)
        {
            var result = await _service.CreateTeachingTrackingAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{TrackId}")]
        public async Task<IActionResult> GetTeachingTrackingAsync([FromRoute] TeachingTrackingEditRequestModel model)
        {
            var result = await _service.GetTeachingTrackingAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTeachingTrackingAsync(int id, [FromBody] TeachingTrackingPatchRequestModel model)
        {
            var result = await _service.PatchTeachingTrackingAsync(id, model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{TrackId}")]
        public async Task<IActionResult> DeleteTeachingTrackingAsync([FromRoute] TeachingTrackingDeleteRequestModel model)
        {
            var result = await _service.DeleteTeachingTrackingAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
