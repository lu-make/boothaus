using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;

namespace Boothaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BoothausMainApplicationWindow : ThemedWindow
{
    private MainViewModel mainViewModel => DataContext as MainViewModel;

    public BoothausMainApplicationWindow()
    {
        InitializeComponent();
    }

    private void LagerplatzRect_Drop(object sender, System.Windows.DragEventArgs e)
    {
        var rect = sender as FrameworkElement;
        if (rect?.DataContext is not LagerplatzViewModel platzVm) return;
        if (DataContext is not MainViewModel mainViewModel) return;
        if (!e.Data.GetDataPresent(typeof(RecordDragDropData))) return;
        
        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
        var myRecord = data?.Records[0];
        Auftrag? auftrag;

        if (myRecord is AuftragListViewModel auftragVm)
        {
            auftrag = auftragVm.Modell;

        }
        else if (myRecord is Auftrag)
        {
            auftrag = myRecord as Auftrag;
        }
        else return;

        if (auftrag is null) return;
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

        }

        DragDropFertig();
    }

    private void CompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
    {
        e.Handled = true;
        DragDropFertig();
    }

    private void DragDropFertig()
    {
        var allePlätze = mainViewModel.LagerViewModel.AllePlätze;

        foreach (var platz in allePlätze)
        {
            platz.IstDragDropAktiv = false;
            platz.IstGültigesDropZiel = false;
        }
    }

    private void StartRecordDrag(object sender, StartRecordDragEventArgs e)
    {
        if (e.Records.FirstOrDefault() is not AuftragListViewModel auftragListVm) return; 
        GültigePlätzeHervorheben(auftragListVm.Modell);
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
            DragDrop.DoDragDrop(rect, new RecordDragDropData( [ auftrag ]), System.Windows.DragDropEffects.Move);
        } 
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
            platz.IstGültigesDropZiel = true;
        }

    }

    private void Auftragliste_SelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
    {
        (mainViewModel.AuftragBearbeitenCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (mainViewModel.AufträgeLöschenCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }
}
