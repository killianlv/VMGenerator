using Renci.SshNet;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace VMGenerator
{
    public class VMssh
    {
        public static async Task ExecuteRemoteCommands(string host, string username, string password, string repoUrl)
        {
            SshClient cSSH = new SshClient(host, username, password);

            // Extraire le nom du répertoire à partir de l'URL GitHub
            Uri uri = new Uri(repoUrl);
            string repoName = Path.GetFileNameWithoutExtension(uri.LocalPath);
            Console.WriteLine($"Nom du répertoire : {repoName}");

            cSSH.Connect();
            var cmd = cSSH.CreateCommand($"pwd ; mkdir project ; cd project; apt-get update; apt-get install git; git clone {repoUrl}; cd {repoName}; ls; npm i; ufw allow 3000/tcp; ufw allow 5000/tcp; npm run dev");

            var result = cmd.BeginExecute();

            using (var reader = new StreamReader(
                                  cmd.OutputStream, Encoding.UTF8, true, 1024, true))
            {
                while (!result.IsCompleted || !reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        Console.WriteLine(line + Environment.NewLine);
                    }
                }
            }

            cmd.EndExecute(result);

        }
    }
}
