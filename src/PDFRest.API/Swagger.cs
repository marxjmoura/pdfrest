using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace PDFRest.API
{
    [ExcludeFromCodeCoverage]
    public static class Swagger
    {
        const string ApiTitle = "PDF Rest";
        const string ApiVersion = "v1";
        const string BasePath = "/";

        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiVersion, new OpenApiInfo
                {
                    Title = ApiTitle,
                    Version = ApiVersion
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
                options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                options.TagActionsBy(api => new[] { api.GroupName });
            });
        }

        public static void UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UsePathBase(BasePath);

            app.UseSwagger(setup =>
            {
                setup.PreSerializeFilters.Add((swaggerDoc, request) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"{request.Scheme}://{request.Host.Value}{BasePath}" }
                    };
                });
            });

            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("swagger/v1/swagger.json", ApiTitle);
                setup.DefaultModelsExpandDepth(-1);
                setup.DocExpansion(DocExpansion.List);
                setup.DocumentTitle = ApiTitle;
                setup.RoutePrefix = string.Empty;
            });
        }
    }
}
