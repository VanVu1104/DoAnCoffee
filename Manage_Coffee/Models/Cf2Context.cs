using System;
using System.Collections.Generic;
using Manage_Coffee.Migrations;
using Manage_Coffee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Manage_Coffee.Models;

public partial class Cf2Context : IdentityDbContext<ApplicationUser>
{
    public Cf2Context()
    {
    }

    public Cf2Context(DbContextOptions<Cf2Context> options)
        : base(options)
    {
    }
    public DbSet<Manage_Coffee.Models.SignUpUserModel> SignUpUserModel { get; set; } = default!;
    public DbSet<Manage_Coffee.Models.SignInModel> SignInModel { get; set; } = default!;
    public DbSet<Manage_Coffee.Models.ChangePasswordModel> ChangePasswordModel { get; set; } = default!;
    public virtual DbSet<Bom> Boms { get; set; }

    public virtual DbSet<ChiNhanh> ChiNhanhs { get; set; }

    public virtual DbSet<ChitietDanhGium> ChitietDanhGia { get; set; }

    public virtual DbSet<Ctphieu> Ctphieus { get; set; }

    public virtual DbSet<CtsanPham> CtsanPhams { get; set; }

    public virtual DbSet<Ctsponl> Ctsponls { get; set; }

    public virtual DbSet<DanhMucCa> DanhMucCas { get; set; }

    public virtual DbSet<DanhMucKm> DanhMucKms { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<NguyenVatLieu> NguyenVatLieus { get; set; }

    public virtual DbSet<Nhacungcap> Nhacungcaps { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuNhapXuat> PhieuNhapXuats { get; set; }

    public virtual DbSet<PhieuOrder> PhieuOrders { get; set; }

    public virtual DbSet<Phieudhonl> Phieudhonls { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-29MRHMV\\SQLEXPRESS;Database=cf7;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bom>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.MaNvl }).HasName("PK__BOM__74849F9A3EE903C7");

            entity.ToTable("BOM");

            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.MaNvl)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNVL");

