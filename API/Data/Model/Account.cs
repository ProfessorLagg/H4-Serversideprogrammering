using System;
using System.Collections.Generic;

namespace API.Data.Model;

public partial class Account
{
    public int Id { get; set; }

    public string Login { get; set; } = null!;

    public string? Cpr { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
