using GitHub_Intermediary_Api.Controllers;
using GitHub_Intermediary_Api.Models;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace GitHub_Intermediary_Api_xTest {
    public class GitHubControllerTest {
        [Fact]
        public void RetrieveUsers_Valid_Xml() {
            List<string> usernames = ["Jazzah00", "Octacat"];
            string result = new GitHubController().RetrieveUsersXml(usernames);
            Assert.NotNull(result);

            if (!string.IsNullOrEmpty(result)) {
                using (StringReader reader = new(result)) {
                    XmlSerializer xmlSerializer = new(typeof(ApiUserResponse));
                    ApiUserResponse response = (ApiUserResponse)xmlSerializer.Deserialize(reader);

                    Assert.NotNull(response);
                    Assert.Equal(2, response.Users.Count);
                }
            }
        }

        [Fact]
        public void RetrieveUsers_Valid_Json() {
            List<string> usernames = ["Jazzah00", "Octacat"];
            string result = new GitHubController().RetrieveUsersJson(usernames);
            Assert.NotNull(result);

            if (!string.IsNullOrEmpty(result)) {
                ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(result);
                Assert.NotNull(response);
                Assert.Equal(2, response.Users.Count);
            }
        }

        [Fact]
        public void RetrieveUsers_Valid_With_Duplicate() {
            List<string> usernames = ["Jazzah00", "Jazzah00", "Octadog"];
            string result = new GitHubController().RetrieveUsersJson(usernames);
            Assert.NotNull(result);

            if (!string.IsNullOrEmpty(result)) {
                ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(result);
                Assert.NotNull(response);
                Assert.Equal(2, response.Users.Count);
                Assert.Single(response.Errors);
            }
        }

        [Fact]
        public void RetrieveUsers_Empty_With_Errors() {
            List<string> usernames = ["Jazzah 00", "petepenguin"];
            string result = new GitHubController().RetrieveUsersJson(usernames);
            Assert.NotNull(result);

            if (!string.IsNullOrEmpty(result)) {
                ApiUserResponse response = JsonConvert.DeserializeObject<ApiUserResponse>(result);
                Assert.NotNull(response);
                Assert.Empty(response.Users);

                Assert.Equal(2, response.Errors.Count);
                Assert.Single(response.Errors.Where(e => e.Message.Equals("Username is not in valid alphanumeric and hypen format.")).ToList());
                Assert.Single(response.Errors.Where(e => e.Message.Equals("User not found.")).ToList());
            }
        }
    }
}