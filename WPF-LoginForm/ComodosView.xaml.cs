using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class ComodosView : UserControl
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public ComodosView()
        {
            InitializeComponent();
            CarregarComodosEEquipamentos();
        }

        private void btnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarComodosEEquipamentos();
        }

        public void CarregarComodosEEquipamentos()
        {
            if (SessaoSistema.ResidenciaId == null) return;

            try
            {
                var listaPlana = new List<dynamic>();

                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    // Buscamos todos os equipamentos da residência do usuário
                    string sql = "SELECT Nome, Tipo, Comodo, Status FROM Equipamentos WHERE ResidenciaId = @resId";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@resId", SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listaPlana.Add(new
                                {
                                    Nome = dr["Nome"].ToString(),
                                    Tipo = dr["Tipo"].ToString(),
                                    Comodo = dr["Comodo"].ToString(),
                                    Status = dr["Status"].ToString()
                                });
                            }
                        }
                    }
                }

                // A MÁGICA DO AGRUPAMENTO:
                // Pegamos a lista do banco e agrupamos pelo nome do cômodo
                var dadosAgrupados = listaPlana
                    .GroupBy(x => x.Comodo)
                    .Select(g => new ComodoAgrupado
                    {
                        NomeComodo = g.Key,
                        Equipamentos = g.Select(e => new EquipamentoSimples
                        {
                            Nome = e.Nome,
                            Tipo = e.Tipo,
                            Status = e.Status
                        }).ToList()
                    }).ToList();

                // Enviamos para o ItemsControl do XAML
                icComodos.ItemsSource = dadosAgrupados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar cômodos: " + ex.Message);
            }
        }
    }
}