namespace GrpcFileWorker.Server.Services;

public interface IFileHandler : IDisposable
{
    Task<bool> UploadFileAsync(byte[] data, CancellationToken cancellationToken);
    Task<bool> UploadFileAsync(string data, CancellationToken cancellationToken);
    Task<string[]> GetTypesAsync();
    Task<string[]> GetFilesAsync(string type);
}