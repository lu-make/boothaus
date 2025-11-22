using DevExpress.Xpf.Core;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for About.xaml
/// </summary>
public partial class About : ThemedWindow
{
    public About()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {

        var uri = e.Uri.AbsoluteUri;
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri) { UseShellExecute = true });

    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        Close();
    }
}