            entity.HasOne(d => d.MaNvlNavigation).WithMany(p => p.Boms)
                .HasForeignKey(d => d.MaNvl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM__MaNVL__628FA481");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.Boms)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BOM__MaSP__619B8048");
        });

        modelBuilder.Entity<ChiNhanh>(entity =>
        {
            entity.HasKey(e => e.MaCn).HasName("PK__ChiNhanh__27258E0E20D82064");

            entity.ToTable("ChiNhanh");

            entity.Property(e => e.MaCn)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaCN");
            entity.Property(e => e.Diachi).HasMaxLength(50);
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<ChitietDanhGium>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.MaKh }).HasName("PK__ChitietD__D55754ED97C2B4EC");

            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.ChitietDanhGia)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChitietDan__MaKH__66603565");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChitietDanhGia)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChitietDan__MaSP__656C112C");
        });

        modelBuilder.Entity<Ctphieu>(entity =>
        {
            entity.HasKey(e => new { e.MaNvl, e.MaPhieu }).HasName("PK__CTPhieu__287F739A30E43D33");

            entity.ToTable("CTPhieu");

            entity.Property(e => e.MaNvl)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNVL");
            entity.Property(e => e.MaPhieu)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaNvlNavigation).WithMany(p => p.Ctphieus)
                .HasForeignKey(d => d.MaNvl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTPhieu__MaNVL__693CA210");

            entity.HasOne(d => d.MaPhieuNavigation).WithMany(p => p.Ctphieus)
                .HasForeignKey(d => d.MaPhieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTPhieu__MaPhieu__6A30C649");
        });

        modelBuilder.Entity<CtsanPham>(entity =>
        {
            entity.HasKey(e => new { e.MaOrder, e.MaSp, e.MaKh, e.MaSize }).HasName("PK__CTSanPha__E3CA93C7BB55FCEE");

            entity.ToTable("CTSanPham");

            entity.Property(e => e.MaOrder)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaSize)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Ghichu).HasMaxLength(50);

            entity.HasOne(d => d.MaOrderNavigation).WithMany(p => p.CtsanPhams)
                .HasForeignKey(d => d.MaOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSanPham__MaOrd__5CD6CB2B");

            entity.HasOne(d => d.MaSizeNavigation).WithMany(p => p.CtsanPhams)
                .HasForeignKey(d => d.MaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSanPham__MaSiz__5EBF139D");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.CtsanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSanPham__MaSP__5DCAEF64");
        });

        modelBuilder.Entity<Ctsponl>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.MaPhieuonl, e.MaSize, e.MaKh }).HasName("PK__CTSPonl__6B340E67A2D88F1E");

            entity.ToTable("CTSPonl");

            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.MaPhieuonl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSize)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.Ghichu).HasMaxLength(50);

            entity.HasOne(d => d.MaPhieuonlNavigation).WithMany(p => p.Ctsponls)
                .HasForeignKey(d => d.MaPhieuonl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSPonl__MaPhieu__6E01572D");

            entity.HasOne(d => d.MaSizeNavigation).WithMany(p => p.Ctsponls)
                .HasForeignKey(d => d.MaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSPonl__MaSize__6EF57B66");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.Ctsponls)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CTSPonl__MaSP__6D0D32F4");
        });

        modelBuilder.Entity<DanhMucCa>(entity =>
        {
            entity.HasKey(e => new { e.Ngay, e.MaNv }).HasName("PK__DanhMucC__D9BEBAC2B9F2395D");

            entity.ToTable("DanhMucCa");

            entity.Property(e => e.Ngay).HasColumnType("date");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.Calam)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.DanhMucCas)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DanhMucCa__MaNV__4222D4EF");
        });

        modelBuilder.Entity<DanhMucKm>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__DanhMucK__2725CF155B7D31EE");

            entity.ToTable("DanhMucKM");

            entity.Property(e => e.MaKm)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKM");
            entity.Property(e => e.GiaTri)
                .HasColumnName("GiaTri");
            entity.Property(e => e.Hanmuc)
                .HasColumnName("Hanmuc");
            entity.Property(e => e.Ten).HasMaxLength(50);
            entity.Property(e => e.Ngayhethan).HasColumnType("date");
            entity.Property(e => e.Ngayapdung).HasColumnType("date");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh).HasName("PK__KhachHan__2725CF1E3B146E27");

            entity.ToTable("KhachHang");

            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.Diachi).HasMaxLength(50);
            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.GioiTinh).IsUnicode(false);
            entity.Property(e => e.Matkhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Sdt).HasColumnName("SDT");
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.Maloai).HasName("PK__Loai__3E1DB46D4D7258A9");

            entity.ToTable("Loai");

            entity.Property(e => e.Maloai)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<NguyenVatLieu>(entity =>
        {
            entity.HasKey(e => e.MaNvl).HasName("PK__NguyenVa__3A197864454B6A99");

            entity.ToTable("NguyenVatLieu");

            entity.Property(e => e.MaNvl)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNVL");
            entity.Property(e => e.Anh)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Dvt)
                .HasMaxLength(50)
                .HasColumnName("DVT");
            entity.Property(e => e.Mota).HasMaxLength(500);
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<Nhacungcap>(entity =>
        {
            entity.HasKey(e => e.MaCcap).HasName("PK__Nhacungc__1A1475A84B56D97C");

            entity.ToTable("Nhacungcap");

            entity.Property(e => e.MaCcap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Diachi).HasMaxLength(50);
            entity.Property(e => e.Sdt).HasColumnName("SDT");
            entity.Property(e => e.Ten).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D70A2E229495");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.Chucvu).HasMaxLength(50);
            entity.Property(e => e.Diachi).HasMaxLength(50);
            entity.Property(e => e.GioiTinh).IsUnicode(false);
            entity.Property(e => e.MaCn)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaCN");
            entity.Property(e => e.Mkhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ngaysinh).HasColumnType("date");
            entity.Property(e => e.Ten).HasMaxLength(50);

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaCn)
                .HasConstraintName("FK__NhanVien__MaCN__3D5E1FD2");
        });

        modelBuilder.Entity<PhieuNhapXuat>(entity =>
        {
            entity.HasKey(e => e.MaPhieu).HasName("PK__PhieuNha__2660BFE0404E71E8");

            entity.ToTable("PhieuNhapXuat");

            entity.Property(e => e.MaPhieu)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Diachi).HasMaxLength(50);
            entity.Property(e => e.Loai).HasMaxLength(50);
            entity.Property(e => e.MaCcap)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayLap).HasColumnType("date");

            entity.HasOne(d => d.MaCcapNavigation).WithMany(p => p.PhieuNhapXuats)
                .HasForeignKey(d => d.MaCcap)
                .HasConstraintName("FK__PhieuNhap__MaCca__59FA5E80");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuNhapXuats)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuNhapX__MaNV__59063A47");
        });

        modelBuilder.Entity<PhieuOrder>(entity =>
        {
            entity.HasKey(e => e.MaOrder).HasName("PK__PhieuOrd__50559EF73AF128AD");

            entity.ToTable("PhieuOrder");

            entity.Property(e => e.MaOrder)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaCn)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaCN");
            
            entity.Property(e => e.MaKm)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKM");
            entity.Property(e => e.MaNv)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.Ngaygiodat).HasColumnType("datetime");
         
            entity.Property(e => e.Pttt)
                .HasMaxLength(50)
                .HasColumnName("PTTT");
            entity.Property(e => e.Ten)
                .HasMaxLength(50)
                .HasColumnName("Ten");
            entity.Property(e => e.Sdt)
                .HasColumnName("Sdt");

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.PhieuOrders)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PhieuOrder__MaCN__5070F446");


            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.PhieuOrders)
                .HasForeignKey(d => d.MaKm)
                .HasConstraintName("FK__PhieuOrder__MaKM__52593CB8");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.PhieuOrders)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK__PhieuOrder__MaNV__5165187F");
        });

        modelBuilder.Entity<Phieudhonl>(entity =>
        {
            entity.HasKey(e => e.MaPhieuonl).HasName("PK__Phieudho__144F3C04508AE3D5");

            entity.ToTable("Phieudhonl");

            entity.Property(e => e.MaPhieuonl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiaChi).HasMaxLength(50);
            entity.Property(e => e.MaCn)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaCN");
            entity.Property(e => e.MaKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaKm)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaKM");
            entity.Property(e => e.Ngaygiodat).HasColumnType("datetime");
            entity.Property(e => e.Ptnh)
                .HasMaxLength(50)
                .HasColumnName("PTNH");
            entity.Property(e => e.Pttt)
                .HasMaxLength(50)
                .HasColumnName("PTTT");

            entity.HasOne(d => d.MaCnNavigation).WithMany(p => p.Phieudhonls)
                .HasForeignKey(d => d.MaCn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Phieudhonl__MaCN__49C3F6B7");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Phieudhonls)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Phieudhonl__MaKH__48CFD27E");

            entity.HasOne(d => d.MaKmNavigation).WithMany(p => p.Phieudhonls)
                .HasForeignKey(d => d.MaKm)
                .HasConstraintName("FK__Phieudhonl__MaKM__4AB81AF0");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081CCC8EB8FA");

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSp)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaSP");
            entity.Property(e => e.Anh)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Dvt)
                .HasMaxLength(50)
                .HasColumnName("DVT");
            entity.Property(e => e.MaTopping)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Maloai)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Mota)
                .HasMaxLength(500)
                .HasColumnName("MOTA");
            entity.Property(e => e.Ten).HasMaxLength(50);

            entity.HasOne(d => d.MaToppingNavigation).WithMany(p => p.InverseMaToppingNavigation)
                .HasForeignKey(d => d.MaTopping)
                .HasConstraintName("FK__SanPham__MaToppi__5629CD9C");

            entity.HasOne(d => d.MaloaiNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.Maloai)
                .HasConstraintName("FK__SanPham__Maloai__5535A963");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.MaSize).HasName("PK__Size__A787E7ED45B00A2A");

            entity.ToTable("Size");

            entity.Property(e => e.MaSize)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Ten).HasMaxLength(50);
        });
        modelBuilder
                  .HasAnnotation("ProductVersion", "8.0.8")
                  .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("Manage_Coffee.Models.ApplicationUser", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("nvarchar(450)");

            b.Property<int>("AccessFailedCount")
                .HasColumnType("int");

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .HasColumnType("nvarchar(max)");

            b.Property<DateTime?>("DateOfBirth")
                .HasColumnType("datetime2");

            b.Property<string>("Email")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<bool>("EmailConfirmed")
                .HasColumnType("bit");

            b.Property<string>("FirstName")
               // .IsRequired()
                .HasColumnType("nvarchar(max)");

            b.Property<string>("LastName")
               // .IsRequired()
                .HasColumnType("nvarchar(max)");

            b.Property<bool>("LockoutEnabled")
                .HasColumnType("bit");

            b.Property<DateTimeOffset?>("LockoutEnd")
                .HasColumnType("datetimeoffset");

            b.Property<string>("NormalizedEmail")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("NormalizedUserName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("PasswordHash")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("PhoneNumber")
                .HasColumnType("nvarchar(max)");

            b.Property<bool>("PhoneNumberConfirmed")
                .HasColumnType("bit");

            b.Property<string>("SecurityStamp")
                .HasColumnType("nvarchar(max)");

            b.Property<bool>("TwoFactorEnabled")
                .HasColumnType("bit");

            b.Property<string>("UserName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.HasKey("Id");

            b.HasIndex("NormalizedEmail")
                .HasDatabaseName("EmailIndex");

            b.HasIndex("NormalizedUserName")
                .IsUnique()
                .HasDatabaseName("UserNameIndex")
                .HasFilter("[NormalizedUserName] IS NOT NULL");

            b.ToTable("AspNetUsers", (string)null);
        });

        modelBuilder.Entity("Manage_Coffee.Models.SignUpUserModel", b =>
        {
            b.Property<string>("Email")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ConfirmPassword")
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            b.Property<string>("FirstName")          
                .HasColumnType("nvarchar(max)");

            b.Property<string>("LastName")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("Password")
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            b.HasKey("Email");

            b.ToTable("SignUpUserModel");
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
        {
            b.Property<string>("Id")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ConcurrencyStamp")
                .IsConcurrencyToken()
                .HasColumnType("nvarchar(max)");

            b.Property<string>("Name")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.Property<string>("NormalizedName")
                .HasMaxLength(256)
                .HasColumnType("nvarchar(256)");

            b.HasKey("Id");

            b.HasIndex("NormalizedName")
                .IsUnique()
                .HasDatabaseName("RoleNameIndex")
                .HasFilter("[NormalizedName] IS NOT NULL");

            b.ToTable("AspNetRoles", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("ClaimType")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("ClaimValue")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("RoleId")
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            b.HasKey("Id");

            b.HasIndex("RoleId");

            b.ToTable("AspNetRoleClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

            b.Property<string>("ClaimType")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("ClaimValue")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("UserId")
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            b.HasKey("Id");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserClaims", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
        {
            b.Property<string>("LoginProvider")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ProviderKey")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("ProviderDisplayName")
                .HasColumnType("nvarchar(max)");

            b.Property<string>("UserId")
                .IsRequired()
                .HasColumnType("nvarchar(450)");

            b.HasKey("LoginProvider", "ProviderKey");

            b.HasIndex("UserId");

            b.ToTable("AspNetUserLogins", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
        {
            b.Property<string>("UserId")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("RoleId")
                .HasColumnType("nvarchar(450)");

            b.HasKey("UserId", "RoleId");

            b.HasIndex("RoleId");

            b.ToTable("AspNetUserRoles", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
        {
            b.Property<string>("UserId")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("LoginProvider")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("Name")
                .HasColumnType("nvarchar(450)");

            b.Property<string>("Value")
                .HasColumnType("nvarchar(max)");

            b.HasKey("UserId", "LoginProvider", "Name");

            b.ToTable("AspNetUserTokens", (string)null);
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
        {
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
        {
            b.HasOne("Manage_Coffee.Models.ApplicationUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
        {
            b.HasOne("Manage_Coffee.Models.ApplicationUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
        {
            b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne("Manage_Coffee.Models.ApplicationUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
        {
            b.HasOne("Manage_Coffee.Models.ApplicationUser", null)
                .WithMany()
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });
        OnModelCreatingPartial(modelBuilder);
        
    }
       
   partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

}
