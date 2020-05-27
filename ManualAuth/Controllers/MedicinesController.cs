using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ManualAuth.Data;
using ManualAuth.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using MailKit.Net.Smtp;

namespace ManualAuth.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicinesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // public void Med()
        //{
        //    List<Medicine> lista = _context.Medicine.ToList();

        //    foreach (var item in _context.Medicine.ToList())
        //    {
        //        var message = new MimeMessage();
        //        message.From.Add(new MailboxAddress("Async Email Service", "test.app.kd@gmail.com"));
        //        message.To.Add(new MailboxAddress("To Add", "krzysztofwozniak1234@gmail.com"));
        //        message.Subject = "Medicine Reminder";
        //        message.Body = new TextPart("plain")
        //        {
        //            Text = "This is the message from your application."
        //        };
        //        using (var client = new SmtpClient())
        //        {
        //            client.Connect("smtp.gmail.com", 587, false);
        //            client.Authenticate("test.app.kd@gmail.com", "Application_1");
        //            client.Send(message);
        //            client.Disconnect(true);
        //        }

        //    }
        //}



        // GET: Medicines
        public async Task<IActionResult> Index()
        {


            var lista = (await _context.Medicine.ToListAsync());
            List<Medicine> medicines = new List<Medicine>();

            foreach (var item in lista)
            {
                if (item.Id_patient == HttpContext.User.Identity.Name)
                {
                    medicines.Add(item);
                }
            }



            //return View(await _context.Medicine.ToListAsync());

            return View(medicines);

        }

        // GET: Medicines/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicine
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // GET: Medicines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicines/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Id_doctor,Day, HourOfTaking, MinuteOfTaking")] Medicine medicine)
        {
            if (ModelState.IsValid)
            {

                bool[] daytable = new bool[8];



                //medicine.Id_patient = "CUSTOM";
                //tutaj proba pobrania ID pacjenta z bazy i zapisania jako id_patient w medicine

                if (HttpContext.User.Identity.Name==null)
                {
                    //TODO: TU TRZEBA ZAŁAĆ DZIURĘ I ZROBIĆ TAK, ŻEBY TO NIE BYŁO WIDOCZNE JAK SIĘNIE JEST ZALOGOWANYM
                }
                else
                {
                    medicine.Id_patient = HttpContext.User.Identity.Name;
                }


                _context.Add(medicine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicine);
        }

        // GET: Medicines/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicine.FindAsync(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View(medicine);
        }

        // POST: Medicines/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Id_patient,Id_doctor,Day")] Medicine medicine)
        {
            if (id != medicine.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicineExists(medicine.Id))
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
            return View(medicine);
        }

        // GET: Medicines/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicine = await _context.Medicine
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medicine = await _context.Medicine.FindAsync(id);
            _context.Medicine.Remove(medicine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(string id)
        {
            return _context.Medicine.Any(e => e.Id == id);
        }
    }
}
