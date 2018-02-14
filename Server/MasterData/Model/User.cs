using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using Server.Database.DataProviders.Util;

namespace Server.MasterData.Model
{

    public class User : IDatabaseRow
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int UserId { get; set; }
        public string Password { get; }
        public string Email { get; }
        public string Username { get; }

        public int PrimaryId => UserId;

        public User()
        {
        }

        public User(string firstName, string lastName, int userId, string email, string password, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            UserId = userId;
            Email = email;
            Password = password;
            Username = username;
        }

        public User(string firstName, string lastName, string email, string password, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
            Username = username;
        }

        public static Func<IDataReader, User> ToDomainConverter
        {
            get
            {
                return reader => new User(userId: (int) reader[nameof(UserId)],
                    firstName: (string) reader[nameof(FirstName)],
                    lastName: (string) reader[nameof(LastName)],
                    email: (string) reader[nameof(Email)],
                    username: (string) reader[nameof(Username)],
                    password: (string) reader[nameof(Password)]);
            }
        }
    }
}