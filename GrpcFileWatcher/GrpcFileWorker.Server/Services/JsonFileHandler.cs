using System.Text;
using Newtonsoft.Json.Linq;

namespace GrpcFileWorker.Server.Services;

public class JsonFileHandler : IFileHandler
{
    private bool _disposedValue;
    
    public async Task<bool> UploadFileAsync(byte[] data, CancellationToken cancellationToken) 
    {
        if (cancellationToken.IsCancellationRequested) 
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UploadFileAsync(string data, CancellationToken cancellationToken) 
    {

        if (cancellationToken.IsCancellationRequested) 
        {
            return false;
        }
        
        return true;
    }

    public async Task<string[]> GetTypesAsync() {
        List<string> result = new List<string>();
        //TODO : пойти по пути "корневая папка/type"
        return result.ToArray();
    }

    public async Task<string[]> GetFilesAsync(string type) {
        //TODO : пойти по пути "корневая папка/type/date"
        return new[] { "" };
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
           //todo 
        }
        _disposedValue = true;
    }
    
}