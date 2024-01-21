using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
internal class Program
{
    static async Task Main(string[] args)
    {
        using (HttpClient client = new HttpClient())
        {
            Console.WriteLine("Veuillez entrer l'url");
            string url = Console.ReadLine();


            // Effectue la requête POST pour accéder à la page protégée
            HttpResponseMessage response = await client.PostAsync(url, null);

            // Vérifie si la requête a réussi
            if (response.IsSuccessStatusCode)
            {
                // Récupère le contenu HTML de la page
                string htmlContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine(htmlContent);

                // Utilise HtmlAgilityPack pour analyser le HTML
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);

                // Extraie et imprime les liens vers les feuilles de style CSS
                var cssLinks = htmlDocument.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
                if (cssLinks != null)
                {
                    foreach (var link in cssLinks)
                    {
                        string cssUrl = link.GetAttributeValue("href", "");

                        // Verifie que l'URL est une URI absolue
                        if (!Uri.TryCreate(new Uri(url), cssUrl, out Uri validCssUrl))
                        {
                            Console.WriteLine($"L'URL CSS spécifiée n'est pas valide : {cssUrl}");
                            continue;
                        }

                        Console.WriteLine($"CSS URL: {validCssUrl}");

                        // Récupère et imprimez le contenu CSS
                        string cssContent = await client.GetStringAsync(validCssUrl);
                        Console.WriteLine($"CSS Content:\n{cssContent}\n");
                    }
                }
                else
                {
                    Console.WriteLine("Aucun lien CSS trouvé sur la page.");
                }
            }
            else
            {
                Console.WriteLine($"La requête a échoué avec le code : {response.StatusCode}");
            }
        }
    }
}