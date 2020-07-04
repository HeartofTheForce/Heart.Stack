using System;
using TicTacToe.Db.Models;

namespace TicTacToe.Api.Models.Users
{
    public class UserModel
    {
        public string Id { get; }
        public string Email { get; }
        public string Alias { get; }
        public DateTimeOffset CreatedDate { get; }

        public UserModel(ApplicationUser applicationUser)
        {
            Id = applicationUser.Id;
            Email = applicationUser.Email;
            Alias = applicationUser.Alias;
            CreatedDate = applicationUser.CreatedDate;
        }
    }
}
