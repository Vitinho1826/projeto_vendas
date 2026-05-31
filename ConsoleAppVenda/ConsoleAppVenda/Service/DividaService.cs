using Npgsql;
using System.ComponentModel.DataAnnotations;
using VendinhaPlena.Database;
using VendinhaPlena.Enums;
using VendinhaPlena.Models;

namespace VendinhaPlena.Services;

public class DividaService
{
    private readonly DatabaseConfig database =
        new DatabaseConfig();

    private readonly ClienteService clienteService =
        new ClienteService();

    public bool Criar(
        Divida divida,
        out List<ValidationResult> erros
    )
    {
        if (!Validar(divida, out erros))
        {
            return false;
        }

        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            INSERT INTO dividas
            (
                cliente_id,
                valor,
                situacao,
                data_criacao,
                data_pagamento
            )
            VALUES
            (
                @clienteId,
                @valor,
                @situacao,
                @dataCriacao,
                @dataPagamento
            )
        ";

        command.Parameters.AddWithValue(
            "@clienteId",
            divida.Cliente.Id
        );

        command.Parameters.AddWithValue(
            "@valor",
            divida.Valor
        );

        command.Parameters.AddWithValue(
            "@situacao",
            (int)divida.Situacao
        );

        command.Parameters.AddWithValue(
            "@dataCriacao",
            divida.DataCriacao
        );

        command.Parameters.AddWithValue(
            "@dataPagamento",
            divida.DataPagamento ?? (object)DBNull.Value
        );

        command.ExecuteNonQuery();

        return true;
    }

    public bool MarcarComoPaga(int id)
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            UPDATE dividas
            SET
                situacao = @situacao,
                data_pagamento = @dataPagamento
            WHERE id = @id
        ";

        command.Parameters.AddWithValue(
            "@situacao",
            (int)SituacaoDivida.Paga
        );

        command.Parameters.AddWithValue(
            "@dataPagamento",
            DateTime.Now
        );

        command.Parameters.AddWithValue(
            "@id",
            id
        );

        return command.ExecuteNonQuery() > 0;
    }

    public Divida Buscar(int id)
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            SELECT
                id,
                cliente_id,
                valor,
                situacao,
                data_criacao,
                data_pagamento
            FROM dividas
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

        return new Divida
        {
            Id = reader.GetInt32(0),

            Cliente = clienteService.Buscar(
                reader.GetInt32(1)
            ),

            Valor = reader.GetDecimal(2),

            Situacao =
                (SituacaoDivida)
                reader.GetInt32(3),

            DataPagamento =
                reader.IsDBNull(5)
                    ? null
                    : reader.GetDateTime(5)
        };
    }

    public List<Divida> Listar()
    {
        using var connection =
            database.GetConnection();

        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText =
        @"
            SELECT
                id,
                cliente_id,
                valor,
                situacao,
                data_criacao,
                data_pagamento
            FROM dividas
        ";

        using var reader =
            command.ExecuteReader();

        var lista =
            new List<Divida>();

        while (reader.Read())
        {
            lista.Add(
                new Divida
                {
                    Id = reader.GetInt32(0),

                    Cliente =
                        clienteService.Buscar(
                            reader.GetInt32(1)
                        ),

                    Valor = reader.GetDecimal(2),

                    Situacao =
                        (SituacaoDivida)
                        reader.GetInt32(3),

                    DataPagamento =
                        reader.IsDBNull(5)
                            ? null
                            : reader.GetDateTime(5)
                }
            );
        }

        return lista;
    }

    public List<Divida> ListarPorCliente(
        int clienteId
    )
    {
        return Listar()
            .Where(item =>
                item.Cliente.Id ==
                clienteId
            )
            .ToList();
    }

    public decimal TotalDividasCliente(
        int clienteId
    )
    {
        return ListarPorCliente(clienteId)
            .Where(item =>
                item.Situacao ==
                SituacaoDivida.Aberta
            )
            .Sum(item =>
                item.Valor
            );
    }

    public bool Validar(
        Divida divida,
        out List<ValidationResult> erros
    )
    {
        var contexto =
            new ValidationContext(divida);

        erros =
            new List<ValidationResult>();

        var valido =
            Validator.TryValidateObject(
                divida,
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
            FROM dividas
            WHERE cliente_id = @clienteId
            AND situacao = 0
        ";

        command.Parameters.AddWithValue(
            "@clienteId",
            divida.Cliente.Id
        );

        var quantidade =
            Convert.ToInt32(
                command.ExecuteScalar()
            );

        if (quantidade > 0)
        {
            erros.Add(
                new ValidationResult(
                    "Cliente já possui dívida em aberto"
                )
            );

            valido = false;
        }

        return valido;
    }
}