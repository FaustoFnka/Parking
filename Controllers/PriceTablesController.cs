using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Parking.Models;

namespace Parking.Controllers
{
    public class PriceTablesController : Controller
    {
        private readonly ParkingContext _context;

        public PriceTablesController(ParkingContext context)
        {
            _context = context;
        }

        // GET: PriceTables
        public async Task<IActionResult> Index()
        {
            return View(await _context.PriceTable.ToListAsync());
        }

        // GET: PriceTables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceTable = await _context.PriceTable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceTable == null)
            {
                return NotFound();
            }

            return View(priceTable);
        }

        // GET: PriceTables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PriceTables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InicioVigencia,FimVigencia,ValorHoraInical,ValorHoraAdicional")] PriceTable priceTable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceTable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priceTable);
        }

        // GET: PriceTables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceTable = await _context.PriceTable.FindAsync(id);
            if (priceTable == null)
            {
                return NotFound();
            }
            return View(priceTable);
        }

        // POST: PriceTables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InicioVigencia,FimVigencia,ValorHoraInical,ValorHoraAdicional")] PriceTable priceTable)
        {
            if (id != priceTable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceTable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceTableExists(priceTable.Id))
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
            return View(priceTable);
        }

        // GET: PriceTables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceTable = await _context.PriceTable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceTable == null)
            {
                return NotFound();
            }

            return View(priceTable);
        }

        // POST: PriceTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceTable = await _context.PriceTable.FindAsync(id);
            _context.PriceTable.Remove(priceTable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceTableExists(int id)
        {
            return _context.PriceTable.Any(e => e.Id == id);
        }
    }
}
