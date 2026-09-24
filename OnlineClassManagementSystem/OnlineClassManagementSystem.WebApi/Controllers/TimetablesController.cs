using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.Timetable;
using OnlineClassManagementSystem.Domain.models.Timetable;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetablesController : ControllerBase
    {
        private readonly TimetableService _service;

        public TimetablesController(TimetableService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTimetablesAsync()
        {
            var result = await _service.GetTimetablesAsync(new TimetableListResquestModel());
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimetableAsync([FromBody] TimetableCreateRequestModel model)
        {
            var result = await _service.CreateTimetableAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("club/{SubClassId}")]
        public async Task<IActionResult> GetTimetablesByClassAsync([FromRoute] TimetableListByClassRequest model)
        {
            var result = await _service.GetTimetablesByClassAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("teacher/{TeacherId}")]
        public async Task<IActionResult> GetTimetablesByTeacherAsync([FromRoute] TimetableListByTeacherRequestModel model)
        {
            var result = await _service.GetTimetablesByTeacherAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTimetableAsync(int id, [FromBody] TimetablePatchRequestModel model)
        {
            var result = await _service.PatchTimetableAsync(id, model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{TimetableId}")]
        public async Task<IActionResult> DeleteTimetableAsync([FromRoute] TimetableDeleteRequestModel model)
        {
            var result = await _service.DeleteTimetableAsync(model);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
