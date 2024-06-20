using SFA.DAS.Apprenticeships.Web.Models.Enums;

namespace SFA.DAS.Apprenticeships.Web.Extensions
{
    public static class BannerExtensions
    {
        public static string AppendEmployerBannersToUrl(this string url, params EmployerApprenticeDetailsBanners[] banners)
        {
            EmployerApprenticeDetailsBanners bannersCombined = 0;
            foreach (var banner in banners)
            {
                bannersCombined |= banner;
            }

            return BuildUrl(url, (ulong)bannersCombined);
        }

        public static string AppendProviderBannersToUrl(this string url, params ProviderApprenticeDetailsBanners[] banners)
        {
            ProviderApprenticeDetailsBanners bannersCombined = 0;
            foreach (var banner in banners)
            {
                bannersCombined |= banner;
            }

            return BuildUrl(url, (ulong)bannersCombined);
        }

        private static string BuildUrl(string url, ulong banners)
        {
            return $"{url}?banners={banners}";
        }
    }
}
