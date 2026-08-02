using System;

namespace CaixaEletronico.Models
{
    public class Transacao
    {
        public int Id { get; private set; }

        public string Tipo { get; private set; }

        public decimal Valor { get; private set; }

        public DateTime DataHora { get; private set; }

        public long ContaOrigem { get; private set; }

        public long? ContaDestino { get; private set; }

        // Nova transação
        public Transacao(
            string tipo,
            decimal valor,
            long contaOrigem,
            long? contaDestino = null)
        {
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.Now;
            ContaOrigem = contaOrigem;
            ContaDestino = contaDestino;
        }

        // Transação lida do banco
        public Transacao(
            int id,
            string tipo,
            decimal valor,
            DateTime dataHora,
            long contaOrigem,
            long? contaDestino = null)
        {
            Id = id;
            Tipo = tipo;
            Valor = valor;
            DataHora = dataHora;
            ContaOrigem = contaOrigem;
            ContaDestino = contaDestino;
        }
    }
}
