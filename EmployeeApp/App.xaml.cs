using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System;
using NLog.Extensions.Logging;

namespace EmpClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        public App()
        {
            _host = new HostBuilder()
                            .ConfigureAppConfiguration((context, configurationBuilder) =>
                            {
                                configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                            })
                            .ConfigureServices((context, services) =>
                            {
                                services.AddSingleton<MainWindow>();
                            })
                            .ConfigureLogging(logBuilder =>
                            {
                                logBuilder.SetMinimumLevel(LogLevel.Information);
                                logBuilder.AddNLog("nlog.config");
                            })
                            .Build();

            using (var serviceScope = _host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var masterWindow = services.GetRequiredService<MainWindow>();
                    masterWindow.Show();

                    Console.WriteLine("Sucess");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured: " + ex.Message);
                }
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
