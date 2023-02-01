using FileGrpcCommon;
using FileGrpcCommon.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcFileWorker.Server.Services;
using GetUploadedTypesResponse = FileGrpcCommon.Protos.GetUploadedTypesResponse;

namespace GrpcFileWorker.Server.GrpcServices;

public class UploadedFileWorkerService : GrpcUploadedFileWorker.GrpcUploadedFileWorkerBase
{
    private readonly IFileHandlerFactory _fileHandlerFactory;

    public UploadedFileWorkerService(IFileHandlerFactory fileHandlerFactory)
    {
        _fileHandlerFactory = fileHandlerFactory;
    }
    
    public override Task<GetUploadedTypesResponse> GetUploadedTypes(Empty request, ServerCallContext context) {
        if (context.CancellationToken.IsCancellationRequested) {
            return Task.FromResult(new GetUploadedTypesResponse());
        }

        var fileHandler = _fileHandlerFactory.Create(FileType.Json);
        var result = fileHandler.GetTypes();
        return Task.FromResult(new GetUploadedTypesResponse { MessageTypes = { result } });
    }

    
    public override async Task GetUploadedFiles(
        GetUploadedFilesRequest request,
        IServerStreamWriter<GetUploadedFilesResponse> responseStream,
        ServerCallContext context) 
    {
        if (context.CancellationToken.IsCancellationRequested) {
            return;
        }

        var fileHandler = _fileHandlerFactory.Create(FileType.Json);
        await foreach (var item in fileHandler.GetFilesAsync(request.Type, context.CancellationToken)) 
        {
            var message = new GetUploadedFilesResponse { Data = { item } };
            await responseStream.WriteAsync(message);
        }
    }
}