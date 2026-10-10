using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.User;
using OnlineClassManagementSystem.Shared.models.User;

namespace OnlineClassManagementSystem.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _service;

    public UserController(UserService service)
    {
        _service = service;
    }

    // GET api/user
    [HttpGet]
    public async Task<IActionResult> GetUsersAsync()
    {
        var result = await _service.GetUsersAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // GET api/user/{id}
    [HttpGet("{UserId}")]
    public async Task<IActionResult> GetUserAsync([FromRoute] UserEditRequestModel model)
    {
        var result = await _service.GetUserAsync(model);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // POST api/user
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateRequestModel model)
    {
        var result = await _service.CreateUserAsync(model);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // PATCH api/user/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchUserAsync(int id, [FromBody] UserPatchRequestModel model)
    {
        var result = await _service.PatchUserAsync(id, model);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    // DELETE api/user/{id}
    [HttpDelete("{UserId}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] UserDeleteRequestModel model)
    {
        var result = await _service.DeleteUserAsync(model);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
