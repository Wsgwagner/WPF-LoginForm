using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WPF_LoginForm.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;
        public RepositoryBase()
        {
            _connectionString = @"Server=TQR216780\SQLEXPRESS;Database=WpfLogin;User Id=tds2;Password=tds123;TrustServerCertificate=True;Encrypt=False;";
        }
        public void TestConnection()
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    // Se chegar aqui, conectou!
                }
                catch (Exception ex)
                {
                    // Aqui você verá o erro real (ex: senha errada, banco não encontrado)
                    Console.WriteLine(ex.Message);
                }
            }
        }


        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
