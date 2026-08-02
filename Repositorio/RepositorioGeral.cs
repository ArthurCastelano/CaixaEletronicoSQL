using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CaixaEletronico.Models;

//criação do banco de dados SQLite
namespace CaixaEletronico.Repositorios
{
    public class RepositorioGeral
    {
        private readonly string Conexao = "Data Source=caixa_eletronico.db";

        public RepositorioGeral()
        {
            CriarBanco();
        }

        private void CriarBanco()
        {
            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco = @"

            CREATE TABLE IF NOT EXISTS Contas(

                Numero INTEGER PRIMARY KEY AUTOINCREMENT,

                Titular TEXT NOT NULL,

                Saldo REAL NOT NULL

            );

            CREATE TABLE IF NOT EXISTS Transacoes(

                Id INTEGER PRIMARY KEY AUTOINCREMENT,

                Tipo TEXT NOT NULL,

                Valor REAL NOT NULL,

                DataHora TEXT NOT NULL,

                ContaOrigem INTEGER NOT NULL,

                ContaDestino INTEGER

            );

            ";

            using var comando = new SqliteCommand(banco, conexao);

            comando.ExecuteNonQuery();
        }
        public int CriarConta(string titular)
        {
            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco = @"
                      INSERT INTO Contas(Titular, Saldo)
                      VALUES(@titular, @saldo);
                      SELECT last_insert_rowid();";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@titular", titular);
            comando.Parameters.AddWithValue("@saldo", 0);

            long numeroConta = (long)comando.ExecuteScalar()!;

            return (int)numeroConta;
        }

        public Conta BuscarConta(int numero)
        {
            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco =
            @"SELECT Numero,
                     Titular,
                     Saldo
              FROM Contas
              WHERE Numero=@numero";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@numero", numero);

            using var leitor = comando.ExecuteReader();

            if (leitor.Read())
            {
                int numeroConta = leitor.GetInt32(0);

                string titular = leitor.GetString(1);

                decimal saldo =
                    Convert.ToDecimal(leitor.GetDouble(2));

                return new Conta(numeroConta, titular, saldo);
            }

            return null!;

        }
        private void RegistrarTransacao
                  (string tipo, decimal valor, long origem, long? destino = null)
        {
            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco =
            @"INSERT INTO Transacoes
            (
                Tipo,
                Valor,
                DataHora,
                ContaOrigem,
                ContaDestino
            )

            VALUES

            (
                @tipo,
                @valor,
                @data,
                @origem,
                @destino
            );";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@tipo", tipo);

            comando.Parameters.AddWithValue("@valor", valor);

            comando.Parameters.AddWithValue
            (
                "@data",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            );

            comando.Parameters.AddWithValue("@origem", origem);

            if (destino == null)
            {
                comando.Parameters.AddWithValue("@destino", DBNull.Value);
            }
            else
            {
                comando.Parameters.AddWithValue("@destino", destino);
            }

            comando.ExecuteNonQuery();
        }
        public void Depositar(int numero, decimal valor)
        {
            if (valor <= 0)
                return;

            Conta conta = BuscarConta(numero);

            if (conta == null)
                return;

            decimal novoSaldo = conta.Saldo + valor;

            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco = @"UPDATE Contas
                   SET Saldo = @saldo
                   WHERE Numero = @numero";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@saldo", novoSaldo);
            comando.Parameters.AddWithValue("@numero", numero);

            comando.ExecuteNonQuery();

            RegistrarTransacao("Depósito", valor, numero);
        }
        public bool Sacar(int numero, decimal valor)
        {
            if (valor <= 0)
                return false;

            Conta conta = BuscarConta(numero);

            if (conta == null)
                return false;

            if (conta.Saldo < valor)
                return false;

            decimal novoSaldo = conta.Saldo - valor;

            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco = @"UPDATE Contas
                   SET Saldo = @saldo
                   WHERE Numero = @numero";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@saldo", novoSaldo);
            comando.Parameters.AddWithValue("@numero", numero);

            comando.ExecuteNonQuery();

            RegistrarTransacao("Saque", valor, numero);

            return true;
        }
        public bool Transferir(int origem, int destino, decimal valor)
        {
            if (valor <= 0)
                return false;

            Conta contaOrigem = BuscarConta(origem);
            Conta contaDestino = BuscarConta(destino);

            if (contaOrigem == null || contaDestino == null)
                return false;

            if (contaOrigem.Saldo < valor)
                return false;

            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            using var transacao = conexao.BeginTransaction();

            try
            {
                var comando = conexao.CreateCommand();

                comando.Transaction = transacao;

                comando.CommandText =
                    @"UPDATE Contas
              SET Saldo = Saldo - @valor
              WHERE Numero = @origem";

                comando.Parameters.AddWithValue("@valor", valor);
                comando.Parameters.AddWithValue("@origem", origem);

                comando.ExecuteNonQuery();

                comando.Parameters.Clear();

                comando.CommandText =
                    @"UPDATE Contas
              SET Saldo = Saldo + @valor
              WHERE Numero = @destino";

                comando.Parameters.AddWithValue("@valor", valor);
                comando.Parameters.AddWithValue("@destino", destino);

                comando.ExecuteNonQuery();

                transacao.Commit();

                RegistrarTransacao(
                    "Transferência",
                    valor,
                    origem,
                    destino);

                return true;
            }
            catch
            {
                transacao.Rollback();
                return false;
            }
        }
        public decimal ConsultarSaldo(int numero)
        {
            Conta conta = BuscarConta(numero);

            if (conta == null)
                return -1;

            return conta.Saldo;
        }
        public List<Transacao> BuscarHistorico(int numero)
        {
            List<Transacao> lista = new List<Transacao>();

            using var conexao = new SqliteConnection(Conexao);

            conexao.Open();

            string banco =
            @"SELECT Tipo,
             Valor,
             DataHora,
             ContaOrigem,
             ContaDestino
      FROM Transacoes
      WHERE ContaOrigem = @numero
         OR ContaDestino = @numero
      ORDER BY DataHora DESC";

            using var comando = new SqliteCommand(banco, conexao);

            comando.Parameters.AddWithValue("@numero", numero);

            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                string tipo = reader.GetString(0);

                decimal valor = Convert.ToDecimal(reader.GetDouble(1));

                DateTime dataHora =
                    DateTime.Parse(reader.GetString(2));

                long origem = reader.GetInt64(3);

                long? destino = reader.IsDBNull(4)
                    ? (long?)null
                    : reader.GetInt64(4);

                lista.Add(
                    new Transacao(
                        0,
                        tipo,
                        valor,
                        dataHora,
                        origem,
                        destino));
            }

            return lista;

        }
    }
}