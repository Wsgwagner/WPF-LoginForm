namespace WPF_LoginForm.Views
{
    public class Equipamento
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public bool Status { get; set; } // Agora permite gravação!
        public int Intensidade { get; set; }
    }

    public class ComodoVinculo
    {
        public string Nome { get; set; }
        public System.Collections.ObjectModel.ObservableCollection<Equipamento> Equipamentos { get; set; }
    }
}