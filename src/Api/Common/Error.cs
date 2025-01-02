namespace Api.Domain;

#pragma warning disable CA1716
/// <summary>
/// Holds error information which can later be sent back to the client.
/// </summary>
/// <param name="Code">The error code.</param>
/// <param name="Description">The error description.</param>
public sealed record Error(string Code, string Description);
#pragma warning restore CA1716