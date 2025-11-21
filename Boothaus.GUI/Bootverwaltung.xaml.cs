using Boothaus.GUI.ViewModels;
using DevExpress.Xpf.Core;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for Bootverwaltung.xaml
/// </summary>
public partial class Bootverwaltung : ThemedWindow
{
    private BootverwaltungViewmodel viewmodel;

    public Bootverwaltung(BootverwaltungViewmodel viewmodel)
    {
        InitializeComponent();
        this.viewmodel = viewmodel;
    }
}
