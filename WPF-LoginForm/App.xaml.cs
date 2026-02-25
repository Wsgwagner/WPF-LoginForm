using System;
using System.Windows;
using WPF_LoginForm.Views;

namespace WPF_LoginForm
{
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new LoginView();

            // O código "para" aqui até que a LoginView seja fechada ou escondida
            bool? result = loginView.ShowDialog();

            // Se o seu LoginViewModel definir uma propriedade de sucesso ou se você apenas esconder a tela
            if (loginView.IsVisible == false)
            {
                var mainView = new MainView();
                Application.Current.MainWindow = mainView;
                mainView.Show();
                // Não precisamos fechar aqui se o ShowDialog já terminou
            }

            {
                if (loginView.IsVisible == false && loginView.IsLoaded)
                {
                    // Usamos o Dispatcher para garantir que o WPF termine de processar 
                    // a mudança de visibilidade antes de tentar abrir/fechar janelas
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var mainView = new MainView();
                        Application.Current.MainWindow = mainView;
                        mainView.Show();
                        loginView.Close();
                    }));
                }
            }
            ;
        }

    }
}