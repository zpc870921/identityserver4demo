using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace CustomUserServiceClient
{

    public static class HttpClientExtend
    {
        //public static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client,TokenRequest request,CancellationToken cancellationToken=default(CancellationToken))
        //{
        //    TokenRequest clone = (TokenRequest)request.Clone();
        //    if (!clone.Parameters.ContainsKey(OidcConstants.TokenRequest.GrantType))
        //    {
        //        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType,request.GrantType);
        //    }

        //    return await client.RequestTokenAsync(clone,cancellationToken).ConfigureAwait(false);
        //}
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var _httpClientFactory = new HttpClientFactory();
            
             GetToken("443813032@qq.com","123456").GetAwaiter().GetResult();
             Console.ReadLine();
        }


        private static async Task GetToken(string phoneNumber,string passWord)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest { Address = "http://localhost:5000", Policy = new DiscoveryPolicy { RequireHttps = false } });
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            var formvalues = new Dictionary<string, string>();
            formvalues.Add("scope", "profile openid offline_access socialnetwork");
            formvalues.Add("Phone", phoneNumber);
            formvalues.Add("PassWord", passWord);
            var content = new FormUrlEncodedContent(formvalues);
            TokenRequest tokenRequest = new TokenRequest
            {
                GrantType = "customuserservice",
                Address = disco.TokenEndpoint,
                ClientId = "userservices",
                ClientSecret = "secret",
                Parameters = formvalues
            };
            var tokenResponse = await client.RequestTokenAsync(tokenRequest);//自定义的授权模式请求


            Console.WriteLine("access_token:\n"+tokenResponse.AccessToken);
            Console.WriteLine("id_token:\n" + tokenResponse.IdentityToken);
            Console.WriteLine("refresh_token:\n" + tokenResponse.RefreshToken);


            await CallApiDemo(tokenResponse);

        }


        private static async Task CallApiDemo(TokenResponse tokenResponse)
        {
            if (null == tokenResponse || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                return;
            }
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5001");
            client.SetBearerToken(tokenResponse.AccessToken);
            var data=await client.GetAsync("api/values");
            var content = await data.Content.ReadAsStringAsync();
            Console.WriteLine("api values:\n"+content);

            Console.WriteLine("refresh_token");
            var client2 = new HttpClient();
            client2.BaseAddress = new Uri("http://localhost:5000");
            var refreshToken=await client2.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                  RefreshToken=tokenResponse.RefreshToken,
                  ClientId= "userservices",
                  ClientSecret="secret",
                  Address="http://localhost:5000/connect/token"
            });

            Console.WriteLine("access_token:\n" + refreshToken.AccessToken);
            Console.WriteLine("id_token:\n" + refreshToken.IdentityToken);
            Console.WriteLine("refresh_token:\n" + refreshToken.RefreshToken);
        }
    }
}
