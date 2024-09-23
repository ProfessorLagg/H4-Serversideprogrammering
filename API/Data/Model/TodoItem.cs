using System;
using System.Collections.Generic;

namespace API.Data.Model;

public partial class TodoItem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int UserId { get; set; }

    public virtual Account User { get; set; } = null!;
}
