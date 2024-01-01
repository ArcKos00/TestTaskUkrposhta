using Microsoft.Data.SqlClient;
using System.Reflection;
using TestTaskUkrPoshta.Repositories.Interfaces;

namespace TestTaskUkrPoshta.Repositories
{
    public class SqlRepository(string connectionString) : ISqlRepository
    {
        public async Task ExecuteSqlRawAsync(string query)
        {
            await using SqlConnection connection = new(connectionString);
            await using SqlCommand command = new(query, connection);

            try
            {
                await connection.OpenAsync();
                Console.WriteLine($"Executing query...{Environment.NewLine + query}");
                await command.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task ExecuteSqlRawAsync(IEnumerable<string> queries)
        {
            await using SqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            foreach (var query in queries)
            {
                try
                {
                    await using SqlCommand command = new(query, connection);
                    Console.WriteLine($"Executing query...{Environment.NewLine + query}");
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<T>> ExecuteSqlRawAsync<T>(string query)
        {
            IEnumerable<T> result = new List<T>();
            await using SqlConnection connection = new(connectionString);
            await using SqlCommand command = new(query, connection);

            try
            {
                await connection.OpenAsync();
                Console.WriteLine($"Executing query...{Environment.NewLine + query}");
                result = await ReadAsync<T>(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        private async Task<IEnumerable<T>> ReadAsync<T>(SqlCommand command)
        {
            var result = new List<T>();
            T obj = default!;

            await using var reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(reader[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, ParseReadValue(reader[prop.Name]), null);
                    }
                }
                result.Add(obj);
            }

            return result;
        }

        private object ParseReadValue(object value)
        {
            return value;
        }
    }
}
