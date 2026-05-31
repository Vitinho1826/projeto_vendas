using VendinhaPlena.Models;
using VendinhaPlena.Services;

var clienteService = new ClienteService();
var dividaService = new DividaService();

while (true)
{
    Console.Clear();

    Console.WriteLine("===== VENDINHA PONTO COM =====");
    Console.WriteLine("1 - Cadastrar Cliente");
    Console.WriteLine("2 - Listar Clientes");
    Console.WriteLine("3 - Buscar Cliente");
    Console.WriteLine("4 - Excluir Cliente");
    Console.WriteLine("5 - Cadastrar Dívida");
    Console.WriteLine("6 - Listar Dívidas");
    Console.WriteLine("7 - Pagar Dívida");
    Console.WriteLine("0 - Sair");

    Console.Write("\nOpção: ");

    int opcao;

    try
    {
        opcao = int.Parse(Console.ReadLine());
    }
    catch
    {
        Console.WriteLine("Opção inválida.");
        Console.ReadKey();
        continue;
    }

    if (opcao == 0)
    {
        break;
    }

    else if (opcao == 1)
    {
        var cliente = new Cliente();

        Console.Write("Nome: ");
        cliente.NomeCompleto = Console.ReadLine();

        Console.Write("CPF: ");
        cliente.CPF = Console.ReadLine();

        Console.Write("Data de nascimento: ");
        cliente.DataNascimento =
            DateTime.Parse(Console.ReadLine());

        Console.Write("Email: ");
        cliente.Email = Console.ReadLine();

        var sucesso =
            clienteService.Criar(
                cliente,
                out var erros
            );

        if (!sucesso)
        {
            foreach (var erro in erros)
            {
                Console.WriteLine(
                    erro.ErrorMessage
                );
            }
        }
        else
        {
            Console.WriteLine(
                "Cliente cadastrado!"
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 2)
    {
        var clientes =
            clienteService.ListarOrdenadoPorDivida();

        foreach (var cliente in clientes)
        {
            cliente.PrintDados();

            Console.WriteLine(
                "----------------------"
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 3)
    {
        Console.Write("ID do cliente: ");

        var id =
            int.Parse(
                Console.ReadLine()
            );

        var cliente =
            clienteService.Buscar(id);

        if (cliente == null)
        {
            Console.WriteLine(
                "Cliente não encontrado."
            );
        }
        else
        {
            cliente.PrintDados();

            Console.WriteLine(
                $"Total devido: R$ {dividaService.TotalDividasCliente(cliente.Id):F2}"
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 4)
    {
        Console.Write("ID do cliente: ");

        var id =
            int.Parse(
                Console.ReadLine()
            );

        var sucesso =
            clienteService.Excluir(id);

        if (sucesso)
        {
            Console.WriteLine(
                "Cliente removido."
            );
        }
        else
        {
            Console.WriteLine(
                "Cliente não encontrado."
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 5)
    {
        Console.Write(
            "ID do cliente: "
        );

        var clienteId =
            int.Parse(
                Console.ReadLine()
            );

        var cliente =
            clienteService.Buscar(
                clienteId
            );

        if (cliente == null)
        {
            Console.WriteLine(
                "Cliente não encontrado."
            );

            Console.ReadKey();
            continue;
        }

        var divida =
            new Divida();

        divida.Cliente = cliente;

        Console.Write(
            "Valor da dívida: "
        );

        divida.Valor =
            decimal.Parse(
                Console.ReadLine()
            );

        var sucesso =
            dividaService.Criar(
                divida,
                out var erros
            );

        if (!sucesso)
        {
            foreach (var erro in erros)
            {
                Console.WriteLine(
                    erro.ErrorMessage
                );
            }
        }
        else
        {
            Console.WriteLine(
                "Dívida cadastrada."
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 6)
    {
        var dividas =
            dividaService.Listar();

        foreach (var divida in dividas)
        {
            Console.WriteLine(
                $"ID: {divida.Id}"
            );

            Console.WriteLine(
                $"Cliente: {divida.Cliente.NomeCompleto}"
            );

            Console.WriteLine(
                $"Valor: R$ {divida.Valor:F2}"
            );

            Console.WriteLine(
                $"Situação: {divida.Situacao}"
            );

            Console.WriteLine(
                "----------------------"
            );
        }

        Console.ReadKey();
    }

    else if (opcao == 7)
    {
        Console.Write(
            "ID da dívida: "
        );

        var id =
            int.Parse(
                Console.ReadLine()
            );

        var sucesso =
            dividaService.MarcarComoPaga(
                id
            );

        if (sucesso)
        {
            Console.WriteLine(
                "Dívida paga."
            );
        }
        else
        {
            Console.WriteLine(
                "Dívida não encontrada."
            );
        }

        Console.ReadKey();
    }
}