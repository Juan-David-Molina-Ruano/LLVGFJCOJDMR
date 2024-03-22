using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LLVGFJCOJDMR.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LLVGFJCOJDMR.Controllers
{
    [Authorize(Roles = "Administrador, Gerente")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Users.Include(u => u.Rol);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RolId,UserName,Password,Email,Status")] User user , IFormFile? Image)
        {
            int Mb_1 = 1048576;
            if (Image != null && Image.Length < Mb_1) // Guardar Si Tienen Menos de 1 Mb
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Image.CopyToAsync(memoryStream);
                        user.Image = memoryStream.ToArray();
                    }
                }

            }

            //Lista
            var errors = new List<string>();

            //mostrar los errores
            if (errors.Any())
            {
                // Si Tiene mas de 1 Mb y mas errores
                if (Image != null && Image.Length > Mb_1)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                    ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                    return View(user);


                }
                else // si tiene solo errores
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                    return View(user);

                }
            }
            else
            {
                if (Image!= null && Image.Length > Mb_1)
                {
                    TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                    ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                    return View(user);
                }

            }


            
                user.Password = CalcularHashMD5(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
          
            
           
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RolId,UserName,Password,Email,Status")] User user , IFormFile? Image)
        {
           if (id != user.Id)
            {
                return NotFound();
            }

           var errors = new List<string>();

            try
            {

                int Mb_1 = 1048576;
                if (Image != null && Image.Length > Mb_1) // No ejecutar nada si tiene mas de 1 Mb Y MOSTAR ERRORES
                {
                    if (errors.Any()) // Si hay errores y imagen mayor a 1MB
                    {
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                        ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                        return View(user);
                    }
                    else // Si Imagen es mayor a 1MB
                    {
                        TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                        ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                        return View(user);
                    }

                }
                else
                {
                    if (Image != null && Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Image.CopyToAsync(memoryStream);
                            user.Image = memoryStream.ToArray();
                        }

                        if (errors.Any()) // Si Hay Un Error Mando A La Vista
                        {
                            foreach (var error in errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                            return View(user);
                        }

                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (errors.Any()) // Si Hay Un Error Mando A La Vista
                        {
                            foreach (var error in errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                            return View(user);
                        }

                        var UserFind = await _context.Users.FirstOrDefaultAsync(s => s.Id == user.Id);
                        if (UserFind?.Image?.Length > 0)
                            user.Image = UserFind.Image;
                        UserFind.UserName = user.UserName;
                        UserFind.RolId = user.RolId;
                        UserFind.Email = user.Email;
                        UserFind.Status = user.Status;

                        var contra = UserFind.Password;
                        if (contra == user.Password)
                        {
                            UserFind.Password = user.Password;
                        }
                        else
                        {
                            UserFind.Password = CalcularHashMD5(user.Password);
                        }

                        _context.Update(UserFind);
                        await _context.SaveChangesAsync();
                    }

                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // ---- ---- ---- ---- ---- ---- Login ---- ---- ---- ---- ---- ---- 
        // ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- -
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl) //Metodo_1 Vista Login
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }



        [AllowAnonymous]
        [HttpPost] //Metodo_2 Datos De La Vista De Login
        public async Task<IActionResult> Login([Bind("Email,Password")] User user, string ReturnUrl)
        {
            user.Password = CalcularHashMD5(user.Password);
            var usuarioAut = await _context.Users.Include(e => e.Rol).FirstOrDefaultAsync(s => s.Email == user.Email && s.Password == user.Password && s.Status == 1);
            if (usuarioAut?.Id > 0 && usuarioAut.Email == user.Email)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Name,usuarioAut.UserName),
                     new Claim(ClaimTypes.Role, usuarioAut.Rol.Name),
                    new Claim("Id", usuarioAut.Id.ToString())
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = true }); ;
                var result = User.Identity.IsAuthenticated;
                if (!string.IsNullOrWhiteSpace(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                ViewBag.Error = "Credenciales incorrectas";
            ViewBag.pReturnUrl = ReturnUrl;
            return View(user);
        }

        private string CalcularHashMD5(string texto)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Convierte la cadena de texto a bytes
                byte[] inputBytes = Encoding.UTF8.GetBytes(texto);

                // Calcula el hash MD5 de los bytes
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convierte el hash a una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        //Se agrega para ver perfil
        [AllowAnonymous]

        public async Task<IActionResult> Perfil()
        {
            int id = int.Parse(User.FindFirstValue("Id"));
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", usuario.RolId);
            return View(usuario);
        }
        [AllowAnonymous]
        // GET: Users/Edit/5
        public async Task<IActionResult> EditarPerfil(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
            return View(user);
        }

        [AllowAnonymous]
        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPerfil(int id, [Bind("Id,RolId,UserName,Password,Email,Status")] User user, IFormFile? Image)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            var errors = new List<string>();

            try
            {

                int Mb_1 = 1048576;
                if (Image != null && Image.Length > Mb_1) // No ejecutar nada si tiene mas de 1 Mb Y MOSTAR ERRORES
                {
                    if (errors.Any()) // Si hay errores y imagen mayor a 1MB
                    {
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                        TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                        ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                        return View(user);
                    }
                    else // Si Imagen es mayor a 1MB
                    {
                        TempData["MensajeError"] = "Solo se permite imagenes de 1 Mb.";
                        ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                        return View(user);
                    }

                }
                else
                {
                    if (Image != null && Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Image.CopyToAsync(memoryStream);
                            user.Image = memoryStream.ToArray();
                        }

                        if (errors.Any()) // Si Hay Un Error Mando A La Vista
                        {
                            foreach (var error in errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                            return View(user);
                        }

                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (errors.Any()) // Si Hay Un Error Mando A La Vista
                        {
                            foreach (var error in errors)
                            {
                                ModelState.AddModelError("", error);
                            }
                            ViewData["RolId"] = new SelectList(_context.Rols, "Id", "Name", user.RolId);
                            return View(user);
                        }

                        var UserFind = await _context.Users.FirstOrDefaultAsync(s => s.Id == user.Id);
                        if (UserFind?.Image?.Length > 0)
                            user.Image = UserFind.Image;
                        UserFind.UserName = user.UserName;
                        UserFind.RolId = user.RolId;
                        UserFind.Email = user.Email;
                        UserFind.Status = user.Status;

                        var contra = UserFind.Password;
                        if (contra == user.Password)
                        {
                            UserFind.Password = user.Password;
                        }
                        else
                        {
                            UserFind.Password = CalcularHashMD5(user.Password);
                        }

                        _context.Update(UserFind);
                        await _context.SaveChangesAsync();
                    }

                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
