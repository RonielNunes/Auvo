
using Application.Application;
using Application.Application.Models;
using Entity.Entities;
using System.Text.Json;

namespace Presentation
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Por favor, insira o caminho do diretório contendo os arquivos CSV:");

            string? caminho = Console.ReadLine()?.Trim();

            #region IsNullOrEmpty
            if (string.IsNullOrEmpty(caminho))
            {
                Console.WriteLine("Nenhum caminho foi inserido.");
                return;
            }
            #endregion IsNullOrEmpty

            #region Directory not exists
            if (!Directory.Exists(caminho))
            {
                Console.WriteLine($"O diretório '{caminho}' não foi encontrado.");
                return;
            }
            #endregion Directory not exists

            var arquivosCSV = Directory.GetFiles(caminho, "*.csv");

            #region Not Found 
            if (arquivosCSV.Length == 0)
            {
                Console.WriteLine($"Nenhum arquivo CSV foi encontrado no diretório '{caminho}'.");
                return;
            }
            #endregion Not Found 

            Console.WriteLine($"\nForam encontrados {arquivosCSV.Length} arquivo(s) CSV no diretório '{caminho}'\n");

            var folhaDePagamentos = new List<FolhaDePagamento>();

            foreach (var arquivo in arquivosCSV)
            {

                #region extraindo informações 
                int indice = arquivo.LastIndexOf("\\", StringComparison.Ordinal); 

                string titulo = arquivo.Substring(indice + 1);

                string[] palavras = titulo.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                var arquivoInformacao = new Arquivo
                {
                    NomeDoDepartamento = palavras[0],
                    MesVigencia = palavras[1],
                    AnoDeVigencia = int.Parse(palavras[2].Replace(".csv", "")),
                };
                #endregion

                var folhaDePontoApp = new FolhaDePontoApplication(arquivo);
                var folhaDePonto = await folhaDePontoApp.CarregarFolhaDePonto();

                #region Erro na conversão para entidade.
                if(folhaDePonto == null)
                {
                    return;
                }
                #endregion Erro na conversão para entidade.

                var folhaDePagamentoApp = new FolhaDePagamentoApplication();
                var folhaDePagamento = await folhaDePagamentoApp.Processar(folhaDePonto, arquivoInformacao);

                folhaDePagamentos.Add(folhaDePagamento);

            }

            #region Geração de arquivo de folha de pagamento em formato de json
            var opcoes = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(folhaDePagamentos, opcoes);


            File.WriteAllText($"{caminho}/FolhaDePagamento.json", jsonString);

            Console.WriteLine("Arquivo JSON salvo com sucesso.");
            Console.WriteLine();
            #endregion
        }
    }
}