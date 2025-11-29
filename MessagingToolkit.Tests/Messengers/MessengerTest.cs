using MessagingToolkit.Messengers;

namespace MessagingToolkit.Tests.Messengers;

[TestFixture]
[TestOf(typeof(IMessenger))]
public abstract class MessengerTest<T>
    where T : IMessenger, new()
{
    protected T Fixture { get; private set; }

    [SetUp]
    public void SetUp()
    {
        Fixture = new T();
    }

    [Test]
    public void ChainedPublish()
    {
        var counter = 0;

        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Register<OtherTestMessage>(this, _ => Fixture.Publish(new TestMessage()));

        Fixture.Publish(new OtherTestMessage());

        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public async Task ChainedAsyncPublish()
    {
        var holder = new Holder();

        Fixture.Register<TestMessage>(this, _ => Increase(holder));
        Fixture.Register<OtherTestMessage>(this, async _ => await Fixture.PublishAsync(new TestMessage()));

        await Fixture.PublishAsync(new OtherTestMessage());

        Assert.That(holder.Counter, Is.EqualTo(1));
    }

    [Test]
    public void TestUnregister()
    {
        var counter = 0;

        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Unregister<TestMessage>(this);

        Fixture.Publish(new TestMessage());

        Assert.That(counter, Is.EqualTo(0));
    }

    [Test]
    public void TestPublish()
    {
        var counter = 0;

        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));

        Fixture.Publish(new TestMessage());

        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public async Task TestPublishAsync()
    {
        var holder = new Holder();

        Fixture.Register<TestMessage>(this, _ => Increase(holder));

        await Fixture.PublishAsync(new TestMessage());

        Assert.That(holder.Counter, Is.EqualTo(1));
    }

    [Test]
    public void TestPublishMultiple()
    {
        var counter = 0;
        var holder = new Holder();

        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Register<TestMessage>(this, _ => Increase(holder));
        Fixture.Register<TestMessage>(this, _ => Increase(holder));

        Fixture.Publish(new TestMessage());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(2));
            Assert.That(holder.Counter, Is.EqualTo(2));
        }
    }

    [Test]
    public async Task TestPublishAsyncMultiple()
    {
        var counter = 0;
        var holder = new Holder();

        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));
        Fixture.Register<TestMessage>(this, _ => Increase(holder));
        Fixture.Register<TestMessage>(this, _ => Increase(holder));

        await Fixture.PublishAsync(new TestMessage());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(2));
            Assert.That(holder.Counter, Is.EqualTo(2));
        }
    }

    private async Task Increase(Holder counter)
    {
        counter.Counter += 1;

        await Task.Delay(1);
    }

    private class Holder
    {
        public int Counter { get; set; }
    }

    private record TestMessage;

    private record OtherTestMessage;
}