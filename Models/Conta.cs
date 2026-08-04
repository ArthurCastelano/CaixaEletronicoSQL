using System;

namespace CaixaEletronico.Models
{
    public class Conta
    {
        public int Numero { get; private set; }
        public string Titular { get; private set; }
        public decimal Saldo { get; private set; }

        // Conta nova
        public Conta(string titular)
        {
            Titular = titular;
            Saldo = 0;
        }

        // Conta existente (para leitura do banco de dados)
        public Conta(int numero, string titular, decimal saldo)
        {
            Numero = numero;
            Titular = titular;
            Saldo = saldo;
        }
        public void Depositar(decimal valor)
        {
            Saldo += valor;
        }

        public bool Sacar(decimal valor)
        {
            if (Saldo < valor)
                return false;

            Saldo -= valor;
            return true;
        }
    }
}