# MessagingToolkit

#### This toolkit provides basic functionality for messaging designed for an mvvm-like infrastructure.

## Usage

There are two base-implementations that differ only in their reference-handling.

> [!WARNING]
> The WeakReferenceMessenger implementation has a cleanup method that must be called manually at appropriate times!

```csharp
var messenger = new [Weak|Strong]ReferenceMessenger([true|false]);
```

### Registration

```csharp
messenger.Register<TestMessage>(this, _ => DoStuff());
messenger.Register<TestMessage>(this, async _ => await DoAsyncStuff());
messenger.Register<TestMessage>(this, new CustomHandler());
```

### Publishing

```csharp
// if called while asynchronous handlers are registered this call will wait and block the current thread
// until the asynchronous actions are done
messenger.Publish(new TestMessage());

// if using asynchronous handlers try to only use
await messenger.PublishAsync(new TestMessage());
```

You can also implement a custom handler that reacts differently when called sync/async.