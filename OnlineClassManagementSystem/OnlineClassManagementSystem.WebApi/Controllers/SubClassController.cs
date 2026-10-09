using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.SubClass;
using OnlineClassManagementSystem.Domain.models.SubClass;
namespace OnlineClassManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubClassController : ControllerBase 
    {
        private readonly SubClassService _service;

        public SubClassController(SubClassService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubClassesAsync()
        {
            var result = await _service.GetSubClassesAsync(new SubClassListRequestModel());
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubClassAsync([FromBody] SubClassCreateRequestModel model)
        {
            var result = await _service.CreateSubClassAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{SubClassId}")]
        public async Task<IActionResult> GetSubClassAsync([FromRoute] SubClassEditRequestModel model)
        {
            var result = await _service.GetSubClassAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchSubClassAsync(int id, [FromBody] SubClassPatchRequestModel model)
        {
            var result = await _service.PatchSubClassAsync(id, model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{SubClassId}")]
        public async Task<IActionResult> DeleteSubClassAsync([FromRoute] SubClassDeleteRequestModel model)
        {
            var result = await _service.DeleteSubClassAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}