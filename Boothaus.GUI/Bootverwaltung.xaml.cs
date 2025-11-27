using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for Bootverwaltung.xaml
/// </summary>
public partial class Bootverwaltung : Window
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

    private void Bootliste_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;

        // Adjust this type name to your actual VM class
        if (DataContext is BootverwaltungViewmodel vm)
        {
            if (vm.BooteLöschenCommand?.CanExecute(null) == true)
            {
                vm.BooteLöschenCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}
