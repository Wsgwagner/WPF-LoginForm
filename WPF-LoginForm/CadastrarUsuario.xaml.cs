using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarUsuario : UserControl
    {
        // String de conexão com seu servidor SQLEXPRESS
        private readonly string connectionString = @"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarUsuario()
        {
            InitializeComponent();
        }

        // --- MÉTODO PARA VALIDAR FORMATO DE EMAIL ---
        private bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        // --- MÉTODO PARA CRIPTOGRAFAR A SENHA ---
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

        // --- MÉTODO PARA LIMPAR CAMPOS APÓS SALVAR ---
        private void LimparCampos()
        {
            txtNome.Clear();
            txtSobrenome.Clear();
            txtEmail.Clear();
            txtTelefone.Clear();
            txtSenha.Clear(); // Certifique-se que o PasswordBox no XAML chama-se txtSenha
            cbNivelAcesso.SelectedIndex = -1;
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validações Básicas de Preenchimento
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSenha.Password))
            {
                MessageBox.Show("Nome, E-mail e Senha são campos obrigatórios.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Validação de formato de E-mail
            if (!IsEmailValid(txtEmail.Text))
            {
                MessageBox.Show("Por favor, insira um e-mail válido.", "E-mail Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return;
            }

            // 3. Inserção no Banco de Dados
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Nota: Verifique se o nome da tabela no banco é 'Usuarios' ou 'Users' para não dar erro
                    string query = "INSERT INTO Usuarios (Nome, Sobrenome, Email, Telefone, NivelAcesso, Senha) " +
                                   "VALUES (@Nome, @Sobrenome, @Email, @Telefone, @Nivel, @Senha)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Nome", SqlDbType.VarChar).Value = txtNome.Text;
                        cmd.Parameters.Add("@Sobrenome", SqlDbType.VarChar).Value = txtSobrenome.Text;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = txtEmail.Text;
                        cmd.Parameters.Add("@Telefone", SqlDbType.VarChar).Value = txtTelefone.Text;

                        // Captura o texto do ComboBox de forma segura
                        string nivel = (cbNivelAcesso.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Morador";
                        cmd.Parameters.Add("@Nivel", SqlDbType.VarChar).Value = nivel;

                        // Salva a senha com Hash para segurança
                        cmd.Parameters.Add("@Senha", SqlDbType.VarChar).Value = ComputeHash(txtSenha.Password);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Usuário cadastrado com sucesso!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Information);
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro técnico ao salvar: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Métodos de evento vazios (podem ser removidos se não forem usados no XAML)
        private void txtNome_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtSobrenome_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e) { }
        private void txtTelefone_TextChanged(object sender, TextChangedEventArgs e) { }
        private void cbNivelAcesso_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }
}