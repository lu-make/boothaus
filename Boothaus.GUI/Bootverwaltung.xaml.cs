using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
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

    private void Bootliste_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not BootverwaltungViewmodel viewmodel) return;
        var grid = (DataGrid)sender;

        viewmodel.AusgewählteBootlisteneinträge.Clear();

        foreach (var item in grid.SelectedItems)
            viewmodel.AusgewählteBootlisteneinträge.Add((Boot)item);


        (viewmodel.BooteLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (viewmodel.BootBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    private void Bootliste_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;

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
