using Manage_Coffee.Models;
using Microsoft.Data.SqlClient;

namespace Manage_Coffee.Service
{
    public class KhachHangService
    {
        private readonly string _connectionString;

        public KhachHangService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<KhachHang> GetKhachHangByIdAsync(string maKh)
        {
            KhachHang khachHang = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string sql = "SELECT MaKh, Ten, DiaChi, Sdt, Email, GioiTinh, Role FROM KhachHang WHERE MaKh = @MaKh";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(new SqlParameter("@MaKh", maKh));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            khachHang = new KhachHang
                            {
                                MaKh = reader["MaKh"].ToString(),
                                Ten = reader["Ten"]?.ToString(),
                                Diachi = reader["DiaChi"]?.ToString(),
                                Sdt = reader["Sdt"] != DBNull.Value ? Convert.ToInt32(reader["Sdt"]) : (int?)null,
                                Email = reader["Email"]?.ToString(),
                                GioiTinh = reader["GioiTinh"] != DBNull.Value ? Convert.ToBoolean(reader["GioiTinh"]) : (bool?)null,
                                Role = reader["Role"]?.ToString()
                            };
                        }
                    }
                }
            }

            return khachHang;
        }

        public async Task UpdateKhachHangAsync(KhachHang khachHang)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Mở kết nối

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Bước 1: Cập nhật bảng KhachHang
                        string updateKhachHangSql = @"
                    UPDATE KhachHang 
                    SET Ten = @Ten, DiaChi = @DiaChi, Sdt = @Sdt, Email = @Email, GioiTinh = @GioiTinh 
                    WHERE MaKh = @MaKh";

                        using (SqlCommand cmdKhachHang = new SqlCommand(updateKhachHangSql, connection, transaction))
                        {
                            cmdKhachHang.Parameters.AddWithValue("@Ten", khachHang.Ten ?? (object)DBNull.Value);
                            cmdKhachHang.Parameters.AddWithValue("@DiaChi", khachHang.Diachi ?? (object)DBNull.Value);
                            cmdKhachHang.Parameters.AddWithValue("@Sdt", khachHang.Sdt.HasValue ? khachHang.Sdt.Value : (object)DBNull.Value);
                            cmdKhachHang.Parameters.AddWithValue("@Email", khachHang.Email ?? (object)DBNull.Value);
                            cmdKhachHang.Parameters.AddWithValue("@GioiTinh", khachHang.GioiTinh.HasValue ? khachHang.GioiTinh.Value : (object)DBNull.Value);
                            cmdKhachHang.Parameters.AddWithValue("@MaKh", khachHang.MaKh);

                            await cmdKhachHang.ExecuteNonQueryAsync();
                        }

                        // Bước 2: Cập nhật bảng AspNetUsers nếu email khớp với MaKh
                        string updateAspNetUserSql = @"
                    UPDATE AspNetUsers 
                    SET PhoneNumber= @P, FirstName= @UserName, LastName=''
                    WHERE UserName = @Email";

                        using (SqlCommand cmdAspNetUsers = new SqlCommand(updateAspNetUserSql, connection, transaction))
                        {
                            cmdAspNetUsers.Parameters.AddWithValue("@UserName", khachHang.Ten ?? (object)DBNull.Value);
                            cmdAspNetUsers.Parameters.AddWithValue("@P", khachHang.Sdt.HasValue ? khachHang.Sdt.Value : (object)DBNull.Value);
                            cmdAspNetUsers.Parameters.AddWithValue("@Email", khachHang.Email ?? (object)DBNull.Value);

                            await cmdAspNetUsers.ExecuteNonQueryAsync();
                        }

                        // Commit transaction nếu cả 2 bảng cập nhật thành công
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback nếu có lỗi xảy ra trong quá trình cập nhật
                        transaction.Rollback();
                        throw new Exception("Có lỗi xảy ra khi cập nhật thông tin.", ex);
                    }
                }
            }
        }
    }
}
