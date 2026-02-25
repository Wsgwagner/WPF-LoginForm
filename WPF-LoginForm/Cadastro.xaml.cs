using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPF_LoginForm.Views;

namespace WPF_LoginForm
{
    public partial class Cadastro : Window
    {
        private readonly string connectionString = @"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public Cadastro()
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int userId = 0;
            bool isUpdate = int.TryParse(lblId.Text, out userId) && userId > 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = isUpdate
                        ? "UPDATE Users SET Username=@User, Name=@Name, LastName=@LName, Email=@Email WHERE Id=@Id"
                        : "INSERT INTO Users (Username, Password, Name, LastName, Email) VALUES (@User, @Pass, @Name, @LName, @Email)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
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
                            cmd.Parameters.Add("@Pass", SqlDbType.VarChar).Value = ComputeHash(txtPassword.Password);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // --- LÓGICA PARA ABRIR A MAINVIEW ---
                // 1. Cria a instância da nova janela
                MainView principal = new MainView();

                // 2. Exibe a janela principal
                principal.Show();

                // 3. Fecha a janela de cadastro atual
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro técnico: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
        private void btnClose_Click(object sender, RoutedEventArgs e) => this.Close();
        private void btnBack_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}