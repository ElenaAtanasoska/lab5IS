using AdminApplication.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace AdminApplication.Controllers
{
    public class ConcertsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportConcerts(IFormFile file)
        {
            // make a copy
            string pathToUpload = $@"{Directory.GetCurrentDirectory()}\files\{file.FileName}";

            using (FileStream stream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(stream);

                stream.Flush();
            }

            // read data from copy file
            List<Concert> concerts = getConcertsFromFile(file.FileName);

            HttpClient client = new HttpClient();
            const string URL = "http://localhost:5054/api/Admin/ImportAllConcerts";

            HttpContent content =
                new StringContent(JsonConvert.SerializeObject(concerts), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<bool>().Result;

            return RedirectToAction("Index", "Order");

        }

        private List<Concert> getConcertsFromFile(string fileName)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
            List<Concert> concerts = new List<Concert>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        concerts.Add(new Concert
                        {
                            ConcertName = reader.GetValue(0).ToString(),
                            ConcertDescription = reader.GetValue(1).ToString(),
                            ConcertImage = reader.GetValue(2).ToString(),
                            Rating = double.Parse(reader.GetValue(3).ToString())
                        });
                    }
                }
            }

            return concerts;
        }
    }
}
