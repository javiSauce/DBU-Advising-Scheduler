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
    public class DegreeScraper
    {
        private async Task<string> GetDegreeHTML()
        {

            //HtmlWeb httpClient = new HtmlWeb();
            //HtmlDocument htmlDocument = await Task.Run(() => httpClient.Load(degreePlanUrl));

            //string degreePlanUrl = "https://www2.dbu.edu/orientation-guidebook-degree-plans/";

            var url = new Uri("https://www2.dbu.edu/orientation-guidebook-degree-plans/");
            var httpClient = new HttpClient();

            try
            {
                var html = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                var degrees = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"degrees\"]");

                return degrees.OuterHtml;
            }
            catch (Exception e)
            {
                string checkResult = "Error " + e.ToString();
                httpClient.Dispose();
                return checkResult;
            }

            //var html = httpClient.GetStringAsync(degreePlanUrl);

            //var htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(html);
            //var degrees = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"degrees\"]");

            //return degrees.OuterHtml;
        }

        public async Task<string> GetDegreeTable(string degree)
        {

            string degreePlanUrl = "https://www2.dbu.edu/orientation-guidebook-degree-plans/";
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
        private async Task<string> GenerateTable(string url)
        {
            //string degreeUrl = url;

            //var httpClient = new HttpClient();
            //var html = await httpClient.GetStringAsync(url);

            //var htmlDocument = new HtmlDocument();
            //htmlDocument.LoadHtml(html);

            var degreePlanUrl = new Uri(url);
            var httpClient = new HttpClient();

            try
            {
                var html = await httpClient.GetStringAsync(degreePlanUrl);
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
            catch (Exception e)
            {
                string checkResult = "Error " + e.ToString();
                httpClient.Dispose();
                return checkResult;
            }

            //var plan = new HtmlDocument();

            //StringBuilder tables = new StringBuilder();

            //foreach (HtmlNode table in htmlDocument.DocumentNode.SelectNodes("//table"))
            //{
            //    HtmlNode individualTable = HtmlNode.CreateNode(table.OuterHtml);
            //    tables.Append(individualTable.OuterHtml);
            //}

            //string tableString = String.Format(@"<html>
            //                                     <head>
            //                                     </head>
            //                                     <body><div>{0}</div></body>
            //                                     </html>", tables.ToString());

            //plan.LoadHtml(tableString);
            //return plan.DocumentNode.OuterHtml;
        }
    }
}