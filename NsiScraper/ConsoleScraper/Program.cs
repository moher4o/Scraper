using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
            //LoadContent();
        }

        static void LoadContent()
        {
            string globalPath = Environment.CurrentDirectory;

            string urlAddress = "https://pochivka.bg/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();
                var targetPath = Path.Combine(globalPath, "pageHtml.html");

                using (var targetStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(targetStream);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(data);
                }


                System.Diagnostics.Process.Start("Chrome", targetPath);

                response.Close();
                readStream.Close();
            }
        }


        async static Task MainAsync(string[] args)
        {
            //Console.WriteLine("Въведете името на файла съдуржащ Json:");
            //var fileWithJson = Console.ReadLine();
            var fileWithJson = "po4ivkaSitemap.txt";
            string globalPath = Environment.CurrentDirectory;
            var filePath = Path.Combine(globalPath, fileWithJson);
            Console.WriteLine(filePath);

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] imagen = new byte[stream.Length];
                await stream.ReadAsync(imagen, 0, (int)stream.Length);
                string result = System.Text.Encoding.UTF8.GetString(imagen);
                var resource = JObject.Parse(result);
                var sitemap = Path.Combine(globalPath, "sitemap.txt");
                using (var sitemapStream = new FileStream(sitemap, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sitemapWriter = new StreamWriter(sitemapStream);
                    sitemapWriter.BaseStream.Seek(0, SeekOrigin.End);
                    


                    foreach (var property in resource.Properties())
                    {
                        Console.WriteLine("{0} - {1}", property.Name, property.Value);
                        await sitemapWriter.WriteLineAsync("{"+ property.Name+"} - {"+property.Value+"}");

                        if (property.Name == "selectors")
                        {
                            foreach (var element in property.Value)
                            {
                                Console.WriteLine("1");
                            }
                        }

                        if (property.Name == "startUrl")
                        {
                            Console.WriteLine("{0} - {1}", property.Name, property.Value);
                            string path = property.Value.ToString();
                            var firstIndex = path.IndexOf("\"");
                            path = path.Substring(firstIndex + 1, path.LastIndexOf("\"") - firstIndex - 1);

                            //HttpClient client = new HttpClient();
                            //var response = await client.GetAsync(path);
                            //var pageContents = await response.Content.ReadAsStringAsync();

                            HtmlDocument pageDocument = new HtmlDocument();
                            var web = new HtmlWeb();
                            pageDocument = web.Load(path);
                            //Console.WriteLine(pageDocument.Text);
                            //pageDocument.LoadHtml(pageContents);
                            var targetPath = Path.Combine(globalPath, "pageHtml.html");
                            using (var targetStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                            {
                                StreamWriter sw = new StreamWriter(targetStream);
                                sw.BaseStream.Seek(0, SeekOrigin.End);
                                await sw.WriteLineAsync(pageDocument.Text);
                            }

                            //System.Diagnostics.Process.Start("Chrome", Uri.EscapeDataString(targetPath));
                            //System.Diagnostics.Process.Start("Chrome", path);
                            var nodes = pageDocument.DocumentNode.SelectNodes("//div[@class='title']/a")
                                         //.Select(y => y.Descendants()
                                         //.Where(x => x.Attributes["class"].Value == "sr-hotel__name")
                                         .ToList();

                            foreach (var node in nodes)
                            {
                                if (node.Attributes.Contains("title"))
                                {
                                    Console.WriteLine(node.InnerText);
                                    //Console.WriteLine(node.Attributes["class"].Value);
                                    //if (node.Attributes["title"].Value.Contains("bui-card__title"))
                                    //{
                                    //    Console.WriteLine(node.InnerText);
                                    //}
                                }
                            }


                            //var hotelName = pageDocument.DocumentNode.SelectSingleNode("(//span[contains(@class,'sr-hotel__name')])[1]").InnerText;
                            //if (hotelName != null)
                            //{
                            //    Console.WriteLine(hotelName);
                            //}
                            //else
                            //{
                            //    Console.WriteLine("NULL");
                            //}
                            //Console.WriteLine(pageDocument.DocumentNode.SelectSingleNode("//(a[contains(@class, 'hotel_name_link')]//span)[1]").InnerText);
                            //Console.WriteLine(pageDocument.DocumentNode.SelectSingleNode("//span[1]").InnerText);
                            //Console.ReadLine();

                        }
                        //Console.WriteLine("{0} - {1}", property.Name, property.Value);


                    }
                }
            }


        }
    }
}
