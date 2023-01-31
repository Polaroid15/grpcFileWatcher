namespace GrpcFileWorker.Server.Services;

public class FileHandlerFactory : IFileHandlerFactory
{
    public IFileHandler Create(FileGrpcCommon.FileType fileType) =>
        fileType switch {
            FileGrpcCommon.FileType.Json => new JsonFileHandler(),
            _ => throw new ArgumentOutOfRangeException(nameof(fileType), fileType, message:"Unknown file type")
        };
}