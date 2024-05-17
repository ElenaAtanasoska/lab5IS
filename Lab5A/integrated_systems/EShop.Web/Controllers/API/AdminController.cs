using EShop.Domain.Domain;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Movie_App.Service.Interface;

namespace EShop.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConcertService _concertService;

        public AdminController(IOrderService orderService, IConcertService concertService)
        {
            _orderService = orderService;
            _concertService = concertService;
        }

        [HttpGet("[action]")]
        public List<Order> GetAllOrders()
        {
            return this._orderService.GetAllOrders();
        }
        [HttpPost("[action]")]
        public Order GetDetailsForOrder(BaseEntity id)
        {
            var data = this._orderService.GetDetailsForOrder(id);
            return data;
        }

        [HttpPost("[action]")]
        public void ImportAllConcerts(List<Concert> model)
        {
            foreach (var item in model)
            {
                var concert = new Concert
                {
                    Id = Guid.NewGuid(),
                    ConcertName = item.ConcertName,
                    ConcertDescription = item.ConcertDescription,
                    ConcertImage = item.ConcertImage,
                    Rating = item.Rating,
                    Tickets = new List<Ticket>()
                };

                _concertService.CreateNewConcert(concert);
            }
        }

    }
}
