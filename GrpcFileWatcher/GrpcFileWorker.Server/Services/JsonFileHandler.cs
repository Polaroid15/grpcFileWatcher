using System.Runtime.CompilerServices;
using System.Text;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GrpcFileWorker.Server.Services;

public class JsonFileHandler : IFileHandler
{
    private readonly string _basePath;
    private const string DefaultDownloadFolderPath = "root";
    private const string TypeSectionName = "type";
    private const string DefaultFileName = "_log_content.json";
    
    private readonly ILogger<JsonFileHandler> _logger;

    public JsonFileHandler(IConfiguration configuration, ILoggerFactory loggerFactory) {
        _logger = loggerFactory.CreateLogger<JsonFileHandler>();
        _basePath = Path.Combine(Environment.CurrentDirectory, configuration["DownloadFolderPath"] ?? DefaultDownloadFolderPath);
    }
    
    public async Task<bool> UploadFileAsync(byte[] data, CancellationToken cancellationToken) 
    {
        if (cancellationToken.IsCancellationRequested) 
        {
            return false;
        }

        var source = Encoding.UTF8.GetString(data);
        await UploadFile(source, cancellationToken);
        return true;
    }

    public async Task<bool> UploadFileAsync(string data, CancellationToken cancellationToken) 
    {
        if (cancellationToken.IsCancellationRequested) 
        {
            return false;
        }

        await UploadFile(data, cancellationToken);
        return true;
    }

    private async Task UploadFile(string data, CancellationToken cancellationToken) 
    {
        try 
        {
            var obj = JObject.Parse(data);
            var type = (string)obj[TypeSectionName]!;
            var date = DateTimeOffset.UtcNow;
            var path = Path.Combine(_basePath, type, date.ToString("yyyy-MM-dd"));
            CreateIfNotExistDirectory(path);

            var fileName = date.ToUnixTimeMilliseconds() + DefaultFileName;
            var output = Path.Combine(path, fileName);
            await File.WriteAllTextAsync(output, data, cancellationToken);
        }
        catch (JsonException e) 
        {
            _logger.LogInformation("Json parsing error: {Error message}", e.Message);
        }
        catch (Exception e) 
        {
            _logger.LogInformation("Error: {Error message}", e.Message);
        }
    }

    public string[] GetTypes() 
    {
        CreateIfNotExistDirectory(_basePath);
        var directoryInfo = new DirectoryInfo(_basePath);
        var result = directoryInfo.GetDirectories().Select(x => x.Name).ToArray();
        return result;
    }

    public async IAsyncEnumerable<string> GetFilesAsync(string type, [EnumeratorCancellation] CancellationToken cancellationToken) 
    {
        if (cancellationToken.IsCancellationRequested) {
            yield break;
        }

        CreateIfNotExistDirectory(_basePath);
        
        var directoryInfo = new DirectoryInfo(_basePath);
        var typeDirectoryInfo = directoryInfo.GetDirectories().FirstOrDefault(x => x.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase));
        
        if (typeDirectoryInfo == null)
        {
            _logger.LogInformation("{type} was not found", type);
            throw new RpcException(new Status(StatusCode.NotFound, $"{type} was not found"));
        }
        
        var typeDirectoryPath = Path.Combine(_basePath, typeDirectoryInfo.Name);
        string[] filesPaths = Directory.GetFiles(typeDirectoryPath, "*.json", SearchOption.AllDirectories);

        foreach (var path in filesPaths) {
            yield return await File.ReadAllTextAsync(path, cancellationToken);
        }
    }

    private static void CreateIfNotExistDirectory(string path) 
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}