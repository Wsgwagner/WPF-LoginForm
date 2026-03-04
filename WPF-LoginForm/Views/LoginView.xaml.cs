using System;
using System.Windows;
using System.Windows.Input;

namespace WPF_LoginForm.Views
{
    public partial class LoginView : Window
    {
        public LoginView() => InitializeComponent();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void btnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void btnCadastro_Click_1(object sender, RoutedEventArgs e)
        {
            // Abre a tela de Cadastro (certifique-se de que o nome da classe está correto)
            WPF_LoginForm.Cadastro tela = new WPF_LoginForm.Cadastro();
            tela.ShowDialog();
        }

        private void btnEsqueciSenha_Click(object sender, RoutedEventArgs e)
        {
            // Abre a tela de RecuperarSenha que está na pasta Views
            RecuperarSenha tela = new RecuperarSenha();
            tela.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}