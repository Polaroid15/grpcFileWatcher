namespace GrpcFileWorker.Server.Services;

public interface IFileHandlerFactory
{
    /// <summary>
    /// Create instance of <see cref="IFileHandler"/>
    /// </summary>
    /// <param name="fileType">file type <see cref="FileGrpcCommon.FileType"/>.</param>
    /// <returns></returns>
    IFileHandler Create(FileGrpcCommon.FileType fileType);
}