using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System;

namespace WPF_LoginForm.Views
{
    public partial class MainView : Window
    {
        private readonly string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public MainView()
        {
            InitializeComponent();
            ShowDashboard(null, null);
        }

        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "PAINEL PRINCIPAL";
            MainContent.Content = null;
            ScrollCards.Visibility = Visibility.Visible;
            CarregarCardsRapidos();
        }

        private void CarregarCardsRapidos()
        {
            // Lógica simples para mostrar equipamentos no painel principal
            ObservableCollection<dynamic> lista = new ObservableCollection<dynamic>();
            try
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT Nome, Tipo FROM Equipamentos WHERE ResidenciaId = @rid";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        // Se não estiver logado, usa ID 1 para teste
                        cmd.Parameters.AddWithValue("@rid", SessaoSistema.ResidenciaId <= 0 ? 1 : SessaoSistema.ResidenciaId);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                            while (dr.Read()) lista.Add(new { Nome = dr["Nome"].ToString(), Tipo = dr["Tipo"].ToString() });
                    }
                }
                icEquipamentos.ItemsSource = lista;
            }
            catch { }
        }

        private void OpenManagementWindow(object sender, RoutedEventArgs e)
        {
            // CRÍTICO: Abre como janela independente para disparar o carregamento do banco
            GerenciarDados tela = new GerenciarDados();
            tela.Owner = this;
            tela.ShowDialog();

            // Ao fechar, volta para o dashboard
            ShowDashboard(null, null);
        }

        private void ShowCadastroResidencia(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR RESIDÊNCIA";
            ScrollCards.Visibility = Visibility.Collapsed;
            MainContent.Content = new CadastrarResidencia();
        }

        private void ShowCadastroUsuario(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR USUÁRIO";
            ScrollCards.Visibility = Visibility.Collapsed;
            MainContent.Content = new CadastrarUsuario();
        }

        private void ShowCadastroEquipamento(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR EQUIPAMENTO";
            ScrollCards.Visibility = Visibility.Collapsed;
            MainContent.Content = new CadastrarEquipamento();
        }

        private void ShowComodos(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "MEUS CÔMODOS";
            ScrollCards.Visibility = Visibility.Collapsed;
            MainContent.Content = new ComodosView();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}