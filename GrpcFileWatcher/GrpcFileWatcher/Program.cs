using GrpcFileWatcher.FileLoadManager;

var watchPath = Path.Combine(Environment.CurrentDirectory, "files");
var directoryInfo = new DirectoryInfo(watchPath);
if (!directoryInfo.Exists) {
    directoryInfo.Create();
}

var fileLoadManager = new GrpcFileLoadManager("http://localhost:5157");

while (true) {
    string[] filesPaths = Directory.GetFiles(watchPath, "*.json", SearchOption.AllDirectories);
    
    var result = false;
    foreach (var filepath in filesPaths) {
        result = await fileLoadManager.UploadFilesByBatchesAsync(filepath);    
    }
    
    foreach (var filepath in filesPaths) {
        result = await fileLoadManager.UploadFilesByStreamingAsync(filepath);    
    }

    if (result) {
        File.Delete(watchPath);
    }
    
    await Task.Delay(10000);
}