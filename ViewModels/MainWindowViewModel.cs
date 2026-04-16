namespace SmirnovCommander.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public PanelViewModel LeftPanel { get; } = new();
    public PanelViewModel RightPanel { get; } = new();
}