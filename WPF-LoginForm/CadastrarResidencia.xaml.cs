using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarResidencia : UserControl
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarResidencia()
        {
            InitializeComponent();
            // Chamamos a busca assim que o componente termina de carregar
            this.Loaded += CadastrarResidencia_Loaded;
        }

        private void CadastrarResidencia_Loaded(object sender, RoutedEventArgs e)
        {
            VerificarResidenciaExistente();
        }

        private void VerificarResidenciaExistente()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Buscamos os dados da residência vinculada ao usuário atual
                    string sql = @"SELECT r.NomeResidencia, r.Endereco, r.Numero, r.Bairro, r.Cidade, r.CodigoVinculo 
                                 FROM Residencias r 
                                 INNER JOIN Users u ON u.ResidenciaId = r.Id 
                                 WHERE u.Id = @userId";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@userId", SessaoSistema.UsuarioId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // Se encontrou, preenche os campos
                                txtNomeResidencia.Text = dr["NomeResidencia"].ToString();
                                txtEndereco.Text = dr["Endereco"].ToString();
                                txtNumero.Text = dr["Numero"].ToString();
                                txtBairro.Text = dr["Bairro"].ToString();
                                txtCidade.Text = dr["Cidade"].ToString();

                                // Fixa os dados na interface
                                FixarDadosInterface(dr["CodigoVinculo"].ToString());

                                // Ajusta o título para indicar visualização
                                if (this.FindName("lblTitulo") is TextBlock lbl)
                                    lbl.Text = "SUA RESIDÊNCIA CADASTRADA";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados existentes: " + ex.Message);
            }
        }

        private string GerarTokenResidencia()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random random = new Random();
            char[] resultado = new char[6];
            for (int i = 0; i < 6; i++)
            {
                resultado[i] = caracteres[random.Next(caracteres.Length)];
            }
            return new string(resultado);
        }

        private void btnSalvarResidencia_Click(object sender, RoutedEventArgs e)
        {
            string nome = txtNomeResidencia.Text.Trim();
            string rua = txtEndereco.Text.Trim();
            string num = txtNumero.Text.Trim();
            string bai = txtBairro.Text.Trim();
            string cid = txtCidade.Text.Trim();
            string codigoGerado = GerarTokenResidencia();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(rua))
            {
                MessageBox.Show("Campos obrigatórios vazios!");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlTransaction transacao = con.BeginTransaction())
                    {
                        try
                        {
                            string sqlRes = @"INSERT INTO Residencias (NomeResidencia, Endereco, Numero, Bairro, Cidade, CodigoVinculo) 
                                             VALUES (@nome, @rua, @num, @bai, @cid, @cod); 
                                             SELECT SCOPE_IDENTITY();";

                            int idResidencia;
                            using (SqlCommand cmd = new SqlCommand(sqlRes, con, transacao))
                            {
                                cmd.Parameters.AddWithValue("@nome", nome);
                                cmd.Parameters.AddWithValue("@rua", rua);
                                cmd.Parameters.AddWithValue("@num", num);
                                cmd.Parameters.AddWithValue("@bai", bai);
                                cmd.Parameters.AddWithValue("@cid", cid);
                                cmd.Parameters.AddWithValue("@cod", codigoGerado);
                                idResidencia = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            string sqlUser = "UPDATE Users SET ResidenciaId = @resId WHERE Id = @userId";
                            using (SqlCommand cmdUser = new SqlCommand(sqlUser, con, transacao))
                            {
                                cmdUser.Parameters.AddWithValue("@resId", idResidencia);
                                cmdUser.Parameters.AddWithValue("@userId", SessaoSistema.UsuarioId);
                                cmdUser.ExecuteNonQuery();
                            }

                            transacao.Commit();
                            SessaoSistema.ResidenciaId = idResidencia;
                            SessaoSistema.NomeResidencia = nome;

                            FixarDadosInterface(codigoGerado);
                            MessageBox.Show("Residência vinculada!");
                        }
                        catch { transacao.Rollback(); throw; }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void FixarDadosInterface(string codigo)
        {
            txtNomeResidencia.IsReadOnly = true;
            txtEndereco.IsReadOnly = true;
            txtNumero.IsReadOnly = true;
            txtBairro.IsReadOnly = true;
            txtCidade.IsReadOnly = true;

            txtNomeResidencia.Foreground = Brushes.LightGreen;
            txtCodigoVinculo.Text = codigo;
            borderCodigo.Visibility = Visibility.Visible;
            btnSalvar.IsEnabled = false;
            btnSalvar.Content = "RESIDÊNCIA VINCULADA";
        }
    }
}