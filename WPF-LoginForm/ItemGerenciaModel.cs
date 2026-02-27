namespace WPF_LoginForm.Views
{
    public class ItemGerenciaModel
    {
        // Propriedades Comuns
        public string Nome { get; set; }
        public string Comodo { get; set; } // Email (Usuário) ou Cômodo (Equipamento)
        public string Tipo { get; set; }   // Nível (Usuário) ou Tipo (Equipamento)
        public string Categoria { get; set; } // "USUÁRIO" ou "EQUIPAMENTO"

        // Propriedades Exclusivas de Usuário
        public string Sobrenome { get; set; }
        public string Telefone { get; set; }
        public string Senha { get; set; }
    }
}