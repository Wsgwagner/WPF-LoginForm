using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.CustomControls
{
    public partial class BindablePasswordBox : UserControl
    {
        // Propriedade que o ViewModel vai acessar
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(BindablePasswordBox),
                new PropertyMetadata(string.Empty));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public BindablePasswordBox()
        {
            // Criação manual do controle para simplificar
            PasswordBox passwordBox = new PasswordBox();
            passwordBox.PasswordChanged += OnPasswordChanged;
            passwordBox.Background = System.Windows.Media.Brushes.Transparent;
            passwordBox.Foreground = System.Windows.Media.Brushes.White;
            passwordBox.BorderThickness = new Thickness(0, 0, 0, 2);
            passwordBox.BorderBrush = System.Windows.Media.Brushes.DarkGray;

            this.Content = passwordBox;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
        }

        internal void Clear()
        {
            throw new NotImplementedException();
        }
    }
}