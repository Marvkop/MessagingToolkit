using System;

namespace MessagingToolkit.CodeGeneration.Locking;

[AttributeUsage(AttributeTargets.Method)]
public class LockAttribute
{
    public LockType LockType { get; set; } = LockType.Read;
}