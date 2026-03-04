using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WPF_LoginForm.Views
{
    public partial class RecuperarSenha : Window
    {
        // Use a mesma string de conexão do seu Login
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public RecuperarSenha()
        {
            InitializeComponent();
        }

        // CORRIGE: 'RecuperarSenha' não contém uma definição para "BtnFechar_Click"
        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // CORRIGE: 'RecuperarSenha' não contém uma definição para "BtnAtualizar_Click"
        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            // CORRIGE: O nome "txtEmail" não existe no contexto atual
            if (string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtNovaSenha.Password))
            {
                MessageBox.Show("Preencha todos os campos!");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string hash = GerarHash(txtNovaSenha.Password);
                    string sql = "UPDATE Users SET Password = @senha WHERE Email = @email";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@senha", hash);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show("Senha atualizada com sucesso!");
                            this.Close();
                        }
                        else { MessageBox.Show("E-mail não encontrado no banco!"); }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private string GerarHash(string senha)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}