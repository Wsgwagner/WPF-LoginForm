using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarUsuario : UserControl
    {
        private readonly string conexaoDb = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarUsuario()
        {
            InitializeComponent();
        }

        private void BtnSalvarFamiliar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCodigoVinculo.Text))
            {
                MessageBox.Show("O código da residência é obrigatório para o vínculo!");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexaoDb))
                {
                    con.Open();

                    // 1. Buscar o ID da residência pelo código de 6 dígitos
                    string sqlBusca = "SELECT Id FROM Residencias WHERE CodigoAcesso = @cod";
                    int? resIdEncontrado = null;

                    using (SqlCommand cmdBusca = new SqlCommand(sqlBusca, con))
                    {
                        cmdBusca.Parameters.AddWithValue("@cod", txtCodigoVinculo.Text.Trim());
                        var result = cmdBusca.ExecuteScalar();
                        if (result != null) resIdEncontrado = (int)result;
                    }

                    if (resIdEncontrado == null)
                    {
                        MessageBox.Show("Código de Residência não encontrado! Verifique com o Administrador.");
                        return;
                    }

                    // 2. Inserir o novo familiar já vinculado ao ID encontrado
                    string sqlInsert = @"INSERT INTO Users (Username, Password, Email, Phone, Name, LastName, ResidenciaId, AccessLevel) 
                                       VALUES (@user, @pass, @email, @phone, @name, @last, @resId, 'Morador')";

                    using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                    {
                        cmd.Parameters.AddWithValue("@user", txtEmailUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", HashFamiliar(txtSenhaUser.Password));
                        cmd.Parameters.AddWithValue("@email", txtEmailUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@phone", txtPhoneUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtNomeUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@last", txtSobrenomeUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@resId", resIdEncontrado);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"{txtNomeUser.Text} cadastrado e vinculado à residência!");
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void LimparCampos()
        {
            txtNomeUser.Clear(); txtSobrenomeUser.Clear(); txtEmailUser.Clear();
            txtPhoneUser.Clear(); txtCodigoVinculo.Clear(); txtSenhaUser.Clear();
        }

        private string HashFamiliar(string senha)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}