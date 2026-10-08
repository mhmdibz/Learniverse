using FluentValidation;
using Learniverse.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Services;

namespace Learniverse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        services.AddScoped<ICourseAuthorizationService, CourseAuthorizationService>();
        services.AddScoped<ICourseContentAccessService,CourseContentAccessService>();
        return services;
    }
}