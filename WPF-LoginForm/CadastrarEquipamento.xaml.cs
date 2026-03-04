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
            this.Loaded += CadastrarEquipamento_Loaded;
        }

        private void CadastrarEquipamento_Loaded(object sender, RoutedEventArgs e)
        {
            // Mantém a restrição: Apenas o Administrador (Wagner) pode editar
            if (SessaoSistema.AccessLevel != "Administrador")
            {
                txtNomeEquip.IsEnabled = false;
                cbTipo.IsEnabled = false;
                cbComodo.IsEnabled = false;
                BtnSalvar.IsEnabled = false;
                BtnSalvar.Content = "BLOQUEADO (APENAS ADM)";
                BtnSalvar.Opacity = 0.5;
            }
        }

        private void BtnSalvarEquipamento_Click(object sender, RoutedEventArgs e)
        {
            if (SessaoSistema.AccessLevel != "Administrador") return;

            if (string.IsNullOrWhiteSpace(txtNomeEquip.Text))
            {
                MessageBox.Show("Dê um nome ao dispositivo!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "INSERT INTO Equipamentos (Nome, Tipo, Comodo, Status, ResidenciaId) VALUES (@nome, @tipo, @comodo, 'Desligado', @resId)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nome", txtNomeEquip.Text.Trim());

                        // AGORA PEGAMOS O TEXTO DIGITADO OU SELECIONADO
                        cmd.Parameters.AddWithValue("@tipo", cbTipo.Text.Trim());
                        cmd.Parameters.AddWithValue("@comodo", cbComodo.Text.Trim());

                        cmd.Parameters.AddWithValue("@resId", SessaoSistema.ResidenciaId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Equipamento cadastrado com sucesso!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Limpa os campos após salvar
                        txtNomeEquip.Clear();
                        cbTipo.Text = "Iluminação";
                        cbComodo.Text = "Sala de Estar";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message, "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}