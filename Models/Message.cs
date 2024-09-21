using System;
using System.Collections.Generic;

namespace ChatApp.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public short SenderId { get; set; }

    public short ReceiverId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public bool IsRead { get; set; }

    public virtual UserList Receiver { get; set; } = null!;

    public virtual UserList Sender { get; set; } = null!;
}
