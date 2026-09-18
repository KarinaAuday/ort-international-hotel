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
    public class ReservasController : Controller
    {
        private readonly ORTInternationalHotelContext _context;

        public ReservasController(ORTInternationalHotelContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var oRTInternationalHotelContext = _context.Reservas.Include(r => r.Habitacion).Include(r => r.Hotel).Include(r => r.Pasajero);
            return View(await oRTInternationalHotelContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Hotel)
                .Include(r => r.Pasajero)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero");
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Ciudad");
            ViewData["PasajeroId"] = new SelectList(_context.Pasajeros, "Id", "Apellido");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaDesde,FechaHasta,Estado,MontoTotal,PasajeroId,HotelId,HabitacionId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Ciudad", reserva.HotelId);
            ViewData["PasajeroId"] = new SelectList(_context.Pasajeros, "Id", "Apellido", reserva.PasajeroId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Ciudad", reserva.HotelId);
            ViewData["PasajeroId"] = new SelectList(_context.Pasajeros, "Id", "Apellido", reserva.PasajeroId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaDesde,FechaHasta,Estado,MontoTotal,PasajeroId,HotelId,HabitacionId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["HabitacionId"] = new SelectList(_context.Habitaciones, "Id", "Numero", reserva.HabitacionId);
            ViewData["HotelId"] = new SelectList(_context.Hoteles, "Id", "Ciudad", reserva.HotelId);
            ViewData["PasajeroId"] = new SelectList(_context.Pasajeros, "Id", "Apellido", reserva.PasajeroId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Habitacion)
                .Include(r => r.Hotel)
                .Include(r => r.Pasajero)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
