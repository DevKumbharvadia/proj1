using AppAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyerInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuyerInfoController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
