using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parking.Models
{
    public class PriceTable
    {
        public int Id { get; set; }
        [DisplayName("Data de Inicio da Vigência")]
        [Required(ErrorMessage = "Data Inicial da Vigência")]
        public DateTime InicioVigencia { get; set; }
        [DisplayName("Data Final da Vigência")]
        [Required(ErrorMessage = "Data Final da Vigência")]
        public DateTime FimVigencia { get; set; }
        [DisplayName("Valor da Hora Inicial")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Valor da Primeira hora")]
        public Double ValorHoraInical { get; set; }
        [DisplayName("Valor da Hora Adicional")]
        [DisplayFormat(DataFormatString = "{0:C}",ApplyFormatInEditMode =true)]
        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "Informe o valor para a hora adicional")]
        public Double ValorHoraAdicional { get; set; }
    }
}
