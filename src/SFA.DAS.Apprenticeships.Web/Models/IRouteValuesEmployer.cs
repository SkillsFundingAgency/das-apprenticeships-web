namespace SFA.DAS.Apprenticeships.Web.Models;

public interface IRouteValuesEmployer
{
    public string? ApprenticeshipHashedId { get; set; }
    public string EmployerAccountId { get; set; }
}