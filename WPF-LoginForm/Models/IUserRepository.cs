using System.Net;
using System.Collections.Generic;

namespace WPF_LoginForm.Models
{
    public interface IUserRepository
    {

        bool AuthenticateUser(NetworkCredential credential);
        UserModel GetByUsername(string username); // Método que o MainViewModel está chamando
        // Outros métodos como Add, Edit, Remove podem vir aqui depois


    }




}