using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarEquipamento : UserControl
    {
        // String de conexão para o servidor TDS do Senac
        private readonly string connectionString = @"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarEquipamento()
        {
            InitializeComponent();
        }

        private void BtnSalvarEquip_Click(object sender, RoutedEventArgs e)
        {
            // Validação de segurança
            if (string.IsNullOrWhiteSpace(txtNomeEquip.Text) || string.IsNullOrWhiteSpace(cbTipo.Text))
            {
                MessageBox.Show("Preencha o nome e o tipo do equipamento.", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO Equipamentos (Nome, Tipo, Comodo, Status) VALUES (@Nome, @Tipo, @Comodo, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = txtNomeEquip.Text;

                        // Captura o texto digitado ou selecionado no ComboBox editável
                        cmd.Parameters.Add("@Tipo", SqlDbType.VarChar).Value = cbTipo.Text;

                        cmd.Parameters.Add("@Comodo", SqlDbType.VarChar).Value = txtComodo.Text;

                        string status = rbAtivo.IsChecked == true ? "Ativo" : "Inativo";
                        cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = status;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Equipamento '" + txtNomeEquip.Text + "' salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LimparCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no SQL Server: " + ex.Message, "Erro Técnico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimparCampos()
        {
            txtNomeEquip.Clear();
            txtComodo.Clear();
            cbTipo.Text = ""; // Limpa o texto digitado no ComboBox
            rbAtivo.IsChecked = true;
            txtNomeEquip.Focus();
        }

        // Métodos obrigatórios para os eventos do XAML
        private void txtNomeEquip_TextChanged(object sender, TextChangedEventArgs e) { }
        private void cbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void txtComodo_TextChanged(object sender, TextChangedEventArgs e) { }
        private void rbAtivo_Checked(object sender, RoutedEventArgs e) { }
        private void rbInativo_Checked(object sender, RoutedEventArgs e) { }
    }
}