using BooksApiClient.Dto;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BooksApiClient
{
    public class RestClientResponse<T> where T : class
    {
        public HttpStatusCode StatusCode { get; }
        public T? Response { get; }
        public RestClientResponse(T response)
        {
            StatusCode = HttpStatusCode.OK;
            Response = response;
        }
        public RestClientResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
    public class RestClient
    {
        readonly string server;
        readonly JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
        public RestClient(string server = "https://localhost:8443")
        {
            this.server = server;
        }
        async Task<RestClientResponse<string>> RequestAsync(HttpMethod method, string url, object? data=null)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(180);
                var request = new HttpRequestMessage(method, url);

                if (data != null)
                {
                    string body = JsonSerializer.Serialize(data, options);
                    var bytes = Encoding.UTF8.GetBytes(body);
                    request.Content = new StringContent(body);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json")
                    {
                        CharSet = Encoding.UTF8.WebName
                    };
                }
                var response = await client.SendAsync(request).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var responseStream = response.Content.ReadAsStream();
                    using var streamReader = new StreamReader(responseStream);
                    StringBuilder result = new();
                    result.Append(streamReader.ReadToEnd());
                    return new RestClientResponse<string>(result.ToString());
                }
                else
                {
                    return new RestClientResponse<string>(response.StatusCode);
                }
            }
            catch
            {
            }
            return new RestClientResponse<string>(HttpStatusCode.InternalServerError);
        }
        protected async Task<RestClientResponse<T>> JsonRequestAsync<T>(HttpMethod method, string url, object? data=null) where T : class
        {
            try
            {
                var response = await RequestAsync(method, url, data);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(response.Response))
                    {
                        var res = JsonSerializer.Deserialize<T>(response.Response,options);
                        if (res != null)
                        {
                            return new RestClientResponse<T>(res);
                        }
                    }
                }
                else
                {
                    return new RestClientResponse<T>(response.StatusCode);
                }
            }
            catch
            {
            }
            return new RestClientResponse<T>(HttpStatusCode.InternalServerError);
        }
        string Url(string path)
        {
            return $"{server}/api{path}";
        }
        public async Task<RestClientResponse<BookDto>> GetBookAsync(long id)
        {
            return await JsonRequestAsync<BookDto>(HttpMethod.Get, Url($"/books/{id}"));
        }
        public async Task<RestClientResponse<BooksDto>> GetBooksAsync(int? pageNumber, int pageSize, long? bookId, string? filterByTitle, string? filterByAuthor)
        {
            StringBuilder url = new (Url($"/books?pageSize={pageSize}"));
            if (pageNumber != null) url.Append($"&pageNumber={pageNumber}");
            if (bookId != null) url.Append($"&bookId={bookId}");
            if (!string.IsNullOrEmpty(filterByTitle)) url.Append($"&filterByTitle={filterByTitle}");
            if (!string.IsNullOrEmpty(filterByAuthor)) url.Append($"&filterByAuthor={filterByAuthor}");
            return await JsonRequestAsync<BooksDto>(HttpMethod.Get, url.ToString());
        }
        public async Task<RestClientResponse<BookDto>> PostBookAsync(BookInformationDto dto)
        {
            return await JsonRequestAsync<BookDto>(HttpMethod.Post, Url("/books"), dto);
        }
        public async Task<RestClientResponse<BookDto>> PutBookAsync(long id, BookInformationDto dto)
        {
            return await JsonRequestAsync<BookDto>(HttpMethod.Put, Url($"/books/{id}"), dto);
        }
        public async Task<RestClientResponse<BookDto>> DeleteBookAsync(long id)
        {
            return await JsonRequestAsync<BookDto>(HttpMethod.Delete, Url($"/books/{id}"));
        }
    }
}