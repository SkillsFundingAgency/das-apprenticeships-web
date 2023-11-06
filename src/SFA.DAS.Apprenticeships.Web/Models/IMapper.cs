namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IMapper<T>
    {
        T Map(object sourceObject);
    }
}
