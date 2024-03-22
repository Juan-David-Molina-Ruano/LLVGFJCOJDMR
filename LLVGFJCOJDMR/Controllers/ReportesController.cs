using LLVGFJCOJDMR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace LLVGFJCOJDMR.Controllers
{
    public class ReportesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GenerarReporte(string firstName, string lastName, string opcion)
        {
            var customers = await _context.Customers.Include(c => c.PhoneNumbers).Where(c => c.FirstName == firstName).ToListAsync();
            if (customers.Count == 0)
            {
                ViewBag.ErrorMessage = "No se encontraron ventas para el cliente proporcionado.";
                return View("Index");
            }
            else if (opcion == "PDF")
            {
                return new ViewAsPdf("GenerarReporte", customers);
            }
            else
            {
                return View("Index");
            }
        }

    }
}
