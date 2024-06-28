namespace SFA.DAS.Apprenticeships.Web.Models.Error;

public class Error403ViewModel
{
    public Error403ViewModel(string dashboardLink, string helpPageLink, bool useDfESign)
    {
        DashboardLink = dashboardLink;
        HelpPageLink = helpPageLink;
        UseDfESignIn = useDfESign;
    }

    public bool UseDfESignIn { get; set; }
    public string HelpPageLink { get; set; }
    public string DashboardLink { get; set; }
}