using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    public partial class ComodosView : UserControl
    {
        private readonly string connectionString = @"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public ComodosView()
        {
            InitializeComponent();
            CarregarDadosDoBanco();
        }

        private void CarregarDadosDoBanco()
        {
            List<Equipamento> listaEquipamentos = new List<Equipamento>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Nome, Tipo, Comodo, Status FROM Equipamentos";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        listaEquipamentos.Add(new Equipamento
                        {
                            Nome = reader["Nome"].ToString(),
                            Tipo = reader["Tipo"].ToString(),
                            Comodo = reader["Comodo"].ToString(),
                            Status = reader["Status"].ToString()
                        });
                    }
                }

                // Agrupando por cômodo usando LINQ para o layout
                var comodosAgrupados = listaEquipamentos
                    .GroupBy(e => e.Comodo)
                    .Select(g => new
                    {
                        NomeComodo = g.Key.ToUpper(),
                        Equipamentos = g.ToList()
                    }).ToList();

                icComodos.ItemsSource = comodosAgrupados;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao carregar cômodos: " + ex.Message);
            }
        }
    }

    public class Equipamento
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Comodo { get; set; }
        public string Status { get; set; }
    }
}