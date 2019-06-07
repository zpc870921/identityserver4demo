using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.OidcClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace XamarinClient
{
    public class Program
    {
        static string _authority = "http://localhost:5000";
        static string _api = "http://localhost:5001/api/values";

        static OidcClient _oidcClient;
        static HttpClient _apiClient = new HttpClient { BaseAddress = new Uri(_api) };

        public static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();

        public static async Task RunAsync()
        {
            await Login();
        }

        private static async Task Login()
        {
            var browser = new SystemBrowser(50006);
            string redirectUri = "http://127.0.0.1:50006";

            var options = new OidcClientOptions
            {
                Authority = _authority,
                ClientId = "xamarin.client",
                ClientSecret = "secret",
                RedirectUri = redirectUri,
                Scope = "openid profile offline_access socialnetwork roles",
                FilterClaims = false,
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.Hybrid,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
                LoadProfile = true
            };

            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            options.LoggerFactory.AddSerilog(serilog);

            _oidcClient = new OidcClient(options);
            var result = await _oidcClient.LoginAsync(new LoginRequest());

            ShowResult(result);
            await NextSteps(result);
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }

        private static async Task NextSteps(LoginResult result)
        {
            var currentAccessToken = result.AccessToken;
            var currentRefreshToken = result.RefreshToken;

            var menu = " x:exit  c:call api";
            if (currentRefreshToken != null)
            {
                menu += "r:refresh token";
            }

            await CallApi(currentAccessToken);

            while (true)
            {
                Console.Write(menu);
                Console.Write("\n");
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.X) return;
                if (key.Key == ConsoleKey.C) await CallApi(currentAccessToken);
                if (key.Key == ConsoleKey.R)
                {
                    var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
                    if (result.IsError)
                    {
                        Console.WriteLine($"Error: {refreshResult.Error}");
                    }
                    else
                    {
                        currentRefreshToken = refreshResult.RefreshToken;
                        currentAccessToken = refreshResult.AccessToken;

                        Console.WriteLine($"access token:   {result.AccessToken}");
                        Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
                    }
                }
            }
        }

        private static async Task CallApi(string currentAccessToken)
        {
            _apiClient.SetBearerToken(currentAccessToken);
            var response = await _apiClient.GetAsync("");

            if (response.IsSuccessStatusCode)
            {
                var json = JArray.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Error: {response.ReasonPhrase}");
            }
        }
    }


    //public class UserToken
    //{
    //    [JsonProperty("id_token")]
    //    public string IdToken { get; set; }

    //    [JsonProperty("access_token")]
    //    public string AccessToken { get; set; }

    //    [JsonProperty("expires_in")]
    //    public int ExpiresIn { get; set; }

    //    [JsonProperty("token_type")]
    //    public string TokenType { get; set; }

    //    [JsonProperty("refresh_token")]
    //    public string RefreshToken { get; set; }
    //}
    //public interface IIdentityService
    //{
    //    string CreateAuthorizationRequest();
    //    string CreateLogoutRequest(string token);
    //    Task<UserToken> GetTokenAsync(string code);
    //}

    //public interface IRequestProvider
    //{
    //    Task<TResult> GetAsync<TResult>(string uri, string token = "");

    //    Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", string header = "");

    //    Task<TResult> PostAsync<TResult>(string uri, string data, string clientId, string clientSecret);

    //    Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "", string header = "");

    //    Task DeleteAsync(string uri, string token = "");
    //}

    //public class RequestProvider : IRequestProvider
    //{
    //    private readonly JsonSerializerSettings _serializerSettings;

    //    public RequestProvider()
    //    {
    //        _serializerSettings = new JsonSerializerSettings
    //        {
    //            ContractResolver = new CamelCasePropertyNamesContractResolver(),
    //            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
    //            NullValueHandling = NullValueHandling.Ignore
    //        };
    //        _serializerSettings.Converters.Add(new StringEnumConverter());
    //    }

    //    public async Task<TResult> GetAsync<TResult>(string uri, string token = "")
    //    {
    //        HttpClient httpClient = CreateHttpClient(token);
    //        HttpResponseMessage response = await httpClient.GetAsync(uri);

    //        await HandleResponse(response);
    //        string serialized = await response.Content.ReadAsStringAsync();

    //        TResult result = await Task.Run(() =>
    //            JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

    //        return result;
    //    }

    //    public async Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", string header = "")
    //    {
    //        HttpClient httpClient = CreateHttpClient(token);

    //        if (!string.IsNullOrEmpty(header))
    //        {
    //            AddHeaderParameter(httpClient, header);
    //        }

    //        var content = new StringContent(JsonConvert.SerializeObject(data));
    //        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        HttpResponseMessage response = await httpClient.PostAsync(uri, content);

    //        await HandleResponse(response);
    //        string serialized = await response.Content.ReadAsStringAsync();

    //        TResult result = await Task.Run(() =>
    //            JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

    //        return result;
    //    }

    //    public async Task<TResult> PostAsync<TResult>(string uri, string data, string clientId, string clientSecret)
    //    {
    //        HttpClient httpClient = CreateHttpClient(string.Empty);

    //        if (!string.IsNullOrWhiteSpace(clientId) && !string.IsNullOrWhiteSpace(clientSecret))
    //        {
    //            AddBasicAuthenticationHeader(httpClient, clientId, clientSecret);
    //        }

    //        var content = new StringContent(data);
    //        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
    //        HttpResponseMessage response = await httpClient.PostAsync(uri, content);

    //        await HandleResponse(response);
    //        string serialized = await response.Content.ReadAsStringAsync();

    //        TResult result = await Task.Run(() =>
    //            JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

    //        return result;
    //    }

    //    public async Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "", string header = "")
    //    {
    //        HttpClient httpClient = CreateHttpClient(token);

    //        if (!string.IsNullOrEmpty(header))
    //        {
    //            AddHeaderParameter(httpClient, header);
    //        }

    //        var content = new StringContent(JsonConvert.SerializeObject(data));
    //        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    //        HttpResponseMessage response = await httpClient.PutAsync(uri, content);

    //        await HandleResponse(response);
    //        string serialized = await response.Content.ReadAsStringAsync();

    //        TResult result = await Task.Run(() =>
    //            JsonConvert.DeserializeObject<TResult>(serialized, _serializerSettings));

    //        return result;
    //    }

    //    public async Task DeleteAsync(string uri, string token = "")
    //    {
    //        HttpClient httpClient = CreateHttpClient(token);
    //        await httpClient.DeleteAsync(uri);
    //    }

    //    private HttpClient CreateHttpClient(string token = "")
    //    {
    //        var httpClient = new HttpClient();
    //        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    //        }
    //        return httpClient;
    //    }

    //    private void AddHeaderParameter(HttpClient httpClient, string parameter)
    //    {
    //        if (httpClient == null)
    //            return;

    //        if (string.IsNullOrEmpty(parameter))
    //            return;

    //        httpClient.DefaultRequestHeaders.Add(parameter, Guid.NewGuid().ToString());
    //    }

    //    private void AddBasicAuthenticationHeader(HttpClient httpClient, string clientId, string clientSecret)
    //    {
    //        if (httpClient == null)
    //            return;

    //        if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
    //            return;

    //        httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(clientId, clientSecret);
    //    }

    //    private async Task HandleResponse(HttpResponseMessage response)
    //    {
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var content = await response.Content.ReadAsStringAsync();

    //            if (response.StatusCode == HttpStatusCode.Forbidden ||
    //                response.StatusCode == HttpStatusCode.Unauthorized)
    //            {
    //                throw new ServiceAuthenticationException(content);
    //            }

    //            throw new HttpRequestExceptionEx(response.StatusCode, content);
    //        }
    //    }
    //}

    //public class HttpRequestExceptionEx : HttpRequestException
    //{
    //    public System.Net.HttpStatusCode HttpCode { get; }
    //    public HttpRequestExceptionEx(System.Net.HttpStatusCode code) : this(code, null, null)
    //    {
    //    }

    //    public HttpRequestExceptionEx(System.Net.HttpStatusCode code, string message) : this(code, message, null)
    //    {
    //    }

    //    public HttpRequestExceptionEx(System.Net.HttpStatusCode code, string message, Exception inner) : base(message,
    //        inner)
    //    {
    //        HttpCode = code;
    //    }

    //}

    //public class IdentityService : IIdentityService
    //{
    //    private readonly IRequestProvider _requestProvider;
    //    private string _codeVerifier;

    //    public IdentityService(IRequestProvider requestProvider)
    //    {
    //        _requestProvider = requestProvider;
    //    }

    //    public string CreateAuthorizationRequest()
    //    {
    //        // Create URI to authorization endpoint
    //        var authorizeRequest = new AuthorizeRequest(GlobalSetting.Instance.IdentityEndpoint);

    //        // Dictionary with values for the authorize request
    //        var dic = new Dictionary<string, string>();
    //        dic.Add("client_id", GlobalSetting.Instance.ClientId);
    //        dic.Add("client_secret", GlobalSetting.Instance.ClientSecret);
    //        dic.Add("response_type", "code id_token");
    //        dic.Add("scope", "openid profile basket orders locations marketing offline_access");
    //        dic.Add("redirect_uri", GlobalSetting.Instance.IdentityCallback);
    //        dic.Add("nonce", Guid.NewGuid().ToString("N"));
    //        dic.Add("code_challenge", CreateCodeChallenge());
    //        dic.Add("code_challenge_method", "S256");

    //        // Add CSRF token to protect against cross-site request forgery attacks.
    //        var currentCSRFToken = Guid.NewGuid().ToString("N");
    //        dic.Add("state", currentCSRFToken);

    //        var authorizeUri = authorizeRequest.Create(dic);
    //        return authorizeUri;
    //    }

    //    public string CreateLogoutRequest(string token)
    //    {
    //        if (string.IsNullOrEmpty(token))
    //        {
    //            return string.Empty;
    //        }

    //        return string.Format("{0}?id_token_hint={1}&post_logout_redirect_uri={2}",
    //            GlobalSetting.Instance.LogoutEndpoint,
    //            token,
    //            GlobalSetting.Instance.LogoutCallback);
    //    }

    //    public async Task<UserToken> GetTokenAsync(string code)
    //    {
    //        string data = string.Format("grant_type=authorization_code&code={0}&redirect_uri={1}&code_verifier={2}", code, WebUtility.UrlEncode(GlobalSetting.Instance.IdentityCallback), _codeVerifier);
    //        var token = await _requestProvider.PostAsync<UserToken>(GlobalSetting.Instance.TokenEndpoint, data, GlobalSetting.Instance.ClientId, GlobalSetting.Instance.ClientSecret);
    //        return token;
    //    }

    //    private string CreateCodeChallenge()
    //    {
    //        _codeVerifier = RandomNumberGenerator.CreateUniqueId();
    //        var sha256 = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
    //        var challengeBuffer = sha256.HashData(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(_codeVerifier)));
    //        byte[] challengeBytes;
    //        CryptographicBuffer.CopyToByteArray(challengeBuffer, out challengeBytes);
    //        return Base64Url.Encode(challengeBytes);
    //    }
}
