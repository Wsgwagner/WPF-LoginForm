using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarUsuario : UserControl
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarUsuario()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                // Bloqueio de Segurança: Valida se o Wagner é 'Administrador'
                if (SessaoSistema.AccessLevel != "Administrador")
                {
                    MessageBox.Show("Acesso Negado! Somente o Administrador pode cadastrar usuários.", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Stop);
                    this.IsEnabled = false;
                }
            };
        }

        private void BtnSalvarFamiliar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // CORREÇÃO: Usando 'CodigoVinculo' conforme sua tabela Residencias
                    string sqlBusca = "SELECT [Id] FROM [Residencias] WHERE [CodigoVinculo] = @cod";

                    int? resId = null;
                    using (SqlCommand cmdBusca = new SqlCommand(sqlBusca, con))
                    {
                        cmdBusca.Parameters.AddWithValue("@cod", txtCodigoVinculo.Text.Trim());
                        var result = cmdBusca.ExecuteScalar();
                        if (result != null) resId = Convert.ToInt32(result);
                    }

                    if (resId == null)
                    {
                        MessageBox.Show("Código de Residência inválido!");
                        return;
                    }

                    // Insert corrigido com colchetes para evitar erro em [Name]
                    string sqlInsert = @"INSERT INTO [Users] ([Username], [Password], [Name], [ResidenciaId], [AccessLevel]) 
                                       VALUES (@user, @pass, @name, @resId, 'Morador')";

                    using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                    {
                        cmd.Parameters.AddWithValue("@user", txtEmailUser.Text);
                        cmd.Parameters.AddWithValue("@pass", "HASH_DA_SENHA");
                        cmd.Parameters.AddWithValue("@name", txtNomeUser.Text);
                        cmd.Parameters.AddWithValue("@resId", resId);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Usuário cadastrado com sucesso!");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }
    }
}