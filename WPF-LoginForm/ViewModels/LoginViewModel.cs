using System;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Windows.Input;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        // Campos (Fields)
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isViewVisible = true;

        // 1. ADICIONE AQUI A INSTÂNCIA DO REPOSITÓRIO
        private IUserRepository _userRepository;

        // Propriedades
        public string Username { get => _username; set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public string Password { get => _password; set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        public bool IsViewVisible { get => _isViewVisible; set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); } }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            // 2. INICIALIZE O REPOSITÓRIO NO CONSTRUTOR
            _userRepository = new UserRepository();
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            return !string.IsNullOrWhiteSpace(Username) && Username.Length >= 3 &&
                   !string.IsNullOrWhiteSpace(Password) && Password.Length >= 3;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = _userRepository.AuthenticateUser(new NetworkCredential(Username, Password));
            if (isValidUser)
            {
                // Define quem é o usuário logado na aplicação
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;
            }
            else
            {
                ErrorMessage = "* Usuário ou senha inválidos";
            }
        }
    }
}
