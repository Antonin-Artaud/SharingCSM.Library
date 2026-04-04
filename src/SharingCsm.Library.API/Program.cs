using System.Text.Json.Serialization;
using SharingCsm.Library.Api.Exceptions;
using SharingCsm.Library.Application;
using SharingCsm.Library.Infrastructure;

namespace SharingCsm.Library.Api
{
	public abstract class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.AddServiceDefaults();

			builder.Services.AddControllers()
				.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
			
			builder.Services.AddOpenApi();

			builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
			builder.Services.AddProblemDetails();

			builder.Services.AddApplicationModule();
			builder.Services.AddInfrastructureModule(builder.Configuration);

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.MapOpenApi(); 
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/openapi/v1.json", "Library API V1");
				});
			}

			app.UseHttpsRedirection();

			app.MapControllers();

			await app.RunAsync();
		}
	}
}
