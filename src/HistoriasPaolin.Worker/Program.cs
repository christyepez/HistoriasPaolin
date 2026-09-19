using HistoriasPaolin.Worker;
using HistoriasPaolin.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHistoriasPaolinInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
