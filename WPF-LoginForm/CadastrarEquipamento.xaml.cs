using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class CadastrarEquipamento : UserControl
    {
        // String de conexão configurada para o seu banco local
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public CadastrarEquipamento()
        {
            InitializeComponent();
            this.Loaded += CadastrarEquipamento_Loaded;
        }

        private void CadastrarEquipamento_Loaded(object sender, RoutedEventArgs e)
        {
            // Verifica permissão usando a classe SessaoSistema
            if (SessaoSistema.AccessLevel != "Administrador")
            {
                BloquearCampos();
                return;
            }

            // Carrega os cômodos já cadastrados para facilitar a seleção
            PreencherComboBoxComodos();
        }

        private void BloquearCampos()
        {
            txtNomeEquip.IsEnabled = false;
            cbTipo.IsEnabled = false;
            cbComodo.IsEnabled = false;
            BtnSalvar.IsEnabled = false;
            BtnSalvar.Content = "BLOQUEADO (APENAS ADM)";
            BtnSalvar.Opacity = 0.5;
        }

        private void PreencherComboBoxComodos()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Filtra os cômodos pela residência da sessão
                    string sql = "SELECT Nome FROM Comodos WHERE ResidenciaId = @rid ORDER BY Nome ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        // Tratando o ResidenciaId nulo da SessaoSistema
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId ?? 1);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            cbComodo.Items.Clear();
                            while (dr.Read())
                            {
                                cbComodo.Items.Add(dr["Nome"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar lista de cômodos: " + ex.Message);
            }
        }

        private void BtnSalvarEquipamento_Click(object sender, RoutedEventArgs e)
        {
            if (SessaoSistema.AccessLevel != "Administrador") return;

            if (string.IsNullOrWhiteSpace(txtNomeEquip.Text) || string.IsNullOrWhiteSpace(cbComodo.Text))
            {
                MessageBox.Show("Preencha o nome do dispositivo e o cômodo!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string nomeEquip = txtNomeEquip.Text.Trim();
            string tipoEquip = cbTipo.Text.Trim();
            string nomeComodo = cbComodo.Text.Trim();
            // Garante um valor inteiro para o ID da residência
            int resId = SessaoSistema.ResidenciaId ?? 1;

            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    int idComodoFinal;

                    // PASSO 1: Verifica ou Cria o Cômodo e retorna o ID
                    string sqlComodo = @"
                        IF EXISTS (SELECT Id FROM Comodos WHERE Nome = @nomeC AND ResidenciaId = @rid)
                            SELECT Id FROM Comodos WHERE Nome = @nomeC AND ResidenciaId = @rid;
                        ELSE
                        BEGIN
                            INSERT INTO Comodos (Nome, ResidenciaId) VALUES (@nomeC, @rid);
                            SELECT SCOPE_IDENTITY();
                        END";

                    using (SqlCommand cmdC = new SqlCommand(sqlComodo, con, trans))
                    {
                        cmdC.Parameters.AddWithValue("@nomeC", nomeComodo);
                        cmdC.Parameters.AddWithValue("@rid", resId);

                        // Correção da conversão explícita para evitar o erro de int?
                        object resultado = cmdC.ExecuteScalar();
                        idComodoFinal = (resultado != null && resultado != DBNull.Value)
                                        ? Convert.ToInt32(resultado)
                                        : 0;
                    }

                    // PASSO 2: Insere o Equipamento vinculado ao ComodoId
                    string sqlEquip = @"
                        INSERT INTO Equipamentos (Nome, Tipo, Comodo, Status, ResidenciaId, ComodoId) 
                        VALUES (@nomeE, @tipo, @comodoTxt, 'Desligado', @rid, @cid)";

                    using (SqlCommand cmdE = new SqlCommand(sqlEquip, con, trans))
                    {
                        cmdE.Parameters.AddWithValue("@nomeE", nomeEquip);
                        cmdE.Parameters.AddWithValue("@tipo", tipoEquip);
                        cmdE.Parameters.AddWithValue("@comodoTxt", nomeComodo);
                        cmdE.Parameters.AddWithValue("@rid", resId);
                        cmdE.Parameters.AddWithValue("@cid", idComodoFinal);

                        cmdE.ExecuteNonQuery();
                    }

                    trans.Commit();
                    MessageBox.Show("Dispositivo e Cômodo cadastrados com sucesso!", "IA HOME");

                    txtNomeEquip.Clear();
                    PreencherComboBoxComodos();
                }
                catch (Exception ex)
                {
                    // Uso da variável 'ex' para remover o aviso de código
                    if (con.State == ConnectionState.Open) trans.Rollback();
                    MessageBox.Show("Erro ao salvar: " + ex.Message, "IA HOME - Erro Crítico");
                }
            }
        }
    }
}