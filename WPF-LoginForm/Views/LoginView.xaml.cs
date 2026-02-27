using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WPF_LoginForm.Views
{
    public partial class LoginView : Window
    {
        private readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";
        //@"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public LoginView()
        {
            InitializeComponent();
        }

        private string ComputeHash(string input)
        {
            if (input == null) return "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void btnLogin_Click_1(object sender, RoutedEventArgs e)
        {
            string usuario = txtUser.Text.Trim();
            string senhaDigitada = Password.Password;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senhaDigitada))
            {
                MessageBox.Show("Preencha todos os campos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

           string hashParaComparar = ComputeHash(senhaDigitada);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT COUNT(1) FROM Users WHERE Username=@User AND Password=@Pass";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = usuario;
                        cmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = hashParaComparar;

                        int resultado = Convert.ToInt32(cmd.ExecuteScalar());

                        if (resultado == 1)
                        {
                            MessageBox.Show("Login realizado com sucesso!", "Bem-vindo", MessageBoxButton.OK, MessageBoxImage.Information);

                            // --- ABRIR MAINVIEW ---
                            MainView principal = new MainView();
                            principal.Show();
                            this.Close(); // Fecha a tela de Login
                        }
                        else
                        {
                            MessageBox.Show("Usuário ou senha incorretos.", "Falha", MessageBoxButton.OK, MessageBoxImage.Error);
                            txtUser.Clear();
                            Password.Clear();
                            txtUser.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro técnico: " + ex.Message, "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } // Fechamento do método btnLogin_Click_1

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void btnCadastro_Click_1(object sender, RoutedEventArgs e)
        {
            new Cadastro().ShowDialog();
        }

        private void BindablePasswordBox_Loaded(object sender, RoutedEventArgs e) { }
    }
}