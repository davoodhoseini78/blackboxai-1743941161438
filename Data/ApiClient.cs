using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using YourNamespace.Data.Models;

namespace YourNamespace.Data
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private string _authToken;

        public ApiClient(string baseAddress)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
        }

        public void SetAuthToken(string token)
        {
            _authToken = token;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<AuthResponse> AuthenticateAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new { username, password });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AuthResponse>();
        }

        public async Task UploadFileAsync(
            Stream fileStream, 
            string fileName, 
            IProgress<int> progress = null, 
            CancellationToken cancellationToken = default)
        {
            using (var content = new MultipartFormDataContent())
            {
                var streamContent = new ProgressStreamContent(fileStream, 4096);
                streamContent.ProgressChanged += (bytesRead, totalBytes) => 
                {
                    progress?.Report((int)((double)bytesRead / totalBytes * 100));
                };

                content.Add(streamContent, "file", fileName);

                var response = await _httpClient.PostAsync(
                    "/api/upload", 
                    content, 
                    cancellationToken);

                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<List<HistoryItem>> GetUploadHistoryAsync()
        {
            var response = await _httpClient.GetAsync("/api/history");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<HistoryItem>>();
        }
    }

    public class ProgressStreamContent : StreamContent
    {
        private readonly Stream _stream;
        private readonly int _bufferSize;
        public event Action<long, long> ProgressChanged;

        public ProgressStreamContent(Stream stream, int bufferSize) : base(stream)
        {
            _stream = stream;
            _bufferSize = bufferSize;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var buffer = new byte[_bufferSize];
            var totalLength = _stream.Length;
            var totalRead = 0L;

            while (true)
            {
                var read = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (read <= 0) break;

                await stream.WriteAsync(buffer, 0, read);
                totalRead += read;
                ProgressChanged?.Invoke(totalRead, totalLength);
            }
        }
    }
}