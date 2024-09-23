using System;
using System.Collections.Generic;

namespace API.Data.Model;

public partial class AccountSession
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public bool Active { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastAuthenticated { get; set; }

    public Guid Token { get; set; }

    public virtual Account Account { get; set; } = null!;
}
