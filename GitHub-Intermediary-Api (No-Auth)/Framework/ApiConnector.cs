﻿using GitHub_Intermediary_Api.Models;
using Newtonsoft.Json;

namespace GitHub_Intermediary_Api.Framework {
    public class ApiConnector() {
        public async Task<User?> RetrieveUsersAsync(string username) {
            try {
                using (HttpClient client = new()) {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                    client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");

                    HttpResponseMessage response = await client.GetAsync($"https://api.github.com/users/{username}");
                    if (response.IsSuccessStatusCode) {
                        if (response.Content != null) {
                            string jsonResponse = await response.Content.ReadAsStringAsync();
                            return JsonConvert.DeserializeObject<User>(jsonResponse);
                        } 
                    } 
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error Message: {ex.Message}");
            }
            return null;
        }
    }
}
