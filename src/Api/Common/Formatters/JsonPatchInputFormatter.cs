using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Api.Common.Formatters;

/// <summary>
/// Utility class for configuring custom Json patch formatters.
/// </summary>
public static class JsonPatchInputFormatter
{
    /// <summary>
    /// Configures and gets a Newtonsoft Json patch input formatter to be used to handle patch requests.
    /// </summary>
    /// <returns>A <see cref="NewtonsoftJsonPatchInputFormatter"/>.</returns>
    public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder.GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}