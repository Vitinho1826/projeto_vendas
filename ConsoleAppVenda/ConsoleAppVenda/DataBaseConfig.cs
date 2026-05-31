using Microsoft.Data.SqlClient;

namespace VendinhaPlena.Database;

public class DatabaseConfig
{
    private string connectionString =
        "Server=(localdb)\\MSSQLLocalDB;" +
        "Database=Vendinha;" +
        "Trusted_Connection=True;" +
        "TrustServerCertificate=True";

    public SqlConnection GetConnection()
    {
        return new SqlConnection(connectionString);
    }

    public void InicializarBanco()
    {
        using var connection = GetConnection();

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandText =
        @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='clientes' AND xtype='U')
        CREATE TABLE clientes
        (
            id INT IDENTITY(1,1) PRIMARY KEY,
            nome_completo VARCHAR(100) NOT NULL,
            cpf VARCHAR(11) UNIQUE NOT NULL,
            data_nascimento DATE NOT NULL,
            email VARCHAR(100)
        );

        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='dividas' AND xtype='U')
        CREATE TABLE dividas
        (
            id INT IDENTITY(1,1) PRIMARY KEY,

            cliente_id INT NOT NULL,

            valor DECIMAL(10,2) NOT NULL,

            situacao INT NOT NULL,

            data_criacao DATETIME NOT NULL,

            data_pagamento DATETIME NULL,

            FOREIGN KEY(cliente_id)
            REFERENCES clientes(id)
        );
        ";

        command.ExecuteNonQuery();
    }
}