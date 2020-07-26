using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DBModel.SqlModels
{
    public partial class MWDBContext : DbContext
    {

        public MWDBContext()
        {
        }

        public MWDBContext(DbContextOptions<MWDBContext> options) : base(options)
        {
        }


        public virtual DbSet<ActionLog> ActionLog { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<BoardInfo> BoardInfo { get; set; }
        public virtual DbSet<Moicasha256> Moicasha256 { get; set; }
        public virtual DbSet<Moicasn> Moicasn { get; set; }
        public virtual DbSet<PostRank> PostRank { get; set; }
        public virtual DbSet<Verification> Verification { get; set; }
        public virtual DbSet<VerifyType> VerifyType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Server=twid.database.windows.net;Database=TWID_APP;user id=teemo;password=Aa@!2019;Connect Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionLog>(entity =>
            {
                entity.HasKey(e => e.No);

                entity.Property(e => e.No).HasColumnName("no");

                entity.Property(e => e.Action).HasMaxLength(50);

                entity.Property(e => e.ClientIp)
                    .HasColumnName("ClientIP")
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(150);

                entity.Property(e => e.UserName).HasMaxLength(40);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId })
                    .HasName("PK_dbo.AspNetUserLogins");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_dbo.AspNetUserRoles");

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.RoleId).HasMaxLength(128);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<BoardInfo>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Board)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ChineseDes).HasMaxLength(50);

                entity.Property(e => e.LastUpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Moderators).HasMaxLength(50);
            });

            modelBuilder.Entity<Moicasha256>(entity =>
            {
                entity.HasKey(e => e.No)
                    .HasName("PK_MOICAHMACSHA256");

                entity.ToTable("MOICASHA256");

                entity.Property(e => e.No)
                    .HasColumnName("no")
                    .ValueGeneratedNever();

                entity.Property(e => e.Hmacsha256)
                    .IsRequired()
                    .HasColumnName("HMACSHA256")
                    .HasMaxLength(64);
            });

            modelBuilder.Entity<Moicasn>(entity =>
            {
                entity.HasKey(e => e.No);

                entity.ToTable("MOICASN");

                entity.Property(e => e.No)
                    .HasColumnName("no")
                    .ValueGeneratedNever();

                entity.Property(e => e.Sn)
                    .IsRequired()
                    .HasColumnName("SN")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<PostRank>(entity =>
            {
                entity.HasKey(e => e.No);

                entity.Property(e => e.No)
                    .HasColumnName("no")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aid)
                    .IsRequired()
                    .HasColumnName("AID")
                    .HasMaxLength(15)
                    .IsFixedLength();

                entity.Property(e => e.Board)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsFixedLength();

                entity.Property(e => e.Pttid)
                    .IsRequired()
                    .HasColumnName("PTTID")
                    .HasMaxLength(15)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Verification>(entity =>
            {
                entity.HasKey(e => e.No)
                    .HasName("PK_ChkPTTID");

                entity.Property(e => e.No).HasColumnName("no");

                entity.Property(e => e.Base5).HasMaxLength(5);

                entity.Property(e => e.CreateDateIp)
                    .HasColumnName("CreateDateIP")
                    .HasMaxLength(30);

                entity.Property(e => e.ModifyDateIp)
                    .HasColumnName("ModifyDateIP")
                    .HasMaxLength(30);

                entity.Property(e => e.Pttid)
                    .IsRequired()
                    .HasColumnName("PTTID")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<VerifyType>(entity =>
            {
                entity.HasKey(e => e.No);

                entity.Property(e => e.No)
                    .HasColumnName("no")
                    .ValueGeneratedNever();

                entity.Property(e => e.TypeName).HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
