using Boothaus.Domain;

namespace Boothaus.GUI.ViewModels;

public class BootViewModel
{
    public Boot Modell { get; }

    public BootViewModel(Boot boot)
    {
        Modell = boot;
    }
}
 