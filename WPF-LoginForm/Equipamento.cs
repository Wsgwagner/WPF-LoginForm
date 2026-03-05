using System.Collections.ObjectModel;

namespace WPF_LoginForm.Views
{
    public class Equipamento
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public bool Status { get; set; } // O 'set' resolve o erro de XamlParseException
        public int Intensidade { get; set; }
    }

    public class ComodoVinculo
    {
        public string Nome { get; set; }
        public ObservableCollection<Equipamento> Equipamentos { get; set; }
    }
}