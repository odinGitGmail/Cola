using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cola.Console
{
    public static class ColaConsoleInject
    {
        public static IServiceCollection AddSingletonColaConsole(
        this IServiceCollection services)
        {
            services.AddSingleton<IColaConsole>(provider => new ColaConsole());
            System.Console.WriteLine("注入类型【 IColaConsole, ColaConsole 】");
            return services;
        }
    }
}
