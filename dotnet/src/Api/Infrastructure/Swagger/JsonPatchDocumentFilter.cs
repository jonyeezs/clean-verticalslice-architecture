using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CleanSlice.Api.Infrastructure.Swagger
{
    public class JsonPatchDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (KeyValuePair<string, OpenApiSchema> item in (List<KeyValuePair<string, OpenApiSchema>>)swaggerDoc.Components.Schemas.ToList())
            {
                if (item.Key.StartsWith("Operation") || item.Key.StartsWith("JsonPatchDocument"))
                {
                    _ = swaggerDoc.Components.Schemas.Remove(item.Key);
                }
            }
            swaggerDoc.Components.Schemas.Add("Operation", new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
            {
                { "op", new OpenApiSchema { Type = "string" } },
                { "value", new OpenApiSchema { Type = "string" } },
                { "path", new OpenApiSchema { Type = "string" } }
            }
            });
            swaggerDoc.Components.Schemas.Add("JsonPatchDocument", new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema
                {
                    Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "Operation" }
                },
                Description = "Array of operations to perform"
            });
            foreach (KeyValuePair<OperationType, OpenApiOperation> path in swaggerDoc.Paths.SelectMany(p => p.Value.Operations)
                         .Where(p => p.Key == OperationType.Patch))
            {
                foreach (KeyValuePair<string, OpenApiMediaType> item in path.Value.RequestBody.Content.Where(c => c.Key != "application/json-patch+json"))
                {
                    _ = path.Value.RequestBody.Content.Remove(item.Key);
                }

                KeyValuePair<string, OpenApiMediaType> response = path.Value.RequestBody.Content.SingleOrDefault(c => c.Key == "application/json-patch+json");

                response.Value.Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = "JsonPatchDocument" }
                };
            }
        }
    }
}
