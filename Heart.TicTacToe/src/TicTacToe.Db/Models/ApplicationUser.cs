using System;
using Microsoft.AspNetCore.Identity;

namespace TicTacToe.Db.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Alias { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
