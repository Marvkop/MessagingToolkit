using MessagingToolkit.Messengers;

namespace MessagingToolkit.Tests.Messengers;

[TestFixture]
[TestOf(typeof(StrongReferenceMessenger))]
public class StrongReferenceMessengerThreadSafeTest : MessengerTest<StrongReferenceMessenger>
{
    protected override StrongReferenceMessenger SetUpFixture() => new(true);
}