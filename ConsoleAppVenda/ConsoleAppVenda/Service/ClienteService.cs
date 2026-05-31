using Npgsql;
using System.ComponentModel.DataAnnotations;
using VendinhaPlena.Database;
using VendinhaPlena.Models;

namespace VendinhaPlena.Services;

public class ClienteService
{
    private readonly DatabaseConfig database =
        new DatabaseConfig();

    public bool Criar(
        Cliente cliente,
        out List<ValidationResult> erros
    )
    {
        if (!Validar(cliente, out erros))
        {
            return false;
        }

        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            INSERT INTO clientes
            (
                nome_completo,
                cpf,
                data_nascimento,
                email
            )
            VALUES
            (
                @nome,
                @cpf,
                @dataNascimento,
                @email
            )
        ";

        command.Parameters.AddWithValue(
            "@nome",
            cliente.NomeCompleto
        );

        command.Parameters.AddWithValue(
            "@cpf",
            cliente.CPF
        );

        command.Parameters.AddWithValue(
            "@dataNascimento",
            cliente.DataNascimento
        );

        command.Parameters.AddWithValue(
            "@email",
            cliente.Email ?? ""
        );

        command.ExecuteNonQuery();

        return true;
    }

    public bool Atualizar(
        Cliente cliente,
        out List<ValidationResult> erros
    )
    {
        if (!Validar(cliente, out erros))
        {
            return false;
        }

        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            UPDATE clientes
            SET
                nome_completo = @nome,
                cpf = @cpf,
                data_nascimento = @dataNascimento,
                email = @email
            WHERE id = @id
        ";

        command.Parameters.AddWithValue(
            "@id",
            cliente.Id
        );

        command.Parameters.AddWithValue(
            "@nome",
            cliente.NomeCompleto
        );

        command.Parameters.AddWithValue(
            "@cpf",
            cliente.CPF
        );

        command.Parameters.AddWithValue(
            "@dataNascimento",
            cliente.DataNascimento
        );

        command.Parameters.AddWithValue(
            "@email",
            cliente.Email ?? ""
        );

        command.ExecuteNonQuery();

        return true;
    }

    public bool Excluir(int id)
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            DELETE FROM clientes
            WHERE id = @id
        ";

        command.Parameters.AddWithValue(
            "@id",
            id
        );

        return command.ExecuteNonQuery() > 0;
    }

    public Cliente Buscar(int id)
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            SELECT
                id,
                nome_completo,
                cpf,
                data_nascimento,
                email
            FROM clientes
            WHERE id = @id
        ";

        command.Parameters.AddWithValue(
            "@id",
            id
        );

        using var reader =
            command.ExecuteReader();

        if (!reader.Read())
        {
            return null;
        }

        return new Cliente
        {
            Id = reader.GetInt32(0),
            NomeCompleto = reader.GetString(1),
            CPF = reader.GetString(2),
            DataNascimento = reader.GetDateTime(3),
            Email = reader.IsDBNull(4)
                ? ""
                : reader.GetString(4)
        };
    }

    public List<Cliente> Listar()
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            SELECT
                id,
                nome_completo,
                cpf,
                data_nascimento,
                email
            FROM clientes
        ";

        using var reader =
            command.ExecuteReader();

        var lista =
            new List<Cliente>();

        while (reader.Read())
        {
            lista.Add(
                new Cliente
                {
                    Id = reader.GetInt32(0),
                    NomeCompleto = reader.GetString(1),
                    CPF = reader.GetString(2),
                    DataNascimento = reader.GetDateTime(3),
                    Email = reader.IsDBNull(4)
                        ? ""
                        : reader.GetString(4)
                }
            );
        }

        return lista;
    }

    public List<Cliente> Pesquisa(
        string texto
    )
    {
        return Listar()
            .Where(cliente =>
                cliente.NomeCompleto.Contains(
                    texto,
                    StringComparison
                    .OrdinalIgnoreCase
                )
            )
            .ToList();
    }

    public List<Cliente> Listar(
        int pageSize,
        int page
    )
    {
        var skip =
            (page - 1) * pageSize;

        return Listar()
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }

    public List<Cliente> ListarOrdenadoPorDivida()
    {
        var dividaService =
            new DividaService();

        return Listar()
            .OrderByDescending(cliente =>
                dividaService.TotalDividasCliente(
                    cliente.Id
                )
            )
            .ToList();
    }

    public bool Validar(
        Cliente cliente,
        out List<ValidationResult> erros
    )
    {
        var contexto =
            new ValidationContext(cliente);

        erros =
            new List<ValidationResult>();

        var valido =
            Validator.TryValidateObject(
                cliente,
                contexto,
                erros,
                true
            );

        using var connection =
            database.GetConnection();

        connection.Open();

        var command =
            connection.CreateCommand();

        command.CommandText =
        @"
            SELECT COUNT(*)
            FROM clientes
            WHERE cpf = @cpf
            AND id != @id
        ";

        command.Parameters.AddWithValue(
            "@cpf",
            cliente.CPF
        );

        command.Parameters.AddWithValue(
            "@id",
            cliente.Id
        );

        var quantidade =
            Convert.ToInt32(
                command.ExecuteScalar()
            );

        if (quantidade > 0)
        {
            erros.Add(
                new ValidationResult(
                    "Já existe cliente com esse CPF"
                )
            );

            valido = false;
        }

        return valido;
    }
}