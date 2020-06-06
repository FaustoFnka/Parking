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
    public class VehicleControlsController : Controller
    {
        private readonly ParkingContext _context;

        public VehicleControlsController(ParkingContext context)
        {
            _context = context;
        }

        // GET: VehicleControls
        public ViewResult Index(string searchString)
        {
            var vehicleControls = from s in _context.VehicleControl
                                  select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                vehicleControls = vehicleControls.Where(s => s.Placa.Contains((searchString)));
            }

            return View(vehicleControls.ToList());
        }


        // GET: VehicleControls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleControl = await _context.VehicleControl
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleControl == null)
            {
                return NotFound();
            }

            return View(vehicleControl);
        }

        // GET: VehicleControls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VehicleControls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Placa,HoraEntrada,HoraSaida,Duracao,QtdHorasCobradas,ValorHora")] VehicleControl vehicleControl)
        {
            if (ModelState.IsValid)
            {
                vehicleControl.HoraSaida = vehicleControl.HoraEntrada;

                _context.Add(vehicleControl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicleControl);
        }

        // GET: VehicleControls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleControl = await _context.VehicleControl.FindAsync(id);
            if (vehicleControl == null)
            {
                return NotFound();
            }
            return View(vehicleControl);
        }

        // POST: VehicleControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Placa,HoraEntrada,HoraSaida,Duracao,QtdHorasCobradas,ValorHora")] VehicleControl vehicleControl)
        {
            if (id != vehicleControl.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(vehicleControl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleControlExists(vehicleControl.Id))
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
            return View(vehicleControl);
        }
        // GET: VehicleControls/Edit/5
        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleControl = await _context.VehicleControl.FindAsync(id);

            if (vehicleControl == null)
            {
                return NotFound();
            }

            if (vehicleControl.HoraEntrada.Equals(vehicleControl.HoraSaida))
            {
                //Marca valor para saida agora.
                var nowDate = DateTime.Now;

                nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, nowDate.Second);

                vehicleControl.HoraSaida = nowDate;
                
                
                TimeSpan Duracao = vehicleControl.HoraSaida - vehicleControl.HoraEntrada;
                var totalHoras2 = Duracao.Hours;
                var totalmin = Duracao.Minutes;

                Double horasAPagar;

                if (totalHoras2 < 1)
                {
                    //Menos de 1 hora
                    //Calcular Qts minutos e Hora parcial
                    if (totalmin <= 30)
                    {
                        horasAPagar =+ 0.5;
                    }
                    else
                    {
                        horasAPagar = +1;
                    }
                }
                else
                {
                    if (totalmin > 10)
                    {
                        horasAPagar = totalHoras2 + 1;
                    }
                    else
                    {
                        horasAPagar = totalHoras2;
                    }
                }

                vehicleControl.QtdHorasCobradas = horasAPagar;

                var pricevalues = from priceRow in _context.PriceTable
                                      select priceRow;

                pricevalues = pricevalues.Where(priceRow => priceRow.InicioVigencia <= vehicleControl.HoraEntrada && priceRow.FimVigencia >= vehicleControl.HoraSaida);

                double valorHoraInicial = pricevalues.FirstOrDefault().ValorHoraInical;
                double valorHoraAdicional = pricevalues.FirstOrDefault().ValorHoraAdicional;

                ViewBag.totalAPagar = new Double();

                ViewBag.totalAPagar = (1 * valorHoraInicial + (horasAPagar - 1) * valorHoraAdicional);

                vehicleControl.ValorHora = valorHoraAdicional;
                vehicleControl.Duracao = Duracao.ToString();
                }

            return View(vehicleControl);
        }

        /*public DateTime TruncateToSecondStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }*/

        // POST: VehicleControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int id, [Bind("Id,Placa,HoraEntrada,HoraSaida,Duracao,QtdHorasCobradas,ValorHora")] VehicleControl vehicleControl)
        {
            if (id != vehicleControl.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(vehicleControl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleControlExists(vehicleControl.Id))
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
            return View(vehicleControl);
        }

        // GET: VehicleControls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicleControl = await _context.VehicleControl
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicleControl == null)
            {
                return NotFound();
            }

            return View(vehicleControl);
        }

        // POST: VehicleControls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicleControl = await _context.VehicleControl.FindAsync(id);
            _context.VehicleControl.Remove(vehicleControl);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleControlExists(int id)
        {
            return _context.VehicleControl.Any(e => e.Id == id);
        }
    }
}
