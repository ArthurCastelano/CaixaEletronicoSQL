using System;
using CaixaEletronico.Repositorios;
using CaixaEletronico.Models;

//Interface do usuário
namespace CaixaEletronico
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RepositorioGeral repositorio = new RepositorioGeral();

            int opcao;

            do
            {
                Console.Clear();

                Console.WriteLine("====== CAIXA ELETRÔNICO ======");
                Console.WriteLine("1 - Criar Conta");
                Console.WriteLine("2 - Depositar");
                Console.WriteLine("3 - Sacar");
                Console.WriteLine("4 - Transferir");
                Console.WriteLine("5 - Consultar Saldo");
                Console.WriteLine("6 - Consultar Histórico");
                Console.WriteLine("0 - Sair");
                Console.WriteLine("=============================");

                Console.Write("\nEscolha uma opção: ");

                if (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.WriteLine("Opção inválida.");
                    Console.ReadKey();
                    continue;
                }

                switch (opcao)
                {
                    case 1:

                        Console.Clear();

                        Console.Write("==============================");
                        Console.Write("\nNome do titular: ");


                        string titular = Console.ReadLine()!;

                        int numeroConta = repositorio.CriarConta(titular);

                        Console.WriteLine("\nConta criada com sucesso!");
                        Console.WriteLine($"Número da conta: {numeroConta}");
                        Console.WriteLine("==============================");

                        break;

                    case 2:

                        Console.Clear();

                        Console.Write("==============================");
                        Console.Write("\nNúmero da conta: ");

                        int contaDeposito =
                            int.Parse(Console.ReadLine()!);

                        Console.Write("Valor do depósito: ");

                        decimal valorDeposito =
                            decimal.Parse(Console.ReadLine()!);

                        repositorio.Depositar(
                            contaDeposito,
                            valorDeposito);

                        Console.WriteLine("\nDepósito realizado!");
                        Console.Write("==============================");

                        break;

                    case 3:

                        Console.Clear();

                        Console.Write("==========================================");
                        Console.Write("\nNúmero da conta: ");

                        int contaSaque =
                            int.Parse(Console.ReadLine()!);

                        Console.Write("Valor do saque: ");

                        decimal valorSaque =
                            decimal.Parse(Console.ReadLine()!);

                        if (repositorio.Sacar(
                            contaSaque,
                            valorSaque))
                        {
                            Console.WriteLine("\nSaque realizado!");
                            Console.Write("==============================");
                        }
                        else
                        {
                            Console.WriteLine("\nSaldo insuficiente ou conta inexistente.");
                            Console.Write("==========================================");
                        }

                        break;

                    case 4:

                        Console.Clear();

                        Console.Write("==============================");
                        Console.Write("\nConta origem: ");

                        int origem =
                            int.Parse(Console.ReadLine()!);

                        Console.Write("Conta destino: ");

                        int destino =
                            int.Parse(Console.ReadLine()!);

                        Console.Write("Valor: ");

                        decimal valorTransferencia =
                            decimal.Parse(Console.ReadLine()!);

                        if (repositorio.Transferir(
                            origem,
                            destino,
                            valorTransferencia))
                        {
                            Console.WriteLine("\nTransferência realizada!");
                            Console.Write("==============================");
                        }
                        else
                        {
                            Console.WriteLine("\nTransferência não realizada.");
                            Console.Write("==============================");
                        }

                        break;

                    case 5:

                        Console.Clear();

                        Console.Write("==============================");
                        Console.Write("\nNúmero da conta: ");

                        int numeroSaldo =
                            int.Parse(Console.ReadLine()!);

                        decimal saldo =
                            repositorio.ConsultarSaldo(numeroSaldo);

                        if (saldo >= 0)
                        {
                            Console.WriteLine($"\nSaldo: R$ {saldo:F2}");
                            Console.Write("==============================");
                        }
                        else
                        {
                            Console.WriteLine("\nConta não encontrada.");
                            Console.Write("==============================");
                        }

                        break;

                    case 6:

                        Console.Clear();

                        Console.Write("==============================");
                        Console.Write("\nNúmero da conta: ");

                        int numeroHistorico =
                            int.Parse(Console.ReadLine()!);

                        var lista =
                            repositorio.BuscarHistorico(numeroHistorico);

                        if (lista.Count == 0)
                        {
                            Console.WriteLine("\nNenhuma transação encontrada.");
                            Console.Write("==============================");
                        }
                        else
                        {
                            Console.WriteLine();

                            foreach (Transacao t in lista)
                            {
                                Console.WriteLine("-----------------------------------");
                                Console.WriteLine($"Data: {t.DataHora}");
                                Console.WriteLine($"Tipo: {t.Tipo}");
                                Console.WriteLine($"Valor: R$ {t.Valor:F2}");
                                Console.WriteLine($"Origem: {t.ContaOrigem}");

                                if (t.ContaDestino != null)
                                {
                                    Console.WriteLine($"Destino: {t.ContaDestino}");
                                    
                                }
                            }
                        }

                        break;

                    case 0:

                        Console.WriteLine("\nEncerrando...");

                        break;

                    default:

                        Console.WriteLine("\nOpção inválida.");

                        break;
                }

                if (opcao != 0)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                }

            } while (opcao != 0);
        }
    }
}