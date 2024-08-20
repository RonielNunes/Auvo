using Entity.Entities;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Application.Application.Interfaces;

namespace Application.Application
{
    public class FolhaDePontoApplication : IFolhaDePontoApplication
    {
        private string _caminho;
        public FolhaDePontoApplication(string caminho) { 
            this._caminho = caminho;
        }
        public async Task<List<FolhaDePonto>> CarregarFolhaDePonto()
        {
            var registrosFolhas = new List<FolhaDePonto>();

            using var reader = new StreamReader(_caminho, Encoding.UTF8);

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                TrimOptions = TrimOptions.Trim
            };

            using var csv = new CsvReader(reader, csvConfig);

            csv.Read();

            csv.ReadHeader();
            try
            {
                var registros = await csv.GetRecordsAsync<FolhaDePonto>().ToListAsync();
                registrosFolhas.AddRange(registros);

                return registrosFolhas;

            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

    }
}
