using System.Collections.ObjectModel;

namespace WPF_LoginForm.Views
{
    public class Equipamentos
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public bool Status { get; set; }
        public int Intensidade { get; set; }
    }

    public class ComodosVinculo
    {
        public string Nome { get; set; }
        public ObservableCollection<Equipamento> Equipamentos { get; set; }
    }
}