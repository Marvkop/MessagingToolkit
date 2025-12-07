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
        _messenger.Register<TestMessage>(this, _ => DoStuff());
    }

    [RelayCommand]
    private void DoStuff()
    {
        // do synchronous stuff
    }

    private async Task DoOtherStuff()
    {
        // do asynchronous stuff

        await Task.CompletedTask;
    }

    private record TestMessage;
}