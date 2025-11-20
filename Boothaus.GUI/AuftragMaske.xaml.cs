using Boothaus.Domain;
using Boothaus.GUI.ViewModels;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using Domain.Services;
using System.Windows;
using System.Windows.Media;

namespace Boothaus.GUI;

/// <summary>
/// Interaction logic for AuftragMaske.xaml
/// </summary>
public partial class AuftragMaske : ThemedWindow
{
    public AuftragMaske(AuftragMaskeViewmodel viewmodel)
    {
        InitializeComponent();
        DataContext = viewmodel; 
        viewmodel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewmodel.Ergebnis) && viewmodel.Ergebnis.HasValue)
                DialogResult = viewmodel.Ergebnis;
        };
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
