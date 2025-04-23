using System.Globalization;
using System.Reflection;

using Auction.Application.Behaviours;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Auction.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;
        Assembly executingAssembly = Assembly.GetExecutingAssembly();

        // Register validators from both assemblies
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        services.AddValidatorsFromAssembly(assembly);
        services.AddValidatorsFromAssembly(executingAssembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        return services;
    }
}