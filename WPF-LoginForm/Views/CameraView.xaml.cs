using System.Windows;

namespace WPF_LoginForm.Views
{
    public partial class CameraView : Window
    {
        public CameraView(string nomeEquipamento)
        {
            InitializeComponent();
            lblNomeEquip.Text = nomeEquipamento.ToUpper();
        }

        private void CloseClick(object sender, RoutedEventArgs e) => this.Close();

        // Permite arrastar a janela
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) DragMove();
        }
    }
}