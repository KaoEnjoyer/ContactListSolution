using ContactList.Shared.Models;
using ContactList.Users.Requests;
using Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using MongoDB.Driver;
namespace ContactList.Users
{
    public class UsersDatabase : IUsersDatabase
    {
        private readonly IConfiguration configuration;

        private const string Salt = "neNaXSodvl";

        public UsersDatabase(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        private IMongoCollection<T> ConnectToMongo<T>(in string collection)//co to in
        {
            var client = new MongoClient(configuration.GetConnectionString("ContactList"));
            var database = client.GetDatabase("ContactList");
            return database.GetCollection<T>(collection);
        }
        public User Login(LoginRequest loginRequest)
        {
            var collection = ConnectToMongo<User>("Users");
            var filter = Builders<User>.Filter.Eq(u => u.Login, loginRequest.Login);
            var user = collection.Find(u => u.Login == loginRequest.Login).ToList().FirstOrDefault();

            //var results = collection.Find(_ => true);
            //foreach (var result in results.ToList())
            //{
            //    Console.WriteLine($"{result.Id} imie: {result.Login} pass: {result.Password}");
            //}

            if (user == null)
            {
                return null;
            }



            switch (user.PasswordFormat)
            {
                case "plainText":
                    {
                        if (user.Password != loginRequest.Password)
                        {
                            return null;
                        }
                        break;
                    }
                case "sha512":
                    {
                        string hash = CreateHash(loginRequest.Password);
                Console.WriteLine($"{user.Id} imie: {user.Login} pass: {user.Password} , hash: {hash}");

                        if (user.Password != hash)
                        {
                            return null;
                        }
                        break;
                    }
            }




            return user;
            //string connectionString = configuration.GetConnectionString("ContactList");
            //using (SqlConnection connection = new(connectionString))
            //{
            //    connection.Open();

            //    using (SqlCommand command = connection.CreateCommand())
            //    {
            //        command.CommandType = System.Data.CommandType.Text;
            //        command.CommandText = "select * from Users where upper(Login) = @login";

            //        command.Parameters.AddWithValue("login", loginRequest.Login.ToUpper());
            //        command.Parameters.AddWithValue("password", loginRequest.Password);

            //        SqlDataReader reader = command.ExecuteReader();
            //        reader.Read();
            //        if (reader.HasRows)
            //        {
            //            User user = GetUser(reader);
            //            switch (user.PasswordFormat)
            //            {
            //                case "plainText":
            //                    {
            //                        if (user.Password != loginRequest.Password)
            //                        {
            //                            return null;
            //                        }
            //                        break;
            //                    }
            //                case "sha512":
            //                    {
            //                        string hash = CreateHash(loginRequest.Password);
            //                        if (user.Password != hash)
            //                        {
            //                            return null;
            //                        }
            //                        break;
            //                    }
            //            }
            //            return user;
            //        }
            //    }
            //    connection.Close();
            //}

            //return null;
        }

        public User Add(RegisterRequest registerRequest)
        {

            var collection = ConnectToMongo<User>("Users");

            string connectionString = configuration.GetConnectionString("Library");

            User user = new User()
            {
                Password = CreateHash(registerRequest.Password),
                PasswordFormat = "sha512",
                Login = registerRequest.Login,
                Email = registerRequest.Email,
            };


             //user.Password = CreateHash(user.Password);
            collection.InsertOne(user);


            var results = collection.Find(_ => true );
            foreach (var result in results.ToList())
            {
                Console.WriteLine($"{result.Id} imie: {result.Login}");
            }

                CheckUser(user);
            return user;
            //user.Password = CreateHash(user.Password);

            //using (SqlConnection connection = new(connectionString))
            //{
            //    connection.Open();

            //    using (SqlCommand command = connection.CreateCommand())
            //    {
            //        command.CommandType = System.Data.CommandType.Text;
            //        command.CommandText = "INSERT INTO [dbo].[Users] ([Password], [Login], [Email], [PasswordFormat])" +
            //                              "OUTPUT inserted.Id " +
            //                              "VALUES" +
            //                              "(@password, @login, @email, @passwordFormat)";

            //        command.Parameters.AddWithValue("password", user.Password);
            //        command.Parameters.AddWithValue("login", user.Login);
            //        command.Parameters.AddWithValue("email", user.Email);
            //        command.Parameters.AddWithValue("passwordFormat", "sha512");

            //        object id = command.ExecuteScalar();

            //        user.Id = Convert.ToInt32(id);
            //    }

            //    connection.Close();
            //}

            //return user;
        }

        public User Edit(int id, User user)
        {
            string connectionString = configuration.GetConnectionString("Library");

            CheckEditUser(user);

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "update Users set " +
                                          "Login = @login, Email = @email " +
                                          "where id = @id";

                    command.Parameters.AddWithValue("login", user.Login);
                    command.Parameters.AddWithValue("email", user.Email);
                    command.Parameters.AddWithValue("id", id);

                    using (TransactionScope transaction = new TransactionScope())
                    {
                        int recourdCount = command.ExecuteNonQuery();
                        if (recourdCount == 0)
                        {
                            throw new NotFoundException("UserNotFound");
                        }
                        if (recourdCount != 1)
                        {
                            throw new Exception("TooMuchUsersChanged");
                        }

                        transaction.Complete();
                    }
                }
                connection.Close();
            }

            return user;
        }

