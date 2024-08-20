using Application.Application.Interfaces;
using Application.Application.Models;
using Entity.Entities;
using System.Globalization;

namespace Application.Application
{
 
       public class FolhaDePagamentoApplication : IFolhaDePagamentoApplication
    {

          public async Task<FolhaDePagamento> Processar(List<FolhaDePonto> folhasDePonto, Arquivo arquivo)
          {
              var folhaDePagamento = new FolhaDePagamento
              {
                  Departamento = arquivo.NomeDoDepartamento,
                  MesVigencia = arquivo.MesVigencia,
                  AnoVigente = arquivo.AnoDeVigencia
              };

              decimal totalPagar = 0;
              decimal totalDescontos = 0;
              decimal totalExtras = 0;

              var funcionarios = new List<Funcionario>();
              var registrosFuncionarios = folhasDePonto.GroupBy(x => x.Codigo);

              foreach (var registros in registrosFuncionarios)
              {
                  var registroDePontos = registros.ToList();

                  decimal totalReceber = 0;
                  decimal horasExtras = 0;
                  decimal horasDebito = 0;
                  var diasExtras = 0;
                  var diasTrabalhados = 0;


                  var valorHora = Decimal.Parse(registroDePontos[0].ValorHora.Replace("R$ ", "").Replace(" ", ""));

                  var funcionario = new Funcionario
                  {
                      Codigo = registroDePontos[0].Codigo,
                      Nome = registroDePontos[0].Nome
                  };


                  var qtdDiasUteis = Enumerable.Range(1, (DateTime.DaysInMonth(arquivo.AnoDeVigencia, DateTime.ParseExact(arquivo.MesVigencia, "MMMM", new CultureInfo("pt-BR")).Month)))
                                 .Select(dia => new DateTime(arquivo.AnoDeVigencia, DateTime.ParseExact(arquivo.MesVigencia, "MMMM", new CultureInfo("pt-BR")).Month, dia))
                                 .Count(data => data.DayOfWeek != DayOfWeek.Saturday &&
                                                data.DayOfWeek != DayOfWeek.Sunday);

                  foreach (var ponto in registroDePontos)
                  {
                    var cultureInfo = new CultureInfo("pt-BR");

                    var data = DateTime.ParseExact(ponto.Data, "dd/MM/yyyy", cultureInfo);
                    var entrada = DateTime.ParseExact(ponto.Entrada, "HH:mm:ss", cultureInfo);
                    var saida = DateTime.ParseExact(ponto.Saida, "HH:mm:ss", cultureInfo);

                    var almocoIntervalo = ponto.Almoco.Split(" - ");
                    var inicioAlmoco = DateTime.ParseExact(almocoIntervalo[0], "HH:mm", cultureInfo);
                    var fimAlmoco = DateTime.ParseExact(almocoIntervalo[1], "HH:mm", cultureInfo);

                    var horasTrabalhadasDia = (saida - entrada) - (fimAlmoco - inicioAlmoco);
                    var valorHoraDia = decimal.Parse(ponto.ValorHora.Replace("R$ ", "").Trim());

                    var horasTrabalhadas = (decimal)horasTrabalhadasDia.TotalHours;
                    var horasExcedentes = horasTrabalhadas - 8;

                    if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                    {
                        diasExtras++;
                        horasExtras += horasTrabalhadas;
                        totalExtras += horasTrabalhadas * valorHoraDia;
                    }
                    else
                    {
                        diasTrabalhados++;

                        if (horasExcedentes >= 0)
                        {
                            horasExtras += horasExcedentes;
                            totalExtras += horasExcedentes * valorHoraDia;
                        }
                        else
                        {
                            var horasDebitoLocal = Math.Abs(horasExcedentes);
                            horasDebito += horasDebitoLocal;
                            totalDescontos += horasDebitoLocal * valorHoraDia;
                        }
                    }


                    totalReceber += ((decimal)horasTrabalhadasDia.TotalHours * valorHoraDia);
                  }

                  totalDescontos += ((qtdDiasUteis - diasTrabalhados) * valorHora);

                  funcionario.TotalReceber = Math.Round(totalReceber, 2);
                  funcionario.HorasExtras = Math.Round(horasExtras, 2);
                  funcionario.HorasDebito = Math.Round(horasDebito, 2);
                  funcionario.DiasFalta = qtdDiasUteis - diasTrabalhados;
                  funcionario.DiasExtras = diasExtras;
                  funcionario.DiasTrabalhados = diasTrabalhados;

                  funcionarios.Add(funcionario);
              }

              folhaDePagamento.TotalPagar = Math.Round(funcionarios.Sum(x => x.TotalReceber), 2);
              folhaDePagamento.TotalDescontos = Math.Round(totalDescontos, 2);
              folhaDePagamento.TotalExtras = Math.Round(totalExtras, 2);
              folhaDePagamento.Funcionarios = funcionarios;

              return folhaDePagamento;
          }

      }

}
