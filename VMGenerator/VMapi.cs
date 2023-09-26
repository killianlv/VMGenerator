using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace VMGenerator
{
    public class VMapi
    {

        public static string apiKey = "TODO";
        private static string serverRegion = "cdg";
        private static string serverPlan = "vc2-2c-4gb"; // Par exemple, "201" pour un plan 1GB RAM
        private static string imqgeID = "NodeJS"; // Par exemple, "387" pour Ubuntu 20.04

        public async static Task CreateVM(string hostName, string repoUrl)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.vultr.com/v2/instances");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");


            var vmData = new VMRequestData
            {
                region = serverRegion,
                plan = serverPlan,
                hostname = hostName,
                image_id = imqgeID,
            };


            var jsonContent = vmData.ToJson();
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            var responseContent = await response.Content.ReadAsStringAsync();
            // Désérialiser la réponse JSON en utilisant un objet dynamique
            dynamic vmResponse = JsonConvert.DeserializeObject(responseContent);

            // Récupérer l'ID de la VM
            string vmId = vmResponse.instance.id;
            string password = vmResponse.instance.default_password;

            Console.WriteLine($"VM créée avec l'ID : {vmId}");

            var host = await VMValidator(vmId);


            //TEST SSH
            //var password = "6sF@q5wgd]$mZ5Lg";
            //var host = "45.76.47.25";
            Console.WriteLine(host);

            await VMssh.ExecuteRemoteCommands(host, "root", password, repoUrl);

        }


        public async static Task<string> VMValidator(string VMid)
        {

            
            while (true)
            {
                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.vultr.com/v2/instances/{VMid}");
                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                // Désérialiser la réponse JSON en utilisant un objet dynamique
                dynamic vmResponse = JsonConvert.DeserializeObject(responseContent);

                // Récupérer l'ID de la VM
                string server_status = vmResponse.instance.server_status;
                Console.WriteLine(server_status);
                if (server_status == "ok")
                {
                    Console.WriteLine("VM is ready");
                    return vmResponse.instance.main_ip;
                }
                else
                {   
                    client.Dispose();
                    Console.WriteLine("VM is not ready yet");
                    await Task.Delay(30000);
                }
            }

        }
    }
}
