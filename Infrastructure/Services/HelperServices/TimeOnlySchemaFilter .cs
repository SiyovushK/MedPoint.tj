using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Infrastructure.Services.HelperServices;

public class TimeOnlySchemaFilter : ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (context.Type == typeof(TimeOnly))
    {
      schema.Example = new OpenApiString("00:00:00");
    }
  }
}