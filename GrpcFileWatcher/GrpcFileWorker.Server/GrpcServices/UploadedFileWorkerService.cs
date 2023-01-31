using FileGrpcCommon.Protos;
using GrpcFileWorker.Server.Services;

namespace GrpcFileWorker.Server.GrpcServices;

public class UploadedFileWorkerService : GrpcUploadedFileWorker.GrpcUploadedFileWorkerBase
{
    private readonly IFileHandlerFactory _fileHandlerFactory;
    private readonly ILogger<UploadedFileWorkerService> _logger;

    public UploadedFileWorkerService(IFileHandlerFactory fileHandlerFactory, ILogger<UploadedFileWorkerService> logger)
    {
        _fileHandlerFactory = fileHandlerFactory;
        _logger = logger;
    }
}