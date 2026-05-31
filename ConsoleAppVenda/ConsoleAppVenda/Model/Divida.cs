using System.ComponentModel.DataAnnotations;
using VendinhaPlena.Enums;

namespace VendinhaPlena.Models;

public class Divida
{
    public int Id { get; set; }

    [Required]
    public Cliente Cliente { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    public SituacaoDivida Situacao { get; set; }

    public DateTime DataCriacao { get; private set; }

    public DateTime? DataPagamento { get; set; }

    public Divida()
    {
        DataCriacao = DateTime.Now;
        Situacao = SituacaoDivida.Aberta;
    }
}