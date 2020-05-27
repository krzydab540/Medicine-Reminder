using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManualAuth.Data;
using ManualAuth.Models;

namespace ManualAuth.Controllers
{
    public class PressuresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PressuresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pressures
        public async Task<IActionResult> Index()
        {

            var lista = (await _context.Pressure.ToListAsync());
            List<Pressure> pressures = new List<Pressure>();

            foreach (var item in lista)
            {
                if (item.Id_patient == HttpContext.User.Identity.Name)
                {
                    pressures.Add(item);
                }
            }

            return View(pressures);
        }

        // GET: Pressures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressure = await _context.Pressures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pressure == null)
            {
                return NotFound();
            }

            return View(pressure);
        }

        // GET: Pressures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pressures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PressureDateTime,PressureSYSValue,PressureDIAValue")] Pressure pressure)
        {
            if (ModelState.IsValid)
            {
                pressure.Id_patient = HttpContext.User.Identity.Name;

                _context.Add(pressure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pressure);
        }

        // GET: Pressures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressure = await _context.Pressures.FindAsync(id);
            if (pressure == null)
            {
                return NotFound();
            }
            return View(pressure);
        }

        // POST: Pressures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PressureDateTime,PressureSYSValue,PressureDIAValue")] Pressure pressure)
        {
            if (id != pressure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pressure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PressureExists(pressure.Id))
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
            return View(pressure);
        }

        // GET: Pressures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pressure = await _context.Pressures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pressure == null)
            {
                return NotFound();
            }

            return View(pressure);
        }

        // POST: Pressures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pressure = await _context.Pressures.FindAsync(id);
            _context.Pressures.Remove(pressure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PressureExists(int id)
        {
            return _context.Pressures.Any(e => e.Id == id);
        }
    }
}
