using Domain.Services;

namespace Boothaus.Services;

public class DruckService
{
    private readonly LagerApplicationService appService;

    public DruckService(LagerApplicationService appService)
    {
        this.appService = appService;
    }

    public void LagerkalenderDrucken()
    {
        var lager = appService.GetLager();

    }
}
