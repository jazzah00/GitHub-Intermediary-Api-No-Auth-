using GitHub_Intermediary_Api.Framework;
using GitHub_Intermediary_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GitHub_Intermediary_Api.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class GitHubController() : ControllerBase {
        [HttpGet("RetrieveUsers")]
        public string RetrieveUsersJson([FromQuery] List<string> usernames) {
            ApiUserResponse apiUserResponse = RetrieveUsers(usernames);
            return JsonConvert.SerializeObject(apiUserResponse);
        }

        [HttpGet("RetrieveUsersXml")]
        public string RetrieveUsersXml([FromQuery] List<string> usernames) {
            ApiUserResponse apiUserResponse = RetrieveUsers(usernames);
            string xmlString = new Converter().ConvertToXml(apiUserResponse, "ApiUserResponse");
            return xmlString;
        }

        private static ApiUserResponse RetrieveUsers(List<string> usernames) {
            usernames = new Validator().ValidateUsernames(usernames, out Dictionary<string, string> errors);
            List<User> users = [];
            foreach (string username in usernames) {
                User? user = new ApiConnector().RetrieveUsersAsync(username).Result;
                if (user != null) users.Add(user);
                else errors.Add(username, "User not found.");
            }
            ApiUserResponse apiUserResponse = new() {
                Users = [.. users.OrderBy(u => u.Name)],
                Errors = errors.Select(e => new Error { Username = e.Key, Message = e.Value }).ToList()
            };
            return apiUserResponse;
        }
    }
}
