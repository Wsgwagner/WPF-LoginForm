using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class DashboardView : UserControl
    {
        // 1. Defina a variável strCon dentro da classe para ser acessível
        private string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;Encrypt=False;";

        public DashboardView()
        {
            InitializeComponent();
            // 2. Chama a função dentro do construtor
            CarregarAutomacao();
        }

        private void CarregarAutomacao()
        {
            if (icComodos == null) return;
            var lista = new System.Collections.ObjectModel.ObservableCollection<ComodoVinculo>();

            try
            {
                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT Id, Nome FROM Comodos WHERE ResidenciaId = @rid";
                    var cmd = new System.Data.SqlClient.SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId ?? 1);

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            int idCom = System.Convert.ToInt32(dr["Id"]);
                            var comodo = new ComodoVinculo
                            {
                                Nome = dr["Nome"].ToString(),
                                Equipamentos = new System.Collections.ObjectModel.ObservableCollection<Equipamento>()
                            };

                            using (System.Data.SqlClient.SqlConnection con2 = new System.Data.SqlClient.SqlConnection(strCon))
                            {
                                con2.Open();
                                string sqlEq = "SELECT Nome, Tipo, Status FROM Equipamentos WHERE ComodoId = @cid";
                                var cmdEq = new System.Data.SqlClient.SqlCommand(sqlEq, con2);
                                cmdEq.Parameters.AddWithValue("@cid", idCom);

                                using (var drEq = cmdEq.ExecuteReader())
                                {
                                    while (drEq.Read())
                                    {
                                        comodo.Equipamentos.Add(new Equipamento
                                        {
                                            Nome = drEq["Nome"].ToString(),
                                            Tipo = drEq["Tipo"].ToString(),
                                            // Converte "Desligado" para false, qualquer outra coisa para true
                                            Status = drEq["Status"].ToString().Trim().ToLower() != "desligado",
                                            Intensidade = 0
                                        });
                                    }
                                }
                            }
                            lista.Add(comodo);
                        }
                    }
                }
                icComodos.ItemsSource = lista;
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao carregar o Painel: " + ex.Message);
            }
        }
    }
}
