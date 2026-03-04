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
        }

        private void BtnSalvarEquipamento_Click(object sender, RoutedEventArgs e)
        {
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
                    string sql = "INSERT INTO Equipamentos (Nome, Tipo, Comodo, Status, ResidenciaId) VALUES (@nome, @tipo, @comodo, 'Desligado', @resId)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@nome", txtNomeEquip.Text.Trim());
                        cmd.Parameters.AddWithValue("@tipo", (cbTipo.SelectedItem as ComboBoxItem)?.Content.ToString());
                        cmd.Parameters.AddWithValue("@comodo", (cbComodo.SelectedItem as ComboBoxItem)?.Content.ToString());
                        // Usa a sessão para vincular à casa do Wagner
                        cmd.Parameters.AddWithValue("@resId", SessaoSistema.ResidenciaId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Equipamento cadastrado com sucesso!");
                        txtNomeEquip.Clear();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }
    }
}