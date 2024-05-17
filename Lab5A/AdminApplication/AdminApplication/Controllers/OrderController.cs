using AdminApplication.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace AdminApplication.Controllers
{
    public class OrderController : Controller
    {
        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();
            string URL = "http://localhost:5054/api/Admin/GetAllOrders";
            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;
            return View(data);
        }

        public IActionResult Details(Guid Id)
        {
            HttpClient client = new HttpClient();
            string URL = "http://localhost:5054/api/Admin/GetDetailsForOrder";
            var model = new
            {
                Id = Id
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;
            return View(data);
        }

        public IActionResult CreateInvoice(Guid id)
        {
            HttpClient client = new HttpClient();
            string URL = "http://localhost:5054/api/Admin/GetDetailsForOrder";
            var model = new
            {
                Id = id
            };
            HttpContent content =
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var data = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);
            // OrderNumber, UserName, ProductsList, TotalPrice
            document.Content.Replace("{{OrderNumber}}", data.Id.ToString());
            document.Content.Replace("{{UserName}}", data.Owner.FirstName + " " + data.Owner.LastName);

            StringBuilder sb = new StringBuilder();
            var totalPrice = 0.0;

            foreach (var ticket in data.ProductInOrders)
            {
                sb.AppendLine(
                    ticket.OrderedProduct.Concert.ConcertName + " with quantity of: " + ticket.Quantity +
                    " and price of: " + ticket.OrderedProduct.Price + " $"
                );
                totalPrice += ticket.OrderedProduct.Price * ticket.Quantity;
            }

            document.Content.Replace("{{ProductList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString());

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType,
                "ExportInvoice.pdf");
        }
    }
}
