using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace User.Producer.Messaging;

public sealed record class UserCreatedMessage
{
    [Required]
    public int Id { get; init; } = default;

    [Required, MinLength(2)]
    public string Name { get; init; } = default;

    [Required, EmailAddress]
    public string Email { get; init; } = default;

    [Required]
    public DateTime CreatedAt { get; init; } = default;

}
