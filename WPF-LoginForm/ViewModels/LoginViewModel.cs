using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using WPF_LoginForm.Views;

namespace WPF_LoginForm.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object obj)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Preencha os campos!";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string sql = "SELECT Id, Name, Password, ResidenciaId FROM Users WHERE Username = @user";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@user", Username);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                if (VerifyHash(Password, dr["Password"].ToString()))
                                {
                                    SessaoSistema.UsuarioId = (int)dr["Id"];
                                    SessaoSistema.Nome = dr["Name"].ToString();
                                    SessaoSistema.ResidenciaId = dr["ResidenciaId"] as int?;

                                    MainView main = new MainView();
                                    main.Show();
                                    Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is LoginView)?.Close();
                                }
                                else ErrorMessage = "Senha incorreta!";
                            }
                            else ErrorMessage = "Usuário não encontrado!";
                        }
                    }
                }
            }
            catch (Exception ex) { ErrorMessage = "Erro: " + ex.Message; }
        }

        private bool VerifyHash(string input, string hash)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString().Equals(hash, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}