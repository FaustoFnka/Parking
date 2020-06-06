using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parking.Models
{
    public class VehicleControl
    {
        public int Id { get; set; }
        [DisplayName("Placa Veículo")]
        [Required(ErrorMessage = "Informe a Placa do Veículo")]
        public string Placa { get; set; }
        [DisplayName("Data/Hora Entrada")]
        [Required(ErrorMessage = "Informe a Data/Hora de Entrada")]
        public DateTime HoraEntrada { get; set; }
        [DisplayName("Data/Hora Saída")]
        [Required(ErrorMessage = "Informe a Data/Hora de Saída")]
        public DateTime HoraSaida { get; set; }
        [DisplayName("Duração")]
        public string Duracao { get; set; }
        [DisplayName("Quantidade Horas")]
        public Double? QtdHorasCobradas { get; set; }
        [DisplayName("Valor por Hora")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Double? ValorHora { get; set; }
    }
}
