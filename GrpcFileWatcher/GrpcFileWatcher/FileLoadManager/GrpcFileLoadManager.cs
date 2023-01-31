using System.Text;
using FileGrpcCommon;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace GrpcFileWatcher.FileLoadManager;

public class GrpcFileLoadManager : IFileLoadManager
{
    private readonly string _hostAddress;
    private readonly int _batchSize = 5;

    /// <summary>
    /// Constructor of GrpcFileManager.
    /// </summary>
    /// <param name="hostAddress">Address of grpc Server.</param>
    public GrpcFileLoadManager(string hostAddress) 
    {
        _hostAddress = hostAddress;
    }

    /// <summary>
    /// Constructor of GrpcFileManager.
    /// </summary>
    /// <param name="batchSize">Size of batch for partial upload files. By default = 5.</param>
    /// <param name="hostAddress">Address of grpc Server.</param>
    public GrpcFileLoadManager(int batchSize, string hostAddress) 
        : this(hostAddress) 
    {
        _batchSize = batchSize;
        _hostAddress = hostAddress;
    }

    /// <inheritdoc/>
    public async Task<bool> UploadFilesByBatchesAsync(string filePath) {
        using var channel = CreateChannel();
        var client = new GrpcFileWorker.GrpcFileWorkerClient(channel);

        string[] lines = await GetFileLinesAsync(filePath);
        var chunks = lines.Chunk(_batchSize);
        bool isUpload = false;
        
        foreach (string[] chunk in chunks) {
            var uploadBatchesRequest = new UploadBatchesRequest {
                Format = FileType.Json,
                Data = { chunk }
            };
            var result = await client.UploadBatchesAsync(uploadBatchesRequest);
            isUpload = result.IsUploaded;
        }
        return isUpload;
    }

    /// <inheritdoc/>
    public async Task<bool> UploadFilesByStreamingAsync(string filePath) {
        string[] lines = await GetFileLinesAsync(filePath);
        using var channel = CreateChannel();
        var client = new GrpcFileWorker.GrpcFileWorkerClient(channel);
        using var streaming = client.UploadFilesStreaming();
        
        foreach (var line in lines) {
            var data = Encoding.UTF8.GetBytes(line);
            var uploadFilesRequest = new UploadStreamingRequest() {
                Format = FileType.Json,
                Data = ByteString.CopyFrom(data)
            };
            await streaming.RequestStream.WriteAsync(uploadFilesRequest);
        }

        await streaming.RequestStream.CompleteAsync();
        var result = await streaming.ResponseAsync;
        
        return result.IsUploaded;
    }

    private GrpcChannel CreateChannel()
    {
        var methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(0.5),
                MaxBackoff = TimeSpan.FromSeconds(0.5),
                BackoffMultiplier = 1,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };

        return GrpcChannel.ForAddress(_hostAddress, new GrpcChannelOptions
        {
            ServiceConfig = new ServiceConfig { MethodConfigs = { methodConfig } }
        });
    }

    private static async Task<string[]> GetFileLinesAsync(string filePath) {
        return await File.ReadAllLinesAsync(filePath);
    }
}