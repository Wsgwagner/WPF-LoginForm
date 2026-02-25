using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser = false;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM [Users] WHERE Username=@user AND Password=@pass";
                command.Parameters.Add("@user", SqlDbType.NVarChar).Value = credential.UserName;
                command.Parameters.Add("@pass", SqlDbType.NVarChar).Value = credential.Password;

                validUser = command.ExecuteReader().HasRows;
            }
            return validUser;
        }

        public UserModel GetByUsername(string username)
        {
            UserModel user = null;
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                // CORREÇÃO: Mudado de [Username] para [Users]
                command.CommandText = "SELECT Id, Username, Name, LastName, Email FROM [Users] WHERE Username=@username";
                command.Parameters.Add("@username", System.Data.SqlDbType.NVarChar).Value = username;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new UserModel()
                        {
                            Id = reader["Id"].ToString(),
                            Username = reader["Username"].ToString(),
                            Name = reader["Name"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                    }
                }
            }
            return user;
        }
    }
}