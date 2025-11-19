using Boothaus.Domain;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using Domain.Services;
using System.Windows;
using System.Windows.Media;

namespace Boothaus.GUI
{
    /// <summary>
    /// Interaction logic for AuftragMaske.xaml
    /// </summary>
    public partial class AuftragMaske : ThemedWindow
    {
        private Lager lager;
        private LagerApplicationService service;
        public Boot? Boot { get; set; }
        public DateOnly? Von { get; set; }
        public DateOnly? Bis { get; set; }
        public Lagerauftrag? Auftrag { get; set; }


        public AuftragMaske(Lager lager, IEnumerable<Boot> boote, LagerApplicationService service) : this(lager, boote, service, null)
        {
        }

        public AuftragMaske(Lager lager, IEnumerable<Boot> boote, LagerApplicationService service, Lagerauftrag? auftrag)
        {
            InitializeComponent();

            Bootliste.ItemsSource = boote;
            this.service = service;

            if (auftrag is not null)
            {
                Auftrag = auftrag;
                Bootliste.SelectedItem = auftrag.Boot;
                Zeitraumauswahl.Start = auftrag.Von.ToDateTime(new TimeOnly(0, 0));
                Zeitraumauswahl.End = auftrag.Bis.ToDateTime(new TimeOnly(0, 0));
            }

            this.lager = lager;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ValidationTexteLeeren();

            PropertiesSetzen();

            var valid = SachenValidieren();
            if (!valid) return;

            Auftrag = new Lagerauftrag(
                id: Auftrag?.Id ?? Guid.NewGuid(),
                lager: lager,
                boot: Boot!,
                von: Von!.Value,
                bis: Bis!.Value
                );

            DialogResult = true;
        }

        private void PropertiesSetzen()
        {
            Boot = Bootliste.SelectedItem as Boot;
            Von = Zeitraumauswahl.Start?.ToDateOnly();
            Bis = Zeitraumauswahl.End?.ToDateOnly();
        }

        private void ValidationTexteLeeren()
        {
            Bootliste.BorderBrush = default;
            Zeitraumauswahl.BorderBrush = default;
            BootauswahlValidationText.Visibility = Visibility.Hidden;
            ZeitraumValidationText.Visibility = Visibility.Hidden;
        }

        private bool SachenValidieren()
        {
            var bootValid = BootValidieren();
            var datumValid = DatenValidieren();
            var konfliktfrei = ValidiereAuftragKonfliktfrei();
            return datumValid && bootValid && konfliktfrei;

        }

        private bool BootValidieren()
        {
            if (Boot is null)
            {
                BootauswahlValidationText.Visibility = Visibility.Visible;
                BootauswahlValidationText.Content = "Pflichtfeld";
                Bootliste.BorderBrush = new SolidColorBrush(color: Colors.Red);
                return false;
            }

            if (!lager.Passt(Boot))
            {
                BootauswahlValidationText.Visibility = Visibility.Visible;
                BootauswahlValidationText.Content = "Das Boot passt nicht ins Lager";
                Bootliste.BorderBrush = new SolidColorBrush(color: Colors.Red);
                return false;
            }

            return true;
        }

        private bool DatenValidieren()
        {
            if (!Von.HasValue || !Bis.HasValue || !Lagerauftrag.IstGültigesDatumspaar(Von!.Value, Bis!.Value))
            {
                var message = "Sie müssen einen gültigen Zeitraum auswählen.";
                ZeitraumValidationText.Visibility = Visibility.Visible;
                ZeitraumValidationText.Content = message;
                Zeitraumauswahl.BorderBrush = new SolidColorBrush(color: Colors.Red);
                return false;
            }

            return true;
        }

        private bool ValidiereAuftragKonfliktfrei()
        {
            if (service.BootAuftragExistiertBereits(Boot!, Von!.Value, Bis!.Value))
            {
                var message = "Das Boot hat bereits einen Lagerauftrag in dem angegebenen Zeitraum.";
                ZeitraumValidationText.Visibility = Visibility.Visible;
                ZeitraumValidationText.Content = message;
                Zeitraumauswahl.BorderBrush = new SolidColorBrush(color: Colors.Red);
                return false;
            }

            return true;
        }

        private void Bootliste_SubstituteDisplayFilter(object sender, DevExpress.Xpf.Editors.Helpers.SubstituteDisplayFilterEventArgs e)
        {
            if (string.IsNullOrEmpty(e.DisplayText)) return;
            var bootnameFilter = CriteriaOperator.Parse("Contains(Name,?)", e.DisplayText);
            var kontaktFilter = CriteriaOperator.Parse("Contains(Kontakt,?)", e.DisplayText);
            e.DisplayFilter = new GroupOperator(GroupOperatorType.Or, bootnameFilter, kontaktFilter);
            e.Handled = true;

        }

    }
}
