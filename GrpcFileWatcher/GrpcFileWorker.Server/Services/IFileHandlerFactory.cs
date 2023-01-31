namespace GrpcFileWorker.Server.Services;

public interface IFileHandlerFactory
{
    IFileHandler Create(FileGrpcCommon.FileType fileType);
}