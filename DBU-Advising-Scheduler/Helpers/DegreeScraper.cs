using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace DBU_Advising_Scheduler.Helpers
{
    public static class DegreeScraper
    {
        private static string degreePlanUrl = "https://www2.dbu.edu/orientation-guidebook-degree-plans/";
        private static async Task<string> GetDegreeHTML()
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(degreePlanUrl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var degrees = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"degrees\"]");

            return degrees.OuterHtml;
        }

        public static async Task<string> GetDegreeTable(string degree)
        {
            var html = await GetDegreeHTML();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlBody = htmlDocument.DocumentNode;

            var a = "";
            foreach (HtmlNode item in htmlBody.Descendants("li"))
            {
                var link = item.Descendants("a").First();
                var url = link.GetAttributeValue("href", null);
                var text = link.InnerText.Trim();

                if(text.Equals(degree))
                {
                    a += url;
                }
            }

            var degreeUrl = degreePlanUrl + a;
            var table = await GenerateTable(degreeUrl);

            return table;
        }
        private static async Task<string> GenerateTable(string url)
        {
            string degreeUrl = url;

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var plan = new HtmlDocument();

            StringBuilder tables = new StringBuilder();

            foreach (HtmlNode table in htmlDocument.DocumentNode.SelectNodes("//table"))
            {
                HtmlNode individualTable = HtmlNode.CreateNode(table.OuterHtml);
                tables.Append(individualTable.OuterHtml);
            }

            string tableString = String.Format(@"<html>
                                                 <head>
                                                 </head>
                                                 <body><div>{0}</div></body>
                                                 </html>", tables.ToString());

            plan.LoadHtml(tableString);
            return plan.DocumentNode.OuterHtml;
        }
    }
}