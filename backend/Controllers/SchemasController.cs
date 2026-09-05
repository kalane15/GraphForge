using GraphForge.Api.DTOs;
using GraphForge.Api.Services.SchemasService;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.Controllers;


[Route("api/projects/{projectId}/schemas")]
[ApiController]
[Authorize]
public class SchemasController: ControllerBase
{
    private readonly ISchemasService _schemasService;
    private readonly IUserIdentityProvider _userIdentityProvider;

    public SchemasController(ISchemasService schemasService, IUserIdentityProvider userIdentityProvider)
    {
        _schemasService = schemasService;
        _userIdentityProvider = userIdentityProvider;
    }


    [HttpGet]
    public async Task<IActionResult> GetSchemas(Guid projectId)
    {
        Guid userId = _userIdentityProvider.GetCurrentUserId();
        SchemasListResponse result = await _schemasService.GetSchemasList(userId, projectId);
        return Ok(result);       
    }
}
