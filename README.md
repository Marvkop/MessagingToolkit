# MessagingToolkit

#### This toolkit provides basic functionality for messaging designed for an mvvm-like infrastructure.

## Usage

There are two base-implementations that differ only in their reference-handling.

> [!WARNING]
> The WeakReferenceMessenger implementation has a cleanup method that must be called at appropriate times! 

```csharp
private readonly IMessenger _messenger = new [Weak|Strong]ReferenceMessenger();
```

Registration of actions/handlers.

```csharp
_messenger.Register<TestMessage>(this, _ => DoStuff());
_messenger.Register<TestMessage>(this, async _ => await DoAsyncStuff());
_messenger.Register<TestMessage>(this, new CustomHandler());
```