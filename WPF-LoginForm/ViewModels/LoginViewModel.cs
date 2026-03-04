using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using WPF_LoginForm.Views;

namespace WPF_LoginForm.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
        }

        private bool CanExecuteLoginCommand(object obj) => !string.IsNullOrWhiteSpace(Username) && Password?.Length >= 3;

        private void ExecuteLoginCommand(object obj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Busca o AccessLevel para destravar o acesso do Wagner como ADM
                    string sql = "SELECT Id, Name, AccessLevel, ResidenciaId FROM Users WHERE (Username = @login OR Email = @login) AND Password = @pass";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@login", Username);
                        cmd.Parameters.AddWithValue("@pass", GerarHash(Password));

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Alimenta a SessaoSistema
                                SessaoSistema.UsuarioId = Convert.ToInt32(dr["Id"]);
                                SessaoSistema.Nome = dr["Name"].ToString();
                                SessaoSistema.AccessLevel = dr["AccessLevel"].ToString();
                                SessaoSistema.ResidenciaId = dr["ResidenciaId"] as int?;

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    // Certifique-se que o nome da sua tela principal é MainWindow
                                    var principal = new MainView();
                                    principal.Show();

                                    // Fecha a janela de login
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window is LoginView) window.Close();
                                    }
                                });
                            }
                            else { ErrorMessage = "* Usuário ou senha incorretos."; }
                        }
                    }
                }
            }
            catch (Exception ex) { ErrorMessage = "* Erro: " + ex.Message; }
        }

        private string GerarHash(string senha)
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