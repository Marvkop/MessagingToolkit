# MessagingToolkit

#### This toolkit provides basic functionality for messaging designed for an mvvm-like infrastructure.

## Usage

There are two base-implementations that differ only in their reference-handling.

> [!WARNING]
> The WeakReferenceMessenger implementation has a cleanup method that must be called manually at appropriate times!

```csharp
var messenger = new [Weak|Strong]ReferenceMessenger();
```

Registration of actions/handlers.

```csharp
messenger.Register<TestMessage>(this, _ => DoStuff());
messenger.Register<TestMessage>(this, async _ => await DoAsyncStuff());
messenger.Register<TestMessage>(this, new CustomHandler());
```