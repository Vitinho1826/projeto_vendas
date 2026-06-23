using System.ComponentModel.DataAnnotations;

namespace VendinhaPlena.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome obrigatório")]
    [StringLength(100)]
    public string NomeCompleto { get; set; }

    [Required(ErrorMessage = "CPF obrigatório")]
    [StringLength(11)]
    public string CPF { get; set; }

    [Required]
    public DateTime DataNascimento { get; set; }

    [EmailAddress]
    public string Email { get; set; }



    public int Idade
    {
        get
        {
            var hoje = DateTime.Today;

            var idade = hoje.Year - DataNascimento.Year;

            if (DataNascimento.Date > hoje.AddYears(-idade))
            {
                idade--;
            }

            return idade;
        }
    }

    public virtual void PrintDados()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Nome: {NomeCompleto}");
        Console.WriteLine($"CPF: {CPF}");
        Console.WriteLine($"Nascimento: {DataNascimento:dd/MM/yyyy}");
        Console.WriteLine($"Idade: {Idade}");
        Console.WriteLine($"Email: {Email}");
    }
}