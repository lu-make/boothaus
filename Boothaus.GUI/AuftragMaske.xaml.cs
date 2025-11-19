using Boothaus.Domain;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Boothaus.GUI
{
    /// <summary>
    /// Interaction logic for AuftragMaske.xaml
    /// </summary>
    public partial class AuftragMaske : ThemedWindow
    {
        private Lager lager;
        public Boot? Boot { get; set; }
        public DateOnly? Von { get; set; }
        public DateOnly? Bis { get; set; }

        public Lagerauftrag? Auftrag { get; set; }

        public AuftragMaske(Lager lager, IEnumerable<Boot> boote) : this(lager, boote, null) { }

        public AuftragMaske(Lager lager, IEnumerable<Boot> boote, Lagerauftrag? auftrag)
        {
            InitializeComponent();

            Bootliste.ItemsSource = boote;

            if (auftrag is not null)
            {
                Auftrag = auftrag;
                Bootliste.SelectedItem = auftrag.Boot;
                VonEdit.Date = auftrag.Von;
                BisEdit.Date = auftrag.Bis;
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

        private void ValidationTexteLeeren()
        {
            BootauswahlValidationText.Visibility = Visibility.Hidden;
            VonEditValidationText.Visibility = Visibility.Hidden;
            BisEditValidationText.Visibility = Visibility.Hidden;
        }

        private bool SachenValidieren()
        {
            return BootValidieren() 
                && DatenValidieren();
        }

        private bool BootValidieren()
        {
            if (Boot is null)
            {
                BootauswahlValidationText.Visibility = Visibility.Visible;
                BootauswahlValidationText.Content = "Pflichtfeld";
                return false;
            }

            if (!lager.Passt(Boot))
            { 
                BootauswahlValidationText.Visibility = Visibility.Visible;
                BootauswahlValidationText.Content = "Das Boot passt nicht ins Lager";
                return false;
            }

            return true;
        }

        private bool DatenValidieren()
        {
            if (Von is null && Bis is null)
            {
                VonEditValidationText.Visibility = Visibility.Visible;
                VonEditValidationText.Content = "Pflichtfeld"; 
                BisEditValidationText.Visibility = Visibility.Visible;
                BisEditValidationText.Content = "Pflichtfeld"; 
                return false;
            }

            if (Von is null)
            {
                VonEditValidationText.Visibility = Visibility.Visible;
                VonEditValidationText.Content = "Pflichtfeld";
                return false;
            }
            if (Bis is null)
            {
                BisEditValidationText.Visibility = Visibility.Visible;
                BisEditValidationText.Content = "Pflichtfeld";
                return false;
            }

            if (Lagerauftrag.IstGültigesDatumspaar(Von.Value, Bis.Value) == false)
            {
                VonEditValidationText.Visibility = Visibility.Visible;
                VonEditValidationText.Content = "Muss vor Enddatum liegen.";
                BisEditValidationText.Visibility = Visibility.Visible;
                BisEditValidationText.Content = "Muss nach Startdatum liegen.";
                return false;
            }

            return true;
        }
         
    }
}
