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
        // Certifique-se de que esta string é a mesma que funciona na sua MainView
        private readonly string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";
        //@"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public ComodosView()
        {
            InitializeComponent();
            CarregarDadosDoBanco();
        }

        private void btnAtualizar_Click(object sender, RoutedEventArgs e)
        {
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
                            // O uso do operador ? evita erros se o campo no banco estiver nulo
                            Nome = reader["Nome"]?.ToString() ?? "Sem Nome",
                            Tipo = reader["Tipo"]?.ToString() ?? "N/A",
                            Comodo = reader["Comodo"]?.ToString() ?? "Geral",
                            Status = reader["Status"]?.ToString() ?? "Off"
                        });
                    }
                }

                // Agrupamento LINQ: Cria a estrutura que o seu XAML espera { NomeComodo, Equipamentos }
                var comodosAgrupados = listaEquipamentos
                    .GroupBy(e => e.Comodo)
                    .Select(g => new
                    {
                        NomeComodo = g.Key.ToUpper(),
                        Equipamentos = g.ToList()
                    }).ToList();

                // TRUQUE DO WPF: Resetar o ItemsSource antes de atribuir o novo
                icComodos.ItemsSource = null;
                icComodos.ItemsSource = comodosAgrupados;

                if (comodosAgrupados.Count == 0)
                {
                    MessageBox.Show("Banco de dados acessado, mas nenhum equipamento foi encontrado.", "Aviso");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar cômodos: " + ex.Message, "Erro SQL");
            }
        }
    }

    // Modelo de classe - Certifique-se de que não há duplicatas desta classe no projeto
    public class Equipamento
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Comodo { get; set; }
        public string Status { get; set; }
    }
}