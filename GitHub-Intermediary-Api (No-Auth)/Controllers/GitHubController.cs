using GitHub_Intermediary_Api.Framework;
using GitHub_Intermediary_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

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
            XmlSerializer xmlSerializer = new(apiUserResponse.GetType());
            using StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, apiUserResponse);
            return stringWriter.ToString();
        }

        private static ApiUserResponse RetrieveUsers(List<string> usernames) {
            usernames = ValidateUsernames(usernames, out Dictionary<string, string> errors);
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

        private static List<string> ValidateUsernames(List<string> usernames, out Dictionary<string, string> errors) {
            List<string> validUsernames = []; errors = [];
            Regex regex = new(@"^[a-zA-Z0-9-]+$");
            foreach (string username in usernames) {
                if (regex.IsMatch(username)) {
                    if (!validUsernames.Contains(username)) validUsernames.Add(username);
                    else errors.Add(username, "Duplicate username found.");
                } else errors.Add(username, "Username is not in valid alphanumeric and hypen format.");
            }
            return validUsernames;
        }
    }
}
