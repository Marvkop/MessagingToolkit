using MessagingToolkit.Messengers;

namespace MessagingToolkit.Tests.Messengers;

[TestFixture]
[TestOf(typeof(WeakReferenceMessenger))]
public class WeakReferenceMessengerTest : MessengerTest<WeakReferenceMessenger>
{
    [Test]
    public void TestPublishIgnoresRemovedReferences()
    {
        var counter = 0;

        Register(_ => Interlocked.Increment(ref counter));
        Fixture.Register<TestMessage>(this, _ => Interlocked.Increment(ref counter));

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Fixture.Publish(new TestMessage());

        Assert.That(counter, Is.EqualTo(1));
    }

    private void Register(Action<TestMessage> action)
    {
        var recipient = new object();
        Fixture.Register(recipient, action);
    }

    private record TestMessage;

    protected override WeakReferenceMessenger SetUpFixture() => new(true);
}