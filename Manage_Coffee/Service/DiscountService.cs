using Manage_Coffee.Models;
using Microsoft.Data.SqlClient;

namespace Manage_Coffee.Service
{
	public class DiscountService
	{
		private readonly string _connectionString;

		public DiscountService(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<List<DanhMucKm>> GetValidDiscountsAsync(DateTime ngayDat)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				string query = @"
                SELECT MaKM, Ten, GiaTri, Soluong, Ngayapdung, Ngayhethan, Hanmuc 
                FROM DanhMucKM 
                WHERE Ngayapdung <= @NgayDat AND Ngayhethan >= @NgayDat AND Hanmuc > 0";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@NgayDat", ngayDat);

					var discounts = new List<DanhMucKm>();
					using (var reader = await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							discounts.Add(new DanhMucKm
							{
								MaKm = reader.GetString(0),
								Ten = reader.GetString(1),
								GiaTri = reader.GetInt32(2),
								Soluong = reader.GetInt32(3),
								Ngayapdung = reader.GetDateTime(4),
								Ngayhethan = reader.GetDateTime(5),
								Hanmuc = reader.GetInt32(6)
							});
						}
					}
					return discounts;
				}
			}
		}
	}

}
