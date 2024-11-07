using FluentAssertions;
using SFA.DAS.Apprenticeships.Web.Extensions;
using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.UnitTests.Extensions;

public class BannerExtensionsTests
{
    [TestCase(EmployerApprenticeDetailsBanners.ChangeOfPriceApproved | EmployerApprenticeDetailsBanners.ChangeOfStartDateRejected, (ulong)34)]
    [TestCase(EmployerApprenticeDetailsBanners.ChangeOfPriceRequestSent, (ulong)4)]
    public void AppendEmployerBannersToUrl_ReturnsExpectedResult(EmployerApprenticeDetailsBanners banners, ulong expectedBannerValue)
    {
        var result = Guid.NewGuid().ToString().AppendEmployerBannersToUrl(banners);
        result.Should().EndWith($"?banners={expectedBannerValue}");
    }

    [TestCase(ProviderApprenticeDetailsBanners.ChangeOfPriceApproved | ProviderApprenticeDetailsBanners.ChangeOfPriceAutoApproved, (ulong)96)]
    [TestCase(ProviderApprenticeDetailsBanners.ChangeOfPriceRequestSent, (ulong)8)]
    public void AppendProviderBannersToUrl_ReturnsExpectedResult(ProviderApprenticeDetailsBanners banners, ulong expectedBannerValue)
    {
        var result = Guid.NewGuid().ToString().AppendProviderBannersToUrl(banners);
        result.Should().EndWith($"?banners={expectedBannerValue}");
    }
}