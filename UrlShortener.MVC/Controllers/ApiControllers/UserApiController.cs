using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UrlShortener.DataAccess;

namespace UrlShortener.MVC.Controllers.ApiControllers;

[Authorize]
[ApiController, Route("api/[controller]")]
public class UserApiController : ControllerBase
{
    public UserApiController(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateMyself(UserModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        _mapper.Map(user, model);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [Authorize(Roles = Constants.AdminRoleName)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateById(Guid id, UserModel model)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        _mapper.Map(user, model);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }


    [Authorize(Roles = Constants.AdminRoleName)]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchMyself(Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<User> patchDoc)
    {
        var user = await _userManager.GetUserAsync(User);

        patchDoc.ApplyTo(user, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [Authorize(Roles = Constants.AdminRoleName)]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchById(Guid id, Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<User> patchDoc)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        patchDoc.ApplyTo(user, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMyself([FromServices] SignInManager<User> signInManager)
    {
        var user = await _userManager.GetUserAsync(User);

        await _userManager.DeleteAsync(user);
        await signInManager.SignOutAsync();

        return NoContent();
    }

    [Authorize(Roles = Constants.AdminRoleName)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound();

        await _userManager.DeleteAsync(user);

        return NoContent();
    }

    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
}