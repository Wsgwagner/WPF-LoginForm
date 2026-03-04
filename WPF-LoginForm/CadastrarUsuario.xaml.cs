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
        // Mantendo sua string de conexão
        private readonly string conexaoDb = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarUsuario()
        {
            InitializeComponent();
        }

        private void BtnSalvarFamiliar_Click(object sender, RoutedEventArgs e)
        {
            // Validação do campo de código
            if (string.IsNullOrWhiteSpace(txtCodigoVinculo.Text))
            {
                MessageBox.Show("O código da residência é obrigatório para o vínculo!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(conexaoDb))
                {
                    con.Open();

                    // 1. Buscar o ID da residência pelo código de vínculo
                    string sqlBusca = "SELECT [Id] FROM [Residencias] WHERE [CodigoVinculo] = @cod";
                    int? resIdEncontrado = null;

                    using (SqlCommand cmdBusca = new SqlCommand(sqlBusca, con))
                    {
                        cmdBusca.Parameters.AddWithValue("@cod", txtCodigoVinculo.Text.Trim());
                        var result = cmdBusca.ExecuteScalar();

                        if (result != null)
                            resIdEncontrado = Convert.ToInt32(result);
                    }

                    if (resIdEncontrado == null)
                    {
                        MessageBox.Show("Código de Residência não encontrado! Verifique com o administrador da casa.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 2. Inserir o novo familiar usando colchetes [] para evitar erro na coluna [Name]
                    string sqlInsert = @"INSERT INTO [Users] ([Username], [Password], [Email], [ResidenciaId], [Phone], [Name], [LastName], [AccessLevel]) 
                                       VALUES (@user, @pass, @email, @resId, @phone, @name, @last, 'Morador')";

                    using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                    {
                        cmd.Parameters.AddWithValue("@user", txtEmailUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", HashFamiliar(txtSenhaUser.Password));
                        cmd.Parameters.AddWithValue("@email", txtEmailUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@resId", resIdEncontrado);
                        cmd.Parameters.AddWithValue("@phone", txtPhoneUser.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtNomeUser.Text.Trim()); // Aqui está a coluna Name
                        cmd.Parameters.AddWithValue("@last", txtSobrenomeUser.Text.Trim());

                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"{txtNomeUser.Text} cadastrado e vinculado com sucesso!", "IA Home", MessageBoxButton.OK, MessageBoxImage.Information);
                        LimparCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar no banco de dados: " + ex.Message, "Erro Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LimparCampos()
        {
            txtNomeUser.Clear();
            txtSobrenomeUser.Clear();
            txtEmailUser.Clear();
            txtPhoneUser.Clear();
            txtCodigoVinculo.Clear();
            txtSenhaUser.Clear();
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