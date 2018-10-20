using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;

namespace server.Controllers
{
    [Route("auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private static AuthSecretRequest secretRequest;

        static LoginController() {
            DirectoryInfo info = new DirectoryInfo(".");
            foreach (FileInfo file in info.EnumerateFiles())
            {
                if (file.Name.StartsWith("client_secret"))
                {
                    string str = "";
                    StreamReader reader = new StreamReader(file.FullName);
                    using (reader)
                    {
                        str = reader.ReadToEnd();
                    }
                    secretRequest = JsonConvert.DeserializeObject<AuthSecretRequest>(str);
                    return;
                }
            }
            throw new FileNotFoundException("Could not locate secret token file");
        }

        private class AuthSecretRequest
        {

            public class Web
            {

                public string client_id;
                public string client_secret;
                public string[] redirect_uris;

            }

            public Web web;

        }

        private class AuthSecretResponse
        {

            public class IDToken
            {

                public string name;
                public string email;

            }

            public string access_token;
            public string id_token;
            public int expires_in;
            public string token_type;
            public string refresh_token;

        }

        [Route("check")]
        [HttpGet]
        public ObjectResult Check()
        {
            ObjectResult result = new ObjectResult(null);
            if (false)
            {
                result.StatusCode = 200;
                result.Value = "zach";
            } else
            {
                result.StatusCode = 401;
            }
            return result;
        }

        [Route("id")]
        [HttpGet]
        public ObjectResult GetId()
        {
            return Ok(secretRequest.web.client_id);
        }

        [Route("login")]
        [HttpPost]
        public async System.Threading.Tasks.Task<ObjectResult> LoginAsync()
        {
            string code;
            StreamReader reader = new StreamReader(Request.Body);
            code = reader.ReadToEnd();

            var values = new Dictionary<string, string>
            {
                { "code", code},
                { "client_id", secretRequest.web.client_id },
                { "client_secret", secretRequest.web.client_secret },
                { "redirect_uri", secretRequest.web.redirect_uris[0] },
                { "grant_type", "authorization_code" }
            };
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(values);

            var response2 = await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", content);

            var responseString = await response2.Content.ReadAsStringAsync();

            //IDateTimeProvider dateTimeProvider = new JWT.UtcDateTimeProvider();

            
            AuthSecretResponse response3 = JsonConvert.DeserializeObject<AuthSecretResponse>(responseString);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(response3.id_token);
            

            return Ok(token.Payload["name"]);
        }

    }
}
