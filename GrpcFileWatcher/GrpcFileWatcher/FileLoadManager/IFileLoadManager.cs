namespace GrpcFileWatcher.FileLoadManager;

public interface IFileLoadManager
{
    /// <summary>
    /// Upload file by batches to grpc server.
    /// </summary>
    /// <param name="filePath">absolute path to file.</param>
    /// <returns>Uploading status.</returns>
    Task<bool> UploadFilesByBatchesAsync(string filePath);
    
    /// <summary>
    /// Upload file by streaming to grpc server.
    /// </summary>
    /// <param name="filePath">absolute path to file.</param>
    /// <returns>Uploading status.</returns>
    Task<bool> UploadFilesByStreamingAsync(string filePath);
}