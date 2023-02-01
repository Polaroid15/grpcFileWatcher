namespace GrpcFileWorker.Server.Services;

public class FileHandlerFactory : IFileHandlerFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public FileHandlerFactory(IConfiguration configuration, ILoggerFactory loggerFactory) {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }
    
    public IFileHandler Create(FileGrpcCommon.FileType fileType) =>
        fileType switch {
            FileGrpcCommon.FileType.Json => new JsonFileHandler(_configuration, _loggerFactory),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, message:"Unknown file type")
        };
}