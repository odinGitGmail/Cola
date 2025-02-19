using Microsoft.Extensions.DependencyInjection;
namespace Cola.Console
{
    public static class Inject
    {
        public static IServiceCollection AddSingletonColaConsole(
        this IServiceCollection services)
        {
            var colaConsole = new ColaConsole();
            services.AddSingleton<IColaConsole>(provider => colaConsole);
            colaConsole!.WriteInfo("注入【 ColaConsole 】");
            return services;
        }
    }
}
