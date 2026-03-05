using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class DashboardView : UserControl
    {
        private string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;Encrypt=False;";

        public DashboardView()
        {
            InitializeComponent();
            CarregarAutomacao();
        }

        private void CarregarAutomacao()
        {
            if (icComodos == null) return;
            var lista = new ObservableCollection<ComodoVinculo>();

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // 1. Busca os cômodos vinculados à residência
                    string sql = "SELECT Id, Nome FROM Comodos WHERE ResidenciaId = @rid";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId ?? 1);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int idCom = Convert.ToInt32(dr["Id"]);
                            var comodo = new ComodoVinculo
                            {
                                Nome = dr["Nome"].ToString(),
                                Equipamentos = new ObservableCollection<Equipamento>()
                            };

                            using (SqlConnection con2 = new SqlConnection(strCon))
                            {
                                con2.Open();
                                // 2. Busca apenas os equipamentos que AINDA EXISTEM no banco
                                string sqlEq = "SELECT Nome, Tipo, Status, Intensidade FROM Equipamentos WHERE ComodoId = @cid";
                                SqlCommand cmdEq = new SqlCommand(sqlEq, con2);
                                cmdEq.Parameters.AddWithValue("@cid", idCom);

                                using (SqlDataReader drEq = cmdEq.ExecuteReader())
                                {
                                    while (drEq.Read())
                                    {
                                        comodo.Equipamentos.Add(new Equipamento
                                        {
                                            Nome = drEq["Nome"].ToString(),
                                            Tipo = drEq["Tipo"].ToString(),
                                            Status = drEq["Status"].ToString().Trim().ToLower() != "desligado",
                                            Intensidade = drEq["Intensidade"] != DBNull.Value ? Convert.ToInt32(drEq["Intensidade"]) : 0
                                        });
                                    }
                                }
                            }

                            // 3. REGRA DE EXCLUSÃO VISUAL: Só adiciona o cômodo se ele tiver equipamentos ativos
                            if (comodo.Equipamentos.Count > 0)
                            {
                                lista.Add(comodo);
                            }
                        }
                    }
                }
                // 4. Atualiza a UI: se o item saiu do banco, ele morre aqui no ItemsSource
                icComodos.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao sincronizar dashboard: " + ex.Message);
            }
        }

        private void BtnAcesso_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string nomeEquip = btn.Tag?.ToString();
            if (string.IsNullOrEmpty(nomeEquip)) return;

            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sqlBusca = "SELECT Status FROM Equipamentos WHERE Nome = @nome AND ResidenciaId = @rid";
                    SqlCommand cmdB = new SqlCommand(sqlBusca, con);
                    cmdB.Parameters.AddWithValue("@nome", nomeEquip);
                    cmdB.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId ?? 1);

                    string statusAtual = cmdB.ExecuteScalar()?.ToString();
                    string novoStatus = (statusAtual == "Ligado") ? "Desligado" : "Ligado";

                    string sqlUpd = "UPDATE Equipamentos SET Status = @novo WHERE Nome = @nome AND ResidenciaId = @rid";
                    SqlCommand cmdU = new SqlCommand(sqlUpd, con);
                    cmdU.Parameters.AddWithValue("@novo", novoStatus);
                    cmdU.Parameters.AddWithValue("@nome", nomeEquip);
                    cmdU.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId ?? 1);
                    cmdU.ExecuteNonQuery();

                    MessageBox.Show($"{nomeEquip} alterado para {novoStatus}!", "IA HOME");
                    CarregarAutomacao(); // Atualiza a tela
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro no comando: " + ex.Message); }
        }

        private void BtnCamera_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            CameraView winCam = new CameraView(btn.Tag?.ToString() ?? "Câmera");
            winCam.Owner = Window.GetWindow(this);
            winCam.ShowDialog();
        }
    }
}