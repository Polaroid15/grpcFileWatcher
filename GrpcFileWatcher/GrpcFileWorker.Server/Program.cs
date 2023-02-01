using GrpcFileWorker.Server;
using GrpcFileWorker.Server.GrpcServices;
using GrpcFileWorker.Server.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    // Setup a HTTP/2 endpoint without TLS.
    options.ListenLocalhost(5157, o => o.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>());
builder.Services.AddSingleton<IFileHandlerFactory, FileHandlerFactory>();
builder.Services.AddTransient<IFileHandler, JsonFileHandler>();

var app = builder.Build();

app.MapGrpcService<UploadedFileWorkerService>();
app.MapGrpcService<FileWorkerService>();

app.Run();