using System;
using System.Collections.Generic;

namespace TodoApi;

public partial class Item1
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? IsComplete { get; set; }

    public int? UserId { get; set; }
}
