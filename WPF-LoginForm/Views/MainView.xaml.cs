using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_LoginForm.Views
{
    public partial class MainView : Window
    {
        // Lista que armazena os equipamentos cadastrados para aparecerem no Painel
        public ObservableCollection<EquipamentoModel> MeusEquipamentos { get; set; }

        public MainView()
        {
            InitializeComponent();
            MeusEquipamentos = new ObservableCollection<EquipamentoModel>();

            // Conecta a lista ao ItemsControl do Dashboard (se você usou o nome ItemsDashboard no XML)
            // ItemsDashboard.ItemsSource = MeusEquipamentos;
        }

        // Permite arrastar a janela sem bordas
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // --- MÉTODOS DE NAVEGAÇÃO ---

        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "PAINEL PRINCIPAL";
            // Lógica para mostrar os cards dos equipamentos
            // Se estiver usando ContentControl, você injetaria a View aqui
        }

        private void ShowCadastroUsuario(object sender, RoutedEventArgs e)
        {
            // 1. Atualiza o título da página para o usuário saber onde está
            txtTituloPagina.Text = "CADASTRO DE USUÁRIOS";

            // 2. Instancia a tela que você criou (UserControl)
            CadastrarUsuario telaCadastro = new CadastrarUsuario();

            // 3. Coloca essa tela dentro do ContentControl chamado 'MainContent'
            MainContent.Content = telaCadastro;
        }

        private void ShowCadastroEquipamento(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CONFIGURAÇÃO DE DISPOSITIVOS";
            // Injeta a nova View dentro do ContentControl central
            MainContent.Content = new CadastrarEquipamento();
        }

        private void ShowComodos(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "VISÃO POR CÔMODOS";
            MainContent.Content = new ComodosView(); // Instancia a tela nova
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // --- LÓGICA DE CADASTRO (EXEMPLO) ---

        private void BtnSalvarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Aqui você adicionaria a lógica com o SqlCommand para salvar no SQL Server
            try
            {
                MessageBox.Show("Usuário cadastrado com sucesso!", "IA HOME", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowDashboard(null, null); // Volta para o painel após salvar
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }
    }

    // Classe simples para representar o equipamento
    public class EquipamentoModel
    {
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public string Comodo { get; set; }
        public string Status { get; set; }
    }
}