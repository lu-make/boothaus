using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Boothaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BoothausMainApplicationWindow : Window
{
    private MainViewModel mainViewModel => DataContext as MainViewModel;

    public BoothausMainApplicationWindow()
    {
        InitializeComponent();
        this.SizeToContent = SizeToContent.WidthAndHeight;
    }

    private void LagerplatzRect_Drop(object sender, DragEventArgs e)
    {
        var rect = sender as FrameworkElement;
        if (rect?.DataContext is not LagerplatzViewModel platzVm) return;
        if (DataContext is not MainViewModel mainViewModel) return;

        Auftrag? auftrag = null;

        // Case 1: dragged from DataGrid (Auftragliste)
        if (e.Data.GetDataPresent(typeof(AuftragListViewModel)))
        {
            var vm = (AuftragListViewModel)e.Data.GetData(typeof(AuftragListViewModel));
            auftrag = vm.Modell;
        }
        // Case 2: dragged from LagerplatzRect itself
        else if (e.Data.GetDataPresent(typeof(Auftrag)))
        {
            auftrag = (Auftrag)e.Data.GetData(typeof(Auftrag));
        }

        if (auftrag is null)
            return;

        var vorherigerPlatz = auftrag.Platz;

        if (mainViewModel.KannZuweisen(auftrag, platzVm.Modell))
        {
            if (vorherigerPlatz is not null)
            {
                vorherigerPlatz.ZuweisungEntfernen(auftrag);
                var vorherigerPlatzVm = mainViewModel.LagerViewModel.AllePlätze
                    .FirstOrDefault(p => p.Modell.Id == vorherigerPlatz.Id);
                vorherigerPlatzVm?.Aktualisieren();
            }

            auftrag.Platz = platzVm.Modell;
            platzVm.AuftragZuweisen(auftrag);
            platzVm.Aktualisieren();

            e.Effects = DragDropEffects.Move;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
        DragDropFertig();
    }

    private void DragDropFertig()
    {
        mainViewModel.RefreshAuftragliste();
        var allePlätze = mainViewModel.LagerViewModel.AllePlätze;

        foreach (var platz in allePlätze)
        {
            platz.IstDragDropAktiv = false;
            platz.IstHervorgehoben = false;
        }
    }

    private void LagerplatzRect_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var rect = sender as FrameworkElement;
        if (rect?.DataContext is not LagerplatzViewModel platzVm) return;


        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var auftrag = platzVm.NächsteZuweisung;
            if (auftrag is null) return;
             
            var reihe = mainViewModel.LagerViewModel.ReihenViewmodels.First(r => r.PlatzViewmodels.Contains(platzVm));

            var zuweisungenHinterDiesemPlatz = reihe.Modell.PlätzeHinter(platzVm.Modell)
                .Where(platz => platz.GetNächsteZuweisung(auftrag.Saison) != null)
                .ToList();

            if (zuweisungenHinterDiesemPlatz.Any())
            {
                // es sind Plätze vor diesem Platz belegt, daher darf nicht gezogen werden
                return;
            }

            GültigePlätzeHervorheben(auftrag);
            SetPapierkorbSichtbar(true);
            DragDrop.DoDragDrop(rect, auftrag, DragDropEffects.Move);
            SetPapierkorbSichtbar(false);


        }
        e.Handled = true;
        DragDropFertig();
    }

    private void GültigePlätzeHervorheben(Auftrag auftrag)
    {
        var allePlätze = mainViewModel.LagerViewModel.AllePlätze;

        foreach (var platz in allePlätze)
        {
            platz.IstDragDropAktiv = true;
        }

        var gültigePlätze = mainViewModel.FindeGültigePlätze(auftrag).ToList();

        foreach (var platz in gültigePlätze)
        {
            platz.IstHervorgehoben = true;
        }

    }

    private void Auftragliste_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var grid = (DataGrid)sender;
        mainViewModel.AusgewählteAuftragListeneinträge.Clear();

        foreach (var item in grid.SelectedItems)
            mainViewModel.AusgewählteAuftragListeneinträge.Add((AuftragListViewModel)item);

        var auserwählte = mainViewModel.AusgewählteAuftragListeneinträge.Select(a => a.Modell);

        foreach (var platz in mainViewModel.LagerViewModel.AllePlätze)
            if (auserwählte.Contains(platz.Modell.GetNächsteZuweisung(mainViewModel.AusgewählteSaison))) 
                platz.IstHervorgehoben = true;
            else
                platz.IstHervorgehoben = false;
        

        (mainViewModel.AuftragBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (mainViewModel.AufträgeLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    private void Auftragliste_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
            return;

        if (DataContext is MainViewModel vm)
        {
            if (vm.AufträgeLöschenCommand?.CanExecute(null) == true)
            {
                vm.AufträgeLöschenCommand.Execute(null);
                e.Handled = true;
            }
        }
    }

    private void Auftragliste_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        if (Auftragliste.SelectedItem is not AuftragListViewModel selected)
            return;

        GültigePlätzeHervorheben(selected.Modell);
        DragDrop.DoDragDrop(Auftragliste, selected, DragDropEffects.Move);
        DragDropFertig();
    }

    private void Papierkorb_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(Auftrag)))
            e.Effects = DragDropEffects.Move;
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }

    private void Papierkorb_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel mainViewModel) return;

        if (!e.Data.GetDataPresent(typeof(Auftrag)))
        {
            e.Effects = DragDropEffects.None;
            return;
        }

        var auftrag = (Auftrag)e.Data.GetData(typeof(Auftrag));

        var platz = auftrag.Platz;
        if (platz is not null)
        {
            platz.ZuweisungEntfernen(auftrag);

            var platzVm = mainViewModel.LagerViewModel.AllePlätze
                .FirstOrDefault(p => p.Modell.Id == platz.Id);
            platzVm?.Aktualisieren();

            auftrag.Platz = null;
        }

        e.Effects = DragDropEffects.Move;
        e.Handled = true;

        DragDropFertig();
    }

    private void SetPapierkorbSichtbar(bool sichtbar)
    {
        PapierkorbArea.Visibility = sichtbar
            ? Visibility.Visible
            : Visibility.Hidden;
    }
}
