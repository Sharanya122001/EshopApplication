namespace MinimalEshop.Application.Interface
{
    public interface ICounterRepo
    {
        Task<long> GetNextOrderNumberAsync();
    }

}
