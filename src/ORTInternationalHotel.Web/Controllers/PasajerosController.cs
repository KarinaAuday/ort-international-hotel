using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ORTInternationalHotel.Web.Data;
using ORTInternationalHotel.Web.Models;

namespace ORTInternationalHotel.Web.Controllers
{
    public class PasajerosController : Controller
    {
        private readonly ORTInternationalHotelContext _context;

        public PasajerosController(ORTInternationalHotelContext context)
        {
            _context = context;
        }

        // GET: Pasajeros
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pasajeros.ToListAsync());
        }

        // GET: Pasajeros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasajero = await _context.Pasajeros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pasajero == null)
            {
                return NotFound();
            }

            return View(pasajero);
        }

        // GET: Pasajeros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pasajeros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Documento,Email,Telefono,PaisOrigen")] Pasajero pasajero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pasajero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pasajero);
        }

        // GET: Pasajeros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasajero = await _context.Pasajeros.FindAsync(id);
            if (pasajero == null)
            {
                return NotFound();
            }
            return View(pasajero);
        }

        // POST: Pasajeros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Documento,Email,Telefono,PaisOrigen")] Pasajero pasajero)
        {
            if (id != pasajero.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pasajero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PasajeroExists(pasajero.Id))
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
            return View(pasajero);
        }

        // GET: Pasajeros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pasajero = await _context.Pasajeros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pasajero == null)
            {
                return NotFound();
            }

            return View(pasajero);
        }

        // POST: Pasajeros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pasajero = await _context.Pasajeros.FindAsync(id);
            if (pasajero != null)
            {
                _context.Pasajeros.Remove(pasajero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PasajeroExists(int id)
        {
            return _context.Pasajeros.Any(e => e.Id == id);
        }
    }
}
