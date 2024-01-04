using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Provider.Shared.UI.Startup;

namespace SFA.DAS.Apprenticeships.Web.Extensions;

public static class ZenDeskExtensions
{
    public static IMvcBuilder AddZenDeskSettings(this IMvcBuilder builder, IConfiguration config)
    {
        var zenDeskConfiguration = config.GetSection("ZenDesk").Get<ZenDeskConfiguration>();
        builder.SetZenDeskConfiguration(zenDeskConfiguration);

        return builder;
    }
}