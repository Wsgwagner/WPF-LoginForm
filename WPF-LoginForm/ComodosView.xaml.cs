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
                var listaPlana = new List<EquipamentoParaAgrupar>();

                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT Nome, Tipo, Comodo, Status FROM Equipamentos WHERE ResidenciaId = @resId";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@resId", SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listaPlana.Add(new EquipamentoParaAgrupar
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

                // CORREÇÃO DA SINTAXE LINQ (Evita erro de Tupla e g não existe)
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

                icComodos.ItemsSource = dadosAgrupados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar cômodos: " + ex.Message, "IA HOME", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Classes renomeadas internamente para evitar ambiguidade com o resto do projeto
    public class ComodoAgrupado
    {
        public string NomeComodo { get; set; }
        public List<EquipamentoSimples> Equipamentos { get; set; }
    }

    public class EquipamentoSimples
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Status { get; set; }
    }

    public class EquipamentoParaAgrupar : EquipamentoSimples
    {
        public string Comodo { get; set; }
    }
}