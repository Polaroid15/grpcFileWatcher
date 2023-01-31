using System.Text.Json;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcFileWorker.Server;

public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger) {
        _logger = logger;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation) {
        //depend on request size, if too big, use another way for logging.
        try {
            var requestJson = JsonSerializer.Serialize(request);
            _logger.LogInformation(requestJson);
        }
        catch (Exception e) {
            _logger.LogError("Logging interceptor error: {Error message}", e.Message);
        }
        
        return base.UnaryServerHandler(request, context, continuation);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation) 
    {
        _logger.LogInformation("server side streaming call has been cancelled");
        return base.AsyncServerStreamingCall(request, context, continuation);
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation) 
    {
        _logger.LogInformation("client side streaming call has been cancelled");
        return base.AsyncClientStreamingCall(context, continuation);
    }
}