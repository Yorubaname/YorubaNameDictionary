﻿using MediatR;

namespace Core.Events
{
    public record NameIndexed(string Name, string Meaning) : INotification
    {
    }
}
