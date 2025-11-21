using Boothaus.GUI.ViewModels;
using DevExpress.Xpf.Core;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for Bootverwaltung.xaml
/// </summary>
public partial class Bootverwaltung : ThemedWindow
{
    public Bootverwaltung(BootverwaltungViewmodel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel;
    }
}
