using System;
using System.Windows;
using System.Windows.Input;

namespace WPF_LoginForm.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            // Inicia o sistema carregando o Painel Principal
            ShowDashboard(null, null);
        }

        // 1. Método para o Painel Principal
        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            try
            {
                txtTituloPagina.Text = "CENTRAL DE AUTOMAÇÃO";
                txtSubtitulo.Text = "Gerencie seus cômodos e dispositivos em tempo real.";

                // Injeta a DashboardView no ContentControl
                MainContent.Content = new DashboardView();
            }
            catch (Exception ex)
            {
                // Usando a variável 'ex' para detalhar o erro e remover o aviso do compilador
                MessageBox.Show("Erro ao carregar o Painel: " + ex.Message);
            }
        }

        // 2. Método para Cadastrar Residência
        private void ShowCadastroResidencia(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR RESIDÊNCIA";
            txtSubtitulo.Text = "Adicione uma nova unidade ao sistema.";
            MainContent.Content = new CadastrarResidencia();
        }

        // 3. Método para Cadastrar Usuário
        private void ShowCadastroUsuario(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR USUÁRIO";
            txtSubtitulo.Text = "Gerencie moradores e níveis de acesso.";
            MainContent.Content = new CadastrarUsuario();
        }

        // 4. Método para Cadastrar Equipamento
        private void ShowCadastroEquipamento(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "CADASTRAR EQUIPAMENTO";
            txtSubtitulo.Text = "Adicione novos dispositivos inteligentes.";
            MainContent.Content = new CadastrarEquipamento();
        }

        // 5. Método para Meus Cômodos
        private void ShowComodos(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "MEUS CÔMODOS";
            txtSubtitulo.Text = "Visualize e organize seus ambientes.";
            MainContent.Content = new ComodosView();
        }

        // 6. Método para Gerenciar Dados (Janela Independente)
        private void OpenManagementWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                GerenciarDados tela = new GerenciarDados();
                tela.Owner = this;
                tela.ShowDialog();

                // Recarrega o dashboard ao fechar a tela de gerenciamento
                ShowDashboard(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir gerenciamento: " + ex.Message);
            }
        }

        // Métodos Utilitários
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}