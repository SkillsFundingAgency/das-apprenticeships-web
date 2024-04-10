namespace SFA.DAS.Apprenticeships.Web.Models
{
    public interface IMapper
    {
        T Map<T>(object sourceObject);
    }

    public interface IMapper<T>
    {
        T Map(object sourceObject);
    }

    public class MapperResolver: IMapper
    {
        private Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        public void Register<T>(IMapper<T> mapper)
        {
            _mappers.Add(typeof(T), mapper);
        }

        public T Map<T>(object sourceObject)
        {
            var mapper = (IMapper<T>)_mappers[typeof(T)];
            return mapper.Map(sourceObject);
        }
    }
}
