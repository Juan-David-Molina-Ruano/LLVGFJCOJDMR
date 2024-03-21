using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LLVGFJCOJDMR.Models;

namespace LLVGFJCOJDMR.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            return _context.Customers != null ?
                        View(await _context.Customers.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
              .Include(s => s.PhoneNumbers)
              .FirstAsync(s => s.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.Accion = "Details";
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            var customer = new Customer();
            customer.PhoneNumbers = new List<PhoneNumber>();
            customer.PhoneNumbers.Add(new PhoneNumber
            {
                NumberPhone = "",
                Note = ""
            });
            ViewBag.Accion = "Create";
            return View(customer);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumbers")] Customer customer)
        {
            _context.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #region Detalles
        public ActionResult AgregarDetalles([Bind("Id,FirstName,LastName,Email,PhoneNumbers")] Customer customer, string accion)
        {
            customer.PhoneNumbers.Add(new PhoneNumber
            {
                NumberPhone = "",
                Note = ""
            });
            ViewBag.Accion = accion;
            return View(accion, customer);
        }

        public ActionResult EliminarDetalles([Bind("Id,FirstName,LastName,Email,PhoneNumbers")] Customer customer, string accion, int index)
        {
            var det = customer.PhoneNumbers[index];
            if (accion == "Edit" && det.Id > 0)
            {
                det.Id = det.Id * -1;
            }
            else
            {
                customer.PhoneNumbers.RemoveAt(index);
            }

            ViewBag.Accion = accion;
            return View(accion, customer);
        }
        #endregion

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
              .Include(s => s.PhoneNumbers)
              .FirstAsync(s => s.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.Accion = "Edit";
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumbers")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            try
            {
                // Obtener los datos de la base de datos que van a ser modificados
                var customerUpdate = await _context.Customers
                        .Include(s => s.PhoneNumbers)
                        .FirstAsync(s => s.Id == customer.Id);
                customerUpdate.FirstName = customer.FirstName;
                customerUpdate.LastName = customer.LastName;
                customerUpdate.Email = customer.Email;

                // Obtener todos los detalles que seran nuevos y agregarlos a la base de datos
                var detNew = customer.PhoneNumbers.Where(s => s.Id == 0);
                foreach (var d in detNew)
                {
                    customerUpdate.PhoneNumbers.Add(d);
                }
                // Obtener todos los detalles que seran modificados y actualizar a la base de datos
                var detUpdate = customer.PhoneNumbers.Where(s => s.Id > 0);
                foreach (var d in detUpdate)
                {
                    var det = customerUpdate.PhoneNumbers.FirstOrDefault(s => s.Id == d.Id);
                    det.NumberPhone = d.NumberPhone;
                    det.Note = d.Note;
                }
                // Obtener todos los detalles que seran eliminados y actualizar a la base de datos
                var delDetIds = customer.PhoneNumbers.Where(s => s.Id < 0).Select(s => -s.Id).ToList();
                if (delDetIds != null && delDetIds.Count > 0)
                {
                    foreach (var detalleId in delDetIds) // Cambiado de 'id' a 'detalleId'
                    {
                        var det = await _context.PhoneNumbers.FindAsync(detalleId); // Cambiado de 'id' a 'detalleId'
                        if (det != null)
                        {
                            _context.PhoneNumbers.Remove(det);
                        }
                    }
                }
                // Aplicar esos cambios a la base de datos
                _context.Update(customerUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
