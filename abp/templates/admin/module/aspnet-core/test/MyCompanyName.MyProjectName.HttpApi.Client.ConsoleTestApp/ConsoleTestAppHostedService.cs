using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using MyCompanyName.MyProjectName.Admin;
using MyCompanyName.MyProjectName.Common;
using MyCompanyName.MyProjectName.Public;

namespace MyCompanyName.MyProjectName.HttpApi.Client.ConsoleTestApp;

public class ConsoleTestAppHostedService : IHostedService
{
    private readonly IConfiguration _configuration;

    public ConsoleTestAppHostedService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<MyProjectNameConsoleApiClientModule>(options =>
        {
           options.Services.ReplaceConfiguration(_configuration);
           options.UseAutofac();
        }))
        {
            await application.InitializeAsync();

            var demoAdmin = application.ServiceProvider.GetRequiredService<ClientAdminDemoService>();
            await demoAdmin.RunAsync();

            var demoCommon = application.ServiceProvider.GetRequiredService<ClientCommonDemoService>();
            await demoCommon.RunAsync();

            var demoPublic = application.ServiceProvider.GetRequiredService<ClientPublicDemoService>();
            await demoPublic.RunAsync();

            await application.ShutdownAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
