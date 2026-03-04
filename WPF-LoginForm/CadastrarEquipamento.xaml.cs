using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarEquipamento : UserControl
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarEquipamento()
        {
            InitializeComponent();
            // Verifica a permissão assim que o controle é carregado na tela
            this.Loaded += CadastrarEquipamento_Loaded;
        }

        private void CadastrarEquipamento_Loaded(object sender, RoutedEventArgs e)
        {
            // Bloqueio de Segurança: Somente Administrador pode interagir
            if (SessaoSistema.AccessLevel != "Administrador")
            {
                // Desabilita a interação com os campos e o botão
                txtNomeEquip.IsEnabled = false;
                cbTipo.IsEnabled = false;
                cbComodo.IsEnabled = false;

                // Opcional: Você pode esconder o botão ou apenas desabilitá-lo
                BtnSalvar.IsEnabled = false;
                BtnSalvar.Content = "BLOQUEADO (APENAS ADM)";
                BtnSalvar.Opacity = 0.5;

                MessageBox.Show("Acesso Restrito! Moradores podem apenas visualizar os equipamentos existentes.",
                                "IA HOME - Segurança", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSalvarEquipamento_Click(object sender, RoutedEventArgs e)
        {
            // Validação redundante por segurança
            if (SessaoSistema.AccessLevel != "Administrador") return;

            if (string.IsNullOrWhiteSpace(txtNomeEquip.Text))
            {
                MessageBox.Show("Dê um nome ao dispositivo!");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Vincula o equipamento à residência correta através da SessãoSistema
                    string sql = "INSERT INTO Equipamentos (Nome, Tipo, Comodo, Status, ResidenciaId) VALUES (@nome, @tipo, @comodo, 'Desligado', @resId)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nome", txtNomeEquip.Text.Trim());
                        cmd.Parameters.AddWithValue("@tipo", (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@comodo", (cbComodo.SelectedItem as ComboBoxItem)?.Content.ToString());

                        // Garante que o equipamento seja da casa do usuário logado
                        cmd.Parameters.AddWithValue("@resId", SessaoSistema.ResidenciaId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Equipamento cadastrado com sucesso!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtNomeEquip.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar equipamento: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}