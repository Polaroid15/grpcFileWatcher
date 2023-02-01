namespace GrpcFileWorker.Server.Services;

public interface IFileHandler
{
    /// <summary>
    /// Upload file.
    /// </summary>
    /// <param name="data">Data to upload in bytes.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Flag for upload status.</returns>
    Task<bool> UploadFileAsync(byte[] data, CancellationToken cancellationToken);
    
    /// <summary>
    /// Upload file.
    /// </summary>
    /// <param name="data">Data to upload in string.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Flag for upload status.</returns>
    Task<bool> UploadFileAsync(string data, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get files by type.
    /// </summary>
    /// <param name="type">Type of files.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <returns>Collections of files.</returns>
    IAsyncEnumerable<string> GetFilesAsync(string type, CancellationToken cancellationToken);
    
    /// <summary>
    /// Get existing types.
    /// </summary>
    /// <returns>Array of types.</returns>
    string[] GetTypes();
}