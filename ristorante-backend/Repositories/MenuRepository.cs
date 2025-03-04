using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Repositories
{
    public class MenuRepository
    {
        private const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;TrustServerCertificate=True";

        private Menu ReadMenu(SqlDataReader reader)
        {
            var id = reader.GetInt32(reader.GetOrdinal("id"));
            var nome = reader.GetString(reader.GetOrdinal("nome"));
            var menu = new Menu(id, nome);
            return menu;
        }

        public async Task<List<Menu>> GetAllMenuAsync()
        {
            string query = "SELECT * FROM menu";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            List<Menu> menu = new List<Menu>();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        menu.Add(ReadMenu(reader));
                    }
                }
            }
            return menu;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            string query = "SELECT * FROM menu where id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("id", id);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Menu menu = ReadMenu(reader);
                    return menu;
                }

            }
            return null;
        }

        public async Task<int> InsertMenu(Menu menu)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            string query = $"INSERT INTO menu (nome) VALUES (@nome);" +
                        $"SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@nome", menu.Nome));

                menu.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return menu.Id;
            }
        }

        public async Task<int> UpdateMenuAsync(int id, Menu menu)
        {
            string query = "UPDATE Menu SET nome = @nome WHERE Id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nome", menu.Nome);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteMenuAsync(int id)
        {
            string query = "DELETE FROM menu WHERE Id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
