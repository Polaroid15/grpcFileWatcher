using FileGrpcCommon;
using Grpc.Core;
using GrpcFileWorker.Server.Services;

namespace GrpcFileWorker.Server.GrpcServices;

public class FileWorkerService : FileGrpcCommon.GrpcFileWorker.GrpcFileWorkerBase
{
    private readonly IFileHandlerFactory _fileHandlerFactory;
    private readonly ILogger<FileWorkerService> _logger;

    public FileWorkerService(IFileHandlerFactory fileHandlerFactory, ILogger<FileWorkerService> logger) {
        _fileHandlerFactory = fileHandlerFactory ?? throw new ArgumentNullException(nameof(fileHandlerFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<UploadFilesResponse> UploadBatches(UploadBatchesRequest request, ServerCallContext context) 
    {
        using var fileHandler = _fileHandlerFactory.Create(request.Format);
        bool isUpload = false;
        foreach (var dataArray in request.Data) 
        {
            isUpload = await fileHandler.UploadFileAsync(dataArray, context.CancellationToken);
        }

        return new UploadFilesResponse() { IsUploaded = isUpload };
    }

    public override async Task<UploadFilesResponse> UploadFilesStreaming(
        IAsyncStreamReader<UploadStreamingRequest> requestStream,
        ServerCallContext context)
    {
        bool isUpload = false;
        try {
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var item = requestStream.Current;
                using var fileHandler = _fileHandlerFactory.Create(item.Format);
                isUpload = await fileHandler.UploadFileAsync(item.Data.ToByteArray(), context.CancellationToken);
            }
        }
        catch (RpcException e) {
            _logger.LogError("FileWorkerService error: {error message}", e.Message);;
        }
        
        return new UploadFilesResponse { IsUploaded = isUpload };
    }
}