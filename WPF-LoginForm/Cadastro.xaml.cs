using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions; // Necessário para a validação de e-mail
using System.Windows;
using System.Windows.Input;
using WPF_LoginForm.Views;

namespace WPF_LoginForm
{
    public partial class Cadastro : Window
    {
        // String de conexão configurada para o seu servidor
        private readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";
        //@"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public Cadastro()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Valida se o formato do e-mail é válido usando Expressão Regular.
        /// </summary>
        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Gera um Hash SHA256 para armazenar a senha de forma segura.
        /// </summary>
        private string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validação de campos vazios
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Campos básicos são obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validação de E-mail
            if (!IsEmailValid(txtEmail.Text))
            {
                MessageBox.Show("Por favor, insira um e-mail válido (exemplo@dominio.com).", "E-mail Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            int userId = 0;
            // Verifica se é uma atualização de usuário existente (pelo ID no label)
            bool isUpdate = int.TryParse(lblId.Text, out userId) && userId > 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Query dinâmica baseada na operação (Insert ou Update)
                    string query = isUpdate
                        ? "UPDATE Users SET Username=@User, Name=@Name, LastName=@LName, Email=@Email WHERE Id=@Id"
                        : "INSERT INTO Users (Username, Password, Name, LastName, Email) VALUES (@User, @Pass, @Name, @LName, @Email)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Parâmetros comuns
                        cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = txtUsername.Text;
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = txtName.Text;
                        cmd.Parameters.Add("@LName", SqlDbType.VarChar).Value = txtLastName.Text;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = txtEmail.Text;

                        if (isUpdate)
                        {
                            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = userId;
                        }
                        else
                        {
                            // Apenas insere a senha (com hash) se for um novo cadastro
                            cmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = ComputeHash(txtPassword.Password);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuário processado com sucesso!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Information);

                // --- NAVEGAÇÃO APÓS SUCESSO ---
                MainView principal = new MainView();
                principal.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco: " + ex.Message, "Erro Técnico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lógica para movimentar a janela com o mouse
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // Botões de fechar e voltar
        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();
        private void btnBack_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}