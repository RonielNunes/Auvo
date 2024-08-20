using CsvHelper.Configuration.Attributes;

namespace Entity.Entities
{
    public class FolhaDePonto
    {
        [Name("Código")]
        public int Codigo { get; set; }

        [Name("Nome")]
        public string Nome { get; set; }

        [Name("Valor hora")]
        public string ValorHora { get; set; }

        [Name("Data")]
        public string Data { get; set; }

        [Name("Entrada")]
        public string Entrada { get; set; }

        [Name("Saída")]
        public string Saida { get; set; }

        [Name("Almoço")]
        public string Almoco { get; set; }
    }
}