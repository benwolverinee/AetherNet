using AetherNet.Service;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);

// Windows Service desteği ekle
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "AetherNet DPI Bypass Service";
});

// Event Log yapılandırması
LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

// Worker servisini ekle
builder.Services.AddHostedService<AetherNetWorker>();

var host = builder.Build();
host.Run();
