namespace WPF_LoginForm
{
    // Esta classe define o que é um equipamento no banco de dados
    public class EquipamentoModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Comodo { get; set; }
        public string Status { get; set; }
        public int ResidenciaId { get; set; } // O ID que garante o isolamento
    }
}