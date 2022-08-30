using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.Payment.UmPay;

namespace UmPayTest;

internal class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .ConfigureServices(services =>
           {
               _ = services.AddNatsMessageQueue(config => config
                   .ConfigQueueOptions((options, ap) =>
                   {
                       //options.Url = "nats://192.168.50.44:4222";
                       options.Url = "nats://localhost:4222";
                   }));
               _ = services.AddPaymentClient();
               _ = services
                   .AddSingleton<UmPaySerializer>();
               _ = services.AddHostedService<Demo>();
           });

    private static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
}
