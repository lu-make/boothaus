using Boothaus.GUI.ViewModels;
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
    /// Interaction logic for Einstellungen.xaml
    /// </summary>
    public partial class Einstellungen : ThemedWindow
    {
        public Einstellungen(EinstellungenViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
            viewmodel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewmodel.Ergebnis) && viewmodel.Ergebnis.HasValue)
                    DialogResult = viewmodel.Ergebnis;
            };
        }
    }
}
