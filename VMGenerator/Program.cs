using CommandLine;
using CliWrap;
using System.Text.RegularExpressions;

namespace VMGenerator
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            //var repoUrl = "https://github.com/johnpapa/node-hello";

            return await Parser.Default.ParseArguments<Options>(args)
                .MapResult(async (Options opts) =>
                {
                    if (opts.RepoUrl != null && IsValidUrl(opts.RepoUrl))
                    {
                        Console.WriteLine($"Création de la VM pour Repo URL : {opts.RepoUrl}");
                        Console.WriteLine($"Exécution de la commande : {opts.CommandToRun}");
                        await VMapi.CreateVM("myMV", opts.RepoUrl, opts.CommandToRun);
                    }
                    else
                    {
                        Console.WriteLine("URL du repo non valide.");
                    }

                    return 0; // Succès
                },
                errs => Task.FromResult(1)); // Échec
        }

        public class Options
        {
            [Option('r', "repo", Required = true, HelpText = "Url du repo")]
            public string? RepoUrl { get; set; }

            [Option('c', "command", Required = false, Default = "npm start", HelpText = "Commande à exécuter après le clonage")]
            public string CommandToRun { get; set; }

        }

        // Fonction pour vérifier si une chaîne est une URL valide
        private static bool IsValidUrl(string url)
        {
            // Utilisez une expression régulière pour vérifier si la chaîne ressemble à une URL
            string pattern = @"^(https?|ftp)://[^\s/$.?#].[^\s]*$";
            return Regex.IsMatch(url, pattern);
        }
    }


}