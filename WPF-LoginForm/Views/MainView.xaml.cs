using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF_LoginForm.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            ShowDashboard(null, null); // Inicia no painel principal
        }

        private void ShowDashboard(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "PAINEL PRINCIPAL";
            MainContent.Content = null;
            ScrollCards.Visibility = Visibility.Visible;
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

        // MÉTODO RESTAURADO PARA GERENCIAR DADOS
        private void OpenManagementWindow(object sender, RoutedEventArgs e)
        {
            txtTituloPagina.Text = "GERENCIAR DADOS";
            ScrollCards.Visibility = Visibility.Collapsed;

            // Aqui você deve carregar sua View de gerenciamento quando criá-la
            // MainContent.Content = new GerenciarDadosView();

            // Exemplo temporário para não dar erro:
            MainContent.Content = new System.Windows.Controls.TextBlock
            {
                Text = "Aqui você poderá editar e excluir seus dados em breve.",
                Foreground = Brushes.Gray,
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}