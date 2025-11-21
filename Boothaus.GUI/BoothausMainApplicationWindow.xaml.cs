using Boothaus.GUI.ViewModels;
using DevExpress.Xpf.Core;
using System.Windows;

namespace Boothaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BoothausMainApplicationWindow : ThemedWindow
{
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
        if (data?.Records[0] is not AuftragListViewModel auftragVm) return; 

        var vorherigerPlatz = auftragVm.Modell.Platz;

        if (mainViewModel.KannZuweisen(auftragVm.Modell, platzVm.Modell))
        {
            if (vorherigerPlatz is not null)
            {
                vorherigerPlatz.ZuweisungEntfernen(auftragVm.Modell);
                var vorherigerPlatzVm = mainViewModel.LagerViewModel.AllePlätze
                    .FirstOrDefault(p => p.Modell.Id == vorherigerPlatz.Id);
                vorherigerPlatzVm?.Aktualisieren();
            }

            auftragVm.Modell.Platz = platzVm.Modell;
            platzVm.AuftragZuweisen(auftragVm.Modell);
            platzVm.Aktualisieren();

        }

    }

    private void CompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
    {
        e.Handled = true;

        if (DataContext is not MainViewModel mainViewModel) return;
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
        if (DataContext is not MainViewModel mainViewModel) return;

        var allePlätze = mainViewModel.LagerViewModel.AllePlätze;

        foreach (var platz in allePlätze)
        {
            platz.IstDragDropAktiv = true;
        }

        var gültigePlätze = mainViewModel.FindeGültigePlätze(auftragListVm.Modell).ToList();

        foreach(var platz in gültigePlätze)
        {
            platz.IstGültigesDropZiel = true;
        }
    }
}
