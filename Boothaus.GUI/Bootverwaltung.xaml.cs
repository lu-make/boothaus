using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Core;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for Bootverwaltung.xaml
/// </summary>
public partial class Bootverwaltung : ThemedWindow
{
    public Bootverwaltung(BootverwaltungViewmodel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel;
    }

    private void Bootliste_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
    {
        var viewModel = DataContext as BootverwaltungViewmodel;
        (viewModel.BooteLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (viewModel.BootBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }
}
