using MessagingToolkit.Messengers;

namespace MessagingToolkit.Tests.Messengers;

[TestFixture]
[TestOf(typeof(StrongReferenceMessenger))]
public class StrongReferenceMessengerTest : MessengerTest<StrongReferenceMessenger>
{
    protected override StrongReferenceMessenger SetUpFixture() => new(false);
}