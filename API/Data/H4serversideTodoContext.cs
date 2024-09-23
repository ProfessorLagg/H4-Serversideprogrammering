using System;
using System.Collections.Generic;
using API.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class H4serversideTodoContext : DbContext
{
    public H4serversideTodoContext()
    {
    }

    public H4serversideTodoContext(DbContextOptions<H4serversideTodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountSession> AccountSessions { get; set; }

    public virtual DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC07FAAD55A5");

            entity.ToTable("Account");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cpr).HasColumnName("CPR");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(192)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<AccountSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC072819F470");

            entity.ToTable("AccountSession");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Expires)
                .HasDefaultValueSql("(getdate()+'01:00:00')")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TodoItem__3214EC0735B6DD36");

            entity.ToTable("TodoItem");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Account).WithMany(p => p.TodoItems)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
