# Vendinha Plena

## Sobre o Projeto

O Vendinha Plena é um sistema desenvolvido em C# para auxiliar uma pequena vendinha no controle de clientes e de suas dívidas pendentes.

O sistema permite cadastrar clientes, registrar dívidas, consultar informações e controlar pagamentos de forma simples, substituindo o controle realizado anteriormente em papel.

## Tecnologias Utilizadas

* C#
* .NET
* PostgreSQL
* Npgsql

## Estrutura do Projeto

```text
VendinhaPlena
│
├── Database
│   └── DatabaseConfig.cs
│
├── Models
│   ├── Cliente.cs
│   └── Divida.cs
│
├── Services
│   ├── ClienteService.cs
│   └── DividaService.cs
│
└── Program.cs
```

## Banco de Dados

O sistema utiliza PostgreSQL como banco de dados.

### Tabela Clientes

| Campo           | Tipo         |
| --------------- | ------------ |
| id              | SERIAL       |
| nome_completo   | VARCHAR(100) |
| cpf             | VARCHAR(11)  |
| data_nascimento | DATE         |
| email           | VARCHAR(100) |

### Tabela Dívidas

| Campo          | Tipo          |
| -------------- | ------------- |
| id             | SERIAL        |
| cliente_id     | INTEGER       |
| valor          | NUMERIC(10,2) |
| situacao       | INTEGER       |
| data_criacao   | TIMESTAMP     |
| data_pagamento | TIMESTAMP     |

## Configuração

### 1. Instalar o PostgreSQL

Criar um banco de dados chamado:

```sql
CREATE DATABASE vendinha;
```

### 2. Configurar a conexão

No arquivo `DatabaseConfig.cs`:

```csharp
private string connectionString =
    "Host=localhost;" +
    "Port=5432;" +
    "Database=vendinha;" +
    "Username=postgres;" +
    "Password=123456";
```

Altere os dados conforme sua instalação do PostgreSQL.

### 3. Restaurar dependências

```bash
dotnet restore
```

### 4. Instalar o Npgsql

```bash
dotnet add package Npgsql
```

### 5. Executar o projeto

```bash
dotnet run
```

## Funcionalidades

* Cadastro de clientes
* Consulta de clientes
* Registro de dívidas
* Consulta de dívidas
* Controle de pagamento de dívidas
* Persistência dos dados em PostgreSQL

## Objetivo Acadêmico

Projeto desenvolvido para fins de aprendizado de programação orientada a objetos, acesso a banco de dados com ADO.NET e utilização do PostgreSQL em aplicações C#.
