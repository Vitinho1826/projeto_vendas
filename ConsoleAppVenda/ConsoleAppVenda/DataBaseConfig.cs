using Npgsql;

namespace VendinhaPlena.Database;

public class DatabaseConfig
{
    private string connectionString =
        "Host=localhost;" +
        "Port=5432;" +
        "Database=vendinha;" +
        "Username=postgres;" +
        "Password=123456";

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(connectionString);
    }

    public void InicializarBanco()
    {
        using var connection = GetConnection();

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText =
        @"
        CREATE TABLE IF NOT EXISTS clientes
        (
            id SERIAL PRIMARY KEY,
            nome_completo VARCHAR(100) NOT NULL,
            cpf VARCHAR(11) UNIQUE NOT NULL,
            data_nascimento DATE NOT NULL,
            email VARCHAR(100)
        );

        CREATE TABLE IF NOT EXISTS dividas
        (
            id SERIAL PRIMARY KEY,

            cliente_id INTEGER NOT NULL,

            valor NUMERIC(10,2) NOT NULL,

            situacao INTEGER NOT NULL,

            data_criacao TIMESTAMP NOT NULL,

            data_pagamento TIMESTAMP,

            CONSTRAINT fk_cliente
                FOREIGN KEY(cliente_id)
                REFERENCES clientes(id)
        );
        ";

        command.ExecuteNonQuery();
    }
}