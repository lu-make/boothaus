using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
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
            var dragdropResult = DragDrop.DoDragDrop(rect, auftrag, DragDropEffects.Move);
            if (dragdropResult == DragDropEffects.None)
            { 
                var vorherigerPlatz = auftrag.Platz;

                if (vorherigerPlatz is not null)
                {
                    var vorherigerPlatzVm = mainViewModel.LagerViewModel.AllePlätze
                        .First(p => p.Modell == vorherigerPlatz);
                    vorherigerPlatz.ZuweisungEntfernen(auftrag);
                    vorherigerPlatzVm.Aktualisieren();
                } 
            }


        }
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

    private void Auftragliste_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
    {
        (mainViewModel.AuftragBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (mainViewModel.AufträgeLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();

        var auserwählte = mainViewModel.AusgewählteAuftragListeneinträge.Select(a => a.Modell);

        foreach (var platz in mainViewModel.LagerViewModel.AllePlätze)
        {
            if (auserwählte.Contains(platz.Modell.GetNächsteZuweisung(mainViewModel.AusgewählteSaison)))
            {
                platz.IstHervorgehoben = true;
            }
            else
            {
                platz.IstHervorgehoben = false;
            }
        }
         
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

        DragDrop.DoDragDrop(Auftragliste, selected, DragDropEffects.Move);
    }
}
