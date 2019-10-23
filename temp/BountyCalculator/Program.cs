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
        static void Main(string[] args)
        {
            List<IBounty> bounties = GetBounties();
            Dictionary<Location, IBounty> prioritizedBounties = PrioritizedBounties(bounties);
        }

        private static List<IBounty> GetBounties()
        {
            ApiClient client = new ApiClient();
            string authorizationUrl = client.GetAuthorizationUrl();

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
