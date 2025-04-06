using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YourNamespace.Data.Models;

namespace YourNamespace.Business
{
    public class UploadService
    {
        private readonly ApiClient _apiClient;
        private CancellationTokenSource _cancellationTokenSource;

        public UploadService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task UploadFilesAsync(
            IEnumerable<string> filePaths, 
            IProgress<int> progress, 
            IProgress<string> status)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            try
            {
                foreach (var filePath in filePaths)
                {
                    if (token.IsCancellationRequested)
                        break;

                    var fileName = Path.GetFileName(filePath);
                    status.Report($"Uploading {fileName}...");

                    using (var fileStream = File.OpenRead(filePath))
                    {
                        await _apiClient.UploadFileAsync(fileStream, fileName, progress, token);
                    }

                    status.Report($"{fileName} uploaded successfully");
                }
            }
            catch (OperationCanceledException)
            {
                status.Report("Upload cancelled");
            }
            catch (Exception ex)
            {
                status.Report($"Error: {ex.Message}");
            }
        }

        public void CancelUpload()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}