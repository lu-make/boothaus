using Boothaus.GUI.ViewModels;
using System.Windows;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for BootMaske.xaml
/// </summary>
public partial class BootMaske : Window
{
    private BootMaskeViewmodel viewmodel;

    public BootMaske(BootMaskeViewmodel viewmodel)
    {
        InitializeComponent();
        this.viewmodel = viewmodel;
    }
}
