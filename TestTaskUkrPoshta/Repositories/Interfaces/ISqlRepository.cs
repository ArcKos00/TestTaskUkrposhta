namespace TestTaskUkrPoshta.Repositories.Interfaces
{
    public interface ISqlRepository
    {
        Task<IEnumerable<T>> ExecuteSqlRawAsync<T>(string query);
        Task ExecuteSqlRawAsync(string query);
        Task ExecuteSqlRawAsync(IEnumerable<string> queries);
    }
}
