using BountyCalculator.Data;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

namespace BountyCalculator
{
    class Program
    {
        private const string m_baseUri = "https://www.bungie.net";
        private const string m_authUri = "en/oauth/authorize";
 
        private const string m_redirectUri = "https://localhost:45932/";

        private const string m_apiKey = "b944b784b5b3426cac173a27ed20d40a";
        private const string m_oauthClientId = "30304";

        static void Main(string[] args)
        {
            List<IBounty> bounties = GetBounties();
            Dictionary<Location, IBounty> prioritizedBounties = PrioritizedBounties(bounties);
        }

        private static List<IBounty> GetBounties()
        {
            Guid guidA = Guid.NewGuid();
            Guid guidB = Guid.NewGuid();
            string guidString = $"{guidA}_{guidB}";

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(m_baseUri)
            };

            Dictionary<string, string> query = new Dictionary<string, string>
            {
                { "client_id", m_oauthClientId },
                { "response_type", "code" },
                { "state", guidString },
                { "redirect_uri", m_redirectUri },
            };
            HttpResponseMessage response = client.GetAsync(QueryHelpers.AddQueryString(m_authUri, query)).Result;

            string authorizationUrl = response.RequestMessage.RequestUri.ToString();
            Process proc = Process.Start(new ProcessStartInfo("cmd", $"/c start {authorizationUrl}") { CreateNoWindow = true });

            return new List<IBounty>();
        }

        private static Dictionary<Location, IBounty> PrioritizedBounties(List<IBounty> bounties)
        {
            return new Dictionary<Location, IBounty>();
        }

        private static void ShowResults(Dictionary<Location, IBounty> results)
        {
            foreach (var pair in results)
            {
                Console.WriteLine(pair.Value);
            }
        }
    }
}
