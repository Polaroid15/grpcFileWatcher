### File grpc watcher
#### Contains
* Console app .NET 6 version app

### Prepare to use
For using File grpc watcher app you need add to first args host address of grpc server (example, "http://localhost:5157")
After launch app starts to json file watching by path: Environment.CurrentDirectory/files.

### FileGrpcCommon
#### Contains
* Lib for sharing proto files .NET 6 version app

### FileGrpcWorker
#### Contains
* Asp.net core 6.0 web api version app

### Prepare to use
You can change base directory name for work with files. For this you need to change the appsetting parameter `DownloadFolderPath`. By default = "root"

### Prc

UploadBatches
UploadFilesStreaming

GetUploadedTypes
GetUploadedFiles
