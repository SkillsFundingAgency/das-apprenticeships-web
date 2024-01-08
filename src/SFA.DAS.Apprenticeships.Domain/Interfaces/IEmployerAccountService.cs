using SFA.DAS.Apprenticeships.Domain.Employers;

namespace SFA.DAS.Apprenticeships.Domain.Interfaces
{
    public interface IEmployerAccountService
    {
        Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
    }
}