using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF_LoginForm.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // O [CallerMemberName] identifica automaticamente qual propriedade chamou o método
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}