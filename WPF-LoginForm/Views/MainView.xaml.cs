using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;

namespace WPF_LoginForm.Views
{
    public partial class MainView : Window
    {
        // Lista ligada à interface (Cards do Dashboard)
        public ObservableCollection<EquipamentoModel> MeusEquipamentos { get; set; }

        // String de Conexão corrigida
        private string strCon = @"Server=(localdb)\MSSQLLocalDB;Database=WpfLogin;User Id=tds;Password=tds123;TrustServerCertificate=True;Encrypt=False;";

        public MainView()
        {
            InitializeComponent();
            MeusEquipamentos = new ObservableCollection<EquipamentoModel>();

            // Vinculação inicial
            if (ItemsDashboard != null)
            {
                ItemsDashboard.ItemsSource = MeusEquipamentos;
            }

            // Carrega dados ao abrir o app
            CarregarDadosDoBanco();
            AtualizarTextoVazio();
        }

        // --- BUSCA DE DADOS NO SQL SERVER ---
        private void CarregarDadosDoBanco()
        {
            try
            {
                MeusEquipamentos.Clear();

                using (SqlConnection conexao = new SqlConnection(strCon))
                {
                    conexao.Open();
                    string query = "SELECT Nome, Tipo, Comodo, Status FROM Equipamentos";
                    SqlCommand comando = new SqlCommand(query, conexao);
                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        MeusEquipamentos.Add(new EquipamentoModel
                        {
                            Nome = reader["Nome"].ToString(),
                            Tipo = reader["Tipo"].ToString(),
                            Comodo = reader["Comodo"].ToString(),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
                AtualizarTextoVazio();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar com o Banco de Dados: " + ex.Message, "Erro SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- NAVEGAÇÃO E VISUALIZAÇÃO ---

        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "PAINEL PRINCIPAL";

            // 1. Limpa o ContentControl (remove a tela de cadastro/comodos que está na frente)
            MainContent.Content = null;

            // 2. Recarrega e exibe todos os itens no Dashboard principal
            CarregarDadosDoBanco();
            ItemsDashboard.ItemsSource = MeusEquipamentos;
        }

        private void ShowComodos(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "VISÃO POR CÔMODOS";

            // 1. Injeta o UserControl especializado em cômodos (o que criamos antes)
            MainContent.Content = new ComodosView();

            // Nota: O código de ordenação que estava aqui foi removido porque 
            // a ComodosView já faz o agrupamento e ordenação por conta própria.
        }

        private void ShowCadastroUsuario(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRO DE USUÁRIOS";
            MainContent.Content = new CadastrarUsuario();
        }

        private void ShowCadastroEquipamento(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CONFIGURAÇÃO DE DISPOSITIVOS";
            MainContent.Content = new CadastrarEquipamento();
        }

        // --- GERENCIAMENTO (JANELA EXTERNA) ---

        private void GerenciarDados_Click(object sender, RoutedEventArgs e)
        {
            GerenciarDados telaGerenciar = new GerenciarDados();
            telaGerenciar.ShowDialog();

            // Ao fechar a janela, recarrega o dashboard principal
            CarregarDadosDoBanco();
        }

        // --- AÇÕES DOS CARDS (EDITAR / EXCLUIR) ---

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as EquipamentoModel;
            if (item != null)
            {
                MessageBox.Show($"Editando dispositivo: {item.Nome}\nLocalizado em: {item.Comodo}", "IA HOME");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.Tag as EquipamentoModel;
            if (item != null)
            {
                var result = MessageBox.Show($"Deseja realmente excluir '{item.Nome}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    ExcluirDoBanco(item.Nome);
                    CarregarDadosDoBanco();
                }
            }
        }

        private void ExcluirDoBanco(string nome)
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(strCon))
                {
                    conexao.Open();
                    string query = "DELETE FROM Equipamentos WHERE Nome = @nome";
                    SqlCommand cmd = new SqlCommand(query, conexao);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao excluir: " + ex.Message); }
        }

        // --- UTILITÁRIOS ---

        private void AtualizarTextoVazio()
        {
            if (txtVazio != null)
                txtVazio.Visibility = MeusEquipamentos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void OpenManagementWindow(object sender, RoutedEventArgs e) => GerenciarDados_Click(sender, e);
    }

    public class EquipamentoModel
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Comodo { get; set; }
        public string Status { get; set; }
    }
}