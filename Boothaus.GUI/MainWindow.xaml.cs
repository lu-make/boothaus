using Boothaus.Domain;
using Boothaus.GUI;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using DevExpress.Xpf.Core;
using Domain.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Boothaus
{
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

            ListenFüllen();
             
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

        private void ListenFüllen()
        { 
            aufträge = new(service.AlleAufträge());
            Auftragliste.ItemsSource = aufträge;
        }

        private void AuftragErfassenButton_Click(object sender, RoutedEventArgs e)
        {
            var maske = new AuftragMaske(lager: service.GetLager(), boote: service.AlleBoote());
            var ergebnis = maske.ShowDialog();
            if (ergebnis == true)
            {
                service.ErfasseAuftrag(maske.Auftrag!);
            }
        }

        private void AuftragBearbeitenButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AuftragLöschenButton_Click(object sender, RoutedEventArgs e)
        {

        }
         
    }
}
