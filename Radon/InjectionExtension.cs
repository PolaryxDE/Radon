using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Radon.Exceptions;
using Radon.Middlewares;
using Radon.Security;
using Radon.Services;

namespace Radon;

/// <summary>
/// The extension class used to inject Radon into the application.
/// </summary>
public static class InjectionExtension
{
    /// <summary>
    /// Configures Radon and prepares the application for use of it.
    /// </summary>
    /// <param name="services">The service collection to which Radon will be added.</param>
    /// <param name="configuration">The configuration of Radon, if omitted, default Radon will be used.</param>
    /// <typeparam name="T">The type which implements the <see cref="IDataStorage"/> interface. Will be added as singleton to the services.</typeparam>
    public static IServiceCollection ConfigureRadon<T>(this IServiceCollection services, RadonConfiguration? configuration = null) where T : class, IDataStorage
    {
        services.AddSingleton(configuration ?? new RadonConfiguration());
        services.AddSingleton<IDataStorage, T>();
        services.AddSingleton<JwtService>();
        services.AddSingleton<AuthService>();
        
        return services;
    }

    /// <summary>
    /// Searches through all types in the executing assembly and adds all <see cref="Voter{TU,TS}"/> implementations
    /// to the current Radon runtime.
    /// </summary>
    public static IServiceCollection AddVoters(this IServiceCollection services)
    {
        foreach (var type in Assembly.GetCallingAssembly().DefinedTypes)
        {
            VoterCollection.Register(type);
        }

        return services;
    }

    /// <summary>
    /// Tells the web application to use Radon's authentication middleware.
    /// </summary>
    /// <param name="app">The app in which Radon will be activated.</param>
    public static void UseRadon(this IApplicationBuilder app)
    {
        app.UseMiddleware<JwtMiddleware>();

        if (app.ApplicationServices.GetRequiredService<RadonConfiguration>().EnableErrorHandler)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }

    /// <summary>
    /// Starts the voting process for the given user with the given subject. If a <see cref="Voter{TU,TS}"/> denies
    /// the authorization, an unauthorized response will be issued.
    /// </summary>
    /// <param name="controller">The controller base starting the voting process.</param>
    /// <param name="user">The user which should be authorized.</param>
    /// <param name="subject">The subject for the voter.</param>
    public static void VoteUser(this ControllerBase controller, UserBase user, object subject)
    {
        VoterCollection.Vote(user, subject);
    }

    /// <summary>
    /// Starts the voting process for the given user with the given subject. If a <see cref="Voter{TU,TS}"/> denies
    /// the authorization, an unauthorized response will be issued.
    /// </summary>
    /// <param name="controller">The controller base starting the voting process.</param>
    /// <param name="subject">The subject for the voter.</param>
    public static void VoteUser(this ControllerBase controller, object subject)
    {
        if (controller.HttpContext.Items["User"] is not UserBase user)
        {
            throw new RadonException("The user must be set in the HttpContext before voting.");
        }
        
        controller.VoteUser(user, subject);
    }
}