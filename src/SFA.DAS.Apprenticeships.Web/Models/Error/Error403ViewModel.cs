namespace SFA.DAS.Apprenticeships.Web.Models.Error
{
    public class Error403ViewModel
    {
        public bool UseDfESignIn { get; set; }
        public string HelpPageLink { get; set; }
        //TODO Once parent app connected, need to set up this HomePage link to (presumably) the PAS home page
        public string HomePageLink { get; set; }
    }
}
