using MessagingToolkit.Messengers;

namespace MessagingToolkit.Tests.Messengers;

[TestFixture]
[TestOf(typeof(WeakReferenceMessenger))]
public class WeakReferenceMessengerTest
{
    private WeakReferenceMessenger _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new WeakReferenceMessenger();
    }

    [Test]
    public void TestUnregister()
    {
        var counter = 0;

        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Unregister<TestMessage>(this);

        _sut.Publish(new TestMessage());

        Assert.That(counter, Is.EqualTo(0));
    }

    [Test]
    public void TestPublish()
    {
        var counter = 0;

        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Publish(new TestMessage());

        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public async Task TestPublishAsync()
    {
        var holder = new Holder();

        _sut.Register<TestMessage>(this, _ => Increase(holder));

        await _sut.PublishAsync(new TestMessage());

        Assert.That(holder.Counter, Is.EqualTo(1));
    }

    [Test]
    public void TestPublishMultiple()
    {
        var counter = 0;
        var holder = new Holder();

        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Register<TestMessage>(this, _ => Increase(holder));
        _sut.Register<TestMessage>(this, _ => Increase(holder));

        _sut.Publish(new TestMessage());

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

        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Register<TestMessage>(this, _ => Increase(ref counter));
        _sut.Register<TestMessage>(this, _ => Increase(holder));
        _sut.Register<TestMessage>(this, _ => Increase(holder));

        await _sut.PublishAsync(new TestMessage());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(2));
            Assert.That(holder.Counter, Is.EqualTo(2));
        }
    }

    private void Increase(ref int counter)
    {
        Interlocked.Increment(ref counter);
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
}