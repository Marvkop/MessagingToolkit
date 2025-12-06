using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MessagingToolkit.Messengers;

namespace MessagingToolkit.Standalone;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly StrongReferenceMessenger _messenger = new();

    public MainWindowViewModel()
    {
        _messenger.Register<TestMessage>(this, async _ => await DoOtherStuff());
    }

    [RelayCommand]
    private async Task DoStuff()
    {
        Console.WriteLine(nameof(DoStuff));

        await _messenger.PublishAsync(new TestMessage());

        Console.WriteLine(nameof(DoStuff) + " after");
    }

    private async Task DoOtherStuff()
    {
        Console.WriteLine(nameof(DoOtherStuff) + " before");

        await Task.Delay(1000);

        Console.WriteLine(nameof(DoOtherStuff) + " after");
    }

    private record TestMessage;
}