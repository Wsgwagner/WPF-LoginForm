using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarResidencia : UserControl
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarResidencia()
        {
            InitializeComponent();
        }

        private string GerarToken()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random random = new Random();
            char[] token = new char[6];
            for (int i = 0; i < 6; i++)
            {
                token[i] = chars[random.Next(chars.Length)];
            }
            return new string(token);
        }

        private void btnSalvarResidencia_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNomeResidencia.Text.Trim();
            string rua = txtEndereco.Text.Trim();
            string numero = txtNumero.Text.Trim();
            string bairro = txtBairro.Text.Trim();
            string cidade = txtCidade.Text.Trim();
            string codigo = GerarToken();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(rua))
            {
                MessageBox.Show("Nome e Logradouro são campos obrigatórios!");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            // 1. Inserir Residência e obter o ID gerado
                            string sqlRes = @"INSERT INTO Residencias (NomeResidencia, Endereco, Numero, Bairro, Cidade, CodigoVinculo) 
                                             VALUES (@nome, @rua, @num, @bai, @cid, @cod); 
                                             SELECT SCOPE_IDENTITY();";

                            int idGerado;
                            using (SqlCommand cmd = new SqlCommand(sqlRes, con, tran))
                            {
                                cmd.Parameters.AddWithValue("@nome", nome);
                                cmd.Parameters.AddWithValue("@rua", rua);
                                cmd.Parameters.AddWithValue("@num", numero);
                                cmd.Parameters.AddWithValue("@bai", bairro);
                                cmd.Parameters.AddWithValue("@cid", cidade);
                                cmd.Parameters.AddWithValue("@cod", codigo);
                                idGerado = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // 2. Vincular o usuário da sessão à residência
                            string sqlUser = "UPDATE Users SET ResidenciaId = @resId WHERE Id = @userId";
                            using (SqlCommand cmdUser = new SqlCommand(sqlUser, con, tran))
                            {
                                cmdUser.Parameters.AddWithValue("@resId", idGerado);
                                cmdUser.Parameters.AddWithValue("@userId", SessaoSistema.UsuarioId);
                                cmdUser.ExecuteNonQuery();
                            }

                            tran.Commit();

                            // 3. Atualizar Sessão e UI
                            SessaoSistema.ResidenciaId = idGerado;
                            SessaoSistema.NomeResidencia = nome;

                            txtCodigoVinculo.Text = codigo;
                            borderCodigo.Visibility = Visibility.Visible;
                            btnSalvar.IsEnabled = false;

                            MessageBox.Show("Residência registrada com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            MessageBox.Show("Erro ao processar transação: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro de conexão: " + ex.Message);
            }
        }
    }
}