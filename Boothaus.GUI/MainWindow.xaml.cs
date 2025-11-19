using Boothaus.Domain;
using Boothaus.GUI;
using Boothaus.GUI.ViewModels;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using DevExpress.Xpf.Core;
using Domain.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Boothaus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class BoothausMainApplicationWindow : ThemedWindow
{
    private LagerApplicationService service;
    private ObservableCollection<Lagerauftrag> aufträge;

    public BoothausMainApplicationWindow()
    {
        InitializeComponent();

        var auftragRepo = new InMemoryAuftragRepository();
        var bootRepo = new InMemoryBootRepository();
        var lagerRepo = new InMemoryLagerRepository();

        InitializeDefaultData(
            auftragRepo: auftragRepo,
            bootRepo: bootRepo,
            lagerRepo: lagerRepo
            );

        service = new LagerApplicationService(
            auftragRepository: auftragRepo,
            bootRepository: bootRepo,
            lagerRepository: lagerRepo
            );

        DarstellungFüllen();

    }

    private void InitializeDefaultData(
        IAuftragRepository auftragRepo,
        IBootRepository bootRepo,
        ILagerRepository lagerRepo
        )
    {
        bootRepo.InitialisiereMitDefaults(DefaultData.Boote());
        lagerRepo.InitialisiereMitDefaults(DefaultData.Lager());
        auftragRepo.InitialisiereMitDefaults(DefaultData.Aufträge(lager: lagerRepo.GetLager(), boote: bootRepo.GetAll()));
    }

    private void DarstellungFüllen()
    {
        var meineAufträge = service.AlleAufträge().ToList();
        var lager = service.GetLager();
        DataContext = new LagerViewModel(lager);

        if (aufträge is null)
        {
            aufträge = new(meineAufträge);
            Auftragliste.ItemsSource = aufträge;
        }
        else
        {
            aufträge.Clear();
            foreach (var auftrag in meineAufträge)
            {
                aufträge.Add(auftrag);
            }
        }
    }

    private void AuftragErfassenButton_Click(object sender, RoutedEventArgs e)
    {
        var maske = new AuftragMaske(lager: service.GetLager(), boote: service.AlleBoote(), service);
        maske.Owner = this;
        var ergebnis = maske.ShowDialog();
        if (ergebnis == true)
        {
            service.ErfasseAuftrag(maske.Auftrag!);
            DarstellungFüllen();
        }
    }

    private void AuftragBearbeitenButton_Click(object sender, RoutedEventArgs e)
    {
        var maske = new AuftragMaske(lager: service.GetLager(), boote: service.AlleBoote(), service, auftrag: Auftragliste.SelectedItem as Lagerauftrag);
        maske.Owner = this;
        var ergebnis = maske.ShowDialog();
        if (ergebnis == true)
        {
            service.AktualisiereAuftrag(maske.Auftrag!);
            DarstellungFüllen();
        }

    }

    private void AuftragLöschenButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = System.Windows.MessageBox.Show("Möchten Sie den ausgewählten Auftrag wirklich löschen?", "Auftrag löschen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            service.LöscheAuftrag(Auftragliste.SelectedItem as Lagerauftrag);
            DarstellungFüllen();
        }
    }

    private void LagerkalenderErzeugenButton_Click(object sender, RoutedEventArgs e)
    {
        service.ErstelleLagerkalender();
        DarstellungFüllen();
    }
}
