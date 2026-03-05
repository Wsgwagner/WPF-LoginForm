using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Drawing; 
using Emgu.CV;         
using Emgu.CV.Structure;

namespace WPF_LoginForm.Views
{
    public partial class CameraView : Window
    {
        private VideoCapture _capture;
        private DispatcherTimer _timer;

        public CameraView(string nomeDispositivo)
        {
            InitializeComponent();
            lblNomeEquip.Text = nomeDispositivo.ToUpper();
            IniciarCamera();
        }

        private void IniciarCamera()
        {
            try
            {
                _capture = new VideoCapture(0);
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(30);
                _timer.Tick += ProcessFrame;
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao acessar câmera: " + ex.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            // Mat e VideoCapture agora serão reconhecidos com o using Emgu.CV
            using (Mat frame = _capture.QueryFrame())
            {
                if (frame != null)
                {
                    imgDisplay.Source = ToBitmapSource(frame);
                }
            }
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource ToBitmapSource(Mat frame)
        {
            // .ToBitmap() requer o pacote Emgu.CV.Bitmap
            using (Bitmap bitmap = frame.ToBitmap())
            {
                IntPtr ptr = bitmap.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                finally
                {
                    DeleteObject(ptr);
                }
            }
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
            _capture?.Dispose();
            this.Close();
        }
    }
}