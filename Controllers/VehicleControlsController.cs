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
          
            //irá filtrar os dados caso tenha sido digitado filtro.
            if (!String.IsNullOrEmpty(searchString))
            {
                vehicleControls = vehicleControls.Where(s => s.Placa.Contains(searchString.ToUpper()));
            }

            //Irá apresentar primeiro os veículos que chegaram por último
            vehicleControls = vehicleControls.OrderBy(s => s.HoraSaida);

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

            if (vehicleControl.QtdHorasCobradas == null)
            {
                vehicleControl.HoraSaida = vehicleControl.HoraEntrada;
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
                //Marca valor para saida do veículo igual agora.
                var nowDate = DateTime.Now;
                nowDate = new DateTime(nowDate.Year, nowDate.Month, nowDate.Day, nowDate.Hour, nowDate.Minute, 0);

                //Configura os valores de Hora Inicial e Adicional apartir da primeira tabela de preços vigente recuperada.
                Decimal valorHoraInicial = 0;
                Decimal valorHoraAdicional = 0;

                //Recupera a primeira tabela de preço vigente baseado na data/hora de entrada do veículo.
                try
                {
                    var pricevalues = from priceRow in _context.PriceTable
                                      select priceRow;

                    pricevalues = pricevalues.Where(priceRow => priceRow.InicioVigencia <= vehicleControl.HoraEntrada && priceRow.FimVigencia >= vehicleControl.HoraSaida);

                    //Configura os valores de Hora Inicial e Adicional apartir da primeira tabela de preços vigente recuperada.
                    valorHoraInicial = pricevalues.FirstOrDefault().ValorHoraInical;
                    valorHoraAdicional = pricevalues.FirstOrDefault().ValorHoraAdicional;
                }
                catch {
                    //Se não for possível a consulta da tabela de preços irá retornar como não encontrado.
                    return NotFound("Tabela Vigente não encontrada!");
                }

                //Carrega os valores hora da tabela para ser apresentado ao usuário.
                vehicleControl.ValorHora = ((valorHoraAdicional + valorHoraInicial)/2);

                //Configura a hora de saída do veículo como Agora.
                vehicleControl.HoraSaida = nowDate;
                
                //Calcula a duração do estacionamento do veículo (separando Horas e Minutos).
                TimeSpan Duracao = vehicleControl.HoraSaida - vehicleControl.HoraEntrada;
                var totalHoras = Duracao.Hours;
                var totalmin = Duracao.Minutes;

                //Verifica se ficou mais de 24horas
                if (Duracao.Days>0)
                {
                    totalHoras =+ Duracao.Days * 24;
                }

                //Carrega o cálculo de Duração para ser apresentado ao usuário
                vehicleControl.Duracao = Duracao.ToString();

                Decimal horasAPagar;

                if (totalHoras < 1)
                {
                    //Menos de 1 hora
                    //Calcular baseado em minutos para cobrar da hora inicial.
                    if (totalmin <= 30)
                    {
                        horasAPagar = 1/2; //Meia hora caso tenha ficado até 30 min.
                    }
                    else
                    {
                        horasAPagar = 1; // Uma hora cheia se passou de 30 min.
                    }
                }
                else
                {
                    if (totalmin > 10)
                    {
                        horasAPagar = totalHoras + 1; //Se passou de uma hora e passou de 10 minutos de tolerância soma mais uma hora.
                    }
                    else
                    {
                        horasAPagar = totalHoras; // se não passou da tolerância de 10 minutos.
                    }
                }
                
                //Carrega a Quantidade de Horas a cobrar para ser apresentada.
                vehicleControl.QtdHorasCobradas = horasAPagar;

                //Carrega valor a pagar para ser apresentado ao usuario.
                ViewBag.totalAPagar = new Decimal();
                ViewBag.totalAPagar = (1 * valorHoraInicial + (horasAPagar - 1) * valorHoraAdicional);


                }

            return View(vehicleControl);
        }

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
