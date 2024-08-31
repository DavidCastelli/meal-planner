using Microsoft.AspNetCore.Mvc;

namespace Api.Common;

/// <summary>
/// Base controller which includes the api controller attribute.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase { }