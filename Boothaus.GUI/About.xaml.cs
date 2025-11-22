using DevExpress.Xpf.Core;
using System.Reflection;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for About.xaml
/// </summary>
public partial class About : ThemedWindow
{
    public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.0.0";

    public About()
    {
        InitializeComponent();
        DataContext = this;
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
