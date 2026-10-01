using FluentValidation;
using Learniverse.API.Contracts.Requests.Identity;
using Learniverse.API.Contracts.Responses;
using Learniverse.API.Middlewares;
using Learniverse.Application;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Infrastructure;
using Learniverse.Infrastructure.Authentication;
using Learniverse.Persistence;
using Learniverse.Persistence.Context;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace Learniverse.API
{
    public class Program
    {
        public static async Task  Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
            builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray());

                    var response = new ErrorResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "One or more validation errors occurred.",
                        Errors = errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            // OpenAPI
            builder.Services.AddOpenApi();
            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddApplication();
            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await context.Database.MigrateAsync(
                    app.Lifetime.ApplicationStopping);

                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();

                await IdentitySeeder.SeedRolesAsync(
                    roleManager);

                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<ApplicationUser>>();

                var bootstrapAdminOptions = scope.ServiceProvider
                    .GetRequiredService<IOptions<BootstrapAdminOptions>>();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                await BootstrapAdminSeeder.SeedAsync(
                    userManager,
                    bootstrapAdminOptions,
                    unitOfWork,
                    app.Lifetime.ApplicationStopping);
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapGet("/", () => Results.Redirect("/swagger"));
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}