        public void Delete(int id)
        {
            string connectionString = configuration.GetConnectionString("Library");
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "delete from Users where id = @id";

                    command.Parameters.AddWithValue("id", id);

                    using (TransactionScope transaction = new TransactionScope())
                    {
                        int recourdCount = command.ExecuteNonQuery();
                        if (recourdCount == 0)
                        {
                            throw new NotFoundException("UserNotFound");
                        }
                        if (recourdCount != 1)
                        {
                            throw new InvalidOperationException("TooMuchUsersDeleted");
                        }

                        transaction.Complete();
                    }
                }

                connection.Close();
            }
        }

        public List<User> SelectAll(SearchUsersRequest searchUsersRequest)
        {
            string connectionString = configuration.GetConnectionString("Library");
            List<User> result = new();
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select * from Users where " +
                                          "(Email like @email or @email is null) and " +
                                          "(Login like @login or @login is null) and " +
                                          "(id = @id or @id = 0) ";
                    command.CommandText += searchUsersRequest.Sort.ToLower() switch
                    {
                        "email" => "order by email",
                        "login" => "order by login",
                        _ => "order by id"
                    };

                    command.CommandText += " OFFSET @from ROWS FETCH NEXT @take ROWS ONLY";

                    command.Parameters.AddWithValue("email", searchUsersRequest.Email switch
                    {
                        "" => DBNull.Value,
                        _ => $"%{searchUsersRequest.Email}%"
                    });
                    command.Parameters.AddWithValue("login", searchUsersRequest.Login switch
                    {
                        "" => DBNull.Value,
                        _ => $"%{searchUsersRequest.Login}%"
                    });
                    command.Parameters.AddWithValue("id", searchUsersRequest.Id);
                    command.Parameters.AddWithValue("sort", searchUsersRequest.Sort);
                    command.Parameters.AddWithValue("from", searchUsersRequest.From);
                    command.Parameters.AddWithValue("take", searchUsersRequest.Take);

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        return null;
                        //result.Add(GetUser(reader));
                    }
                }

                connection.Close();
            }

            return result;
        }

        public User Select(string id)
        {
            var connection = ConnectToMongo<User>("Users");


            return connection.Find(u => u.Id == id).FirstOrDefault();
            //using (SqlConnection connection = new(connectionString))
            //{
            //    connection.Open();

            //    using (SqlCommand command = connection.CreateCommand())
            //    {
            //        command.CommandType = System.Data.CommandType.Text;
            //        command.CommandText = "select * from Users where id = @id";

            //        command.Parameters.AddWithValue("id", id);

            //        SqlDataReader reader = command.ExecuteReader();
            //        reader.Read();
            //        if (reader.HasRows)
            //        {
            //            return null;
            //           /// return GetUser(reader);
            //        }
            //    }
            //    connection.Close();
            //}

            //return null;
        }

        public int SelectCount(SearchUsersRequest searchUsersRequest)
        {
            string connectionString = configuration.GetConnectionString("Library");
            int result;
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "select count(*) AS cnt from Users where" +
                                          "(Login like @login or @login is null) and" +
                                          "(Email like @email or @email is null) and" +
                                          "(Id like @id or @id = 0)";

                    command.Parameters.AddWithValue("login", searchUsersRequest.Login switch
                    {
                        null => DBNull.Value,
                        _ => $"%{searchUsersRequest.Login}%"
                    });
                    command.Parameters.AddWithValue("email", searchUsersRequest.Email switch
                    {
                        null => DBNull.Value,
                        _ => $"%{searchUsersRequest.Email}%"
                    });
                    command.Parameters.AddWithValue("id", searchUsersRequest.Id);

                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    result = reader.GetFieldValue<int>(0);
                }

                connection.Close();
            }

            return result;
        }

        //private User GetUser(SqlDataReader reader)
        //{
        //    //User user = new User
        //    //{
        //    //    Id = (int)reader["id"],
        //    //    Password = ReadString(reader, "password"),
        //    //    Login = ReadString(reader, "login"),
        //    //    Email = ReadString(reader, "email"),
        //    //    PasswordFormat = ReadString(reader, "passwordFormat")
        //    //};
        //    return user;
        //}

        private string ReadString(SqlDataReader reader, string fieldName)
        {
            object obj = reader[fieldName];
            if (obj == DBNull.Value)
            {
                return null;
            }

            return (string)obj;
        }

        private void CheckEditUser(User user)
        {
            if (user.Login == "")
            {
                throw new ValidationException("EmptyLogin");
            }
            if (user.Login.Length >= 100)
            {
                throw new ValidationException("TooLongLogin");
            }

            if (user.Email == "")
            {
                throw new ValidationException("EmptyEmail");
            }
            if (user.Email.Length >= 100)
            {
                throw new ValidationException("TooLongEmail");
            }
        }

        private void CheckUser(User user)
        {
            if (user.Password == "")
            {
                throw new ValidationException("EmptyPassword");
            }
            if (user.Password.Length <= 5)
            {
                throw new ValidationException("TooShortPassword");
            }
            if (user.Password.Length >= 100)
            {
                throw new ValidationException("TooLongPassword");
            }

            if (user.Login == "")
            {
                throw new ValidationException("EmptyLogin");
            }
            if (user.Login.Length >= 100)
            {
                throw new ValidationException("TooLongLogin");
            }

            Regex regex = new(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            if (!regex.IsMatch(user.Email))
            {
                throw new ValidationException("IncorrectEmail");
            }
            if (user.Email == "")
            {
                throw new ValidationException("EmptyEmail");
            }
            if (user.Email.Length >= 100)
            {
                throw new ValidationException("TooLongEmail");
            }
        }

        private string CreateHash(string password)
        {
            string input = password + Salt;
            byte[] hash = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
            string result = Convert.ToBase64String(hash);
            return result;
        }
    }

}
