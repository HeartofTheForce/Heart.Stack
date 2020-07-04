using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TicTacToe.Api.Models
{
    public class ClaimsUser
    {
        public Guid Id { get; }

        public ClaimsUser(ClaimsPrincipal claimsPrincipal)
        {
            var nameIdentifier = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier);
            Id = Guid.Parse(nameIdentifier.Value);
        }
    }
}
