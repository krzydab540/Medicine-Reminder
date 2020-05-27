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
    public class GlucosesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GlucosesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Glucoses
        public async Task<IActionResult> Index()
        {
            var lista = (await _context.Glucoses.ToListAsync());
            List<Glucose> glucoses = new List<Glucose>();

            foreach (var item in lista)
            {
                if (item.Id_patient == HttpContext.User.Identity.Name)
                {
                    glucoses.Add(item);
                }
            }

            return View(glucoses);
        }

        // GET: Glucoses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var glucose = await _context.Glucoses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (glucose == null)
            {
                return NotFound();
            }

            return View(glucose);
        }

        // GET: Glucoses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Glucoses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GlucoseDateTime,GlucoseValue")] Glucose glucose)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.Name == null)
                {
                    //TODO: TU TRZEBA ZAŁAĆ DZIURĘ I ZROBIĆ TAK, ŻEBY TO NIE BYŁO WIDOCZNE JAK SIĘNIE JEST ZALOGOWANYM
                }
                else
                {
                    glucose.Id_patient = HttpContext.User.Identity.Name;
                }

                _context.Add(glucose);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(glucose);
        }

        // GET: Glucoses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var glucose = await _context.Glucoses.FindAsync(id);
            if (glucose == null)
            {
                return NotFound();
            }
            return View(glucose);
        }

        // POST: Glucoses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GlucoseDateTime,GlucoseValue")] Glucose glucose)
        {
            if (id != glucose.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(glucose);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlucoseExists(glucose.Id))
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
            return View(glucose);
        }

        // GET: Glucoses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var glucose = await _context.Glucoses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (glucose == null)
            {
                return NotFound();
            }

            return View(glucose);
        }

        // POST: Glucoses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var glucose = await _context.Glucoses.FindAsync(id);
            _context.Glucoses.Remove(glucose);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlucoseExists(int id)
        {
            return _context.Glucoses.Any(e => e.Id == id);
        }
    }
}
