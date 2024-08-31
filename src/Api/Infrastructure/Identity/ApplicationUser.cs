using Microsoft.AspNetCore.Identity;

namespace Api.Infrastructure.Identity;

/// <summary>
/// Represents an application user which builds off the default identity user.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
}