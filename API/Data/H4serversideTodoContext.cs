using System;
using System.Collections.Generic;

using API.Data.Model;

using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class H4serversideTodoContext : DbContext {
    public H4serversideTodoContext() {
    }

    public H4serversideTodoContext(DbContextOptions<H4serversideTodoContext> options)
        : base(options) {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountSession> AccountSessions { get; set; }

    public virtual DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Account>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC0773637072");

            entity.ToTable("Account");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Cpr).HasColumnName("CPR");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(96)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<AccountSession>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__AccountS__3214EC07D0CA667F");

            entity.ToTable("AccountSession");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.LastAuthenticated).HasColumnType("datetime");
            entity.Property(e => e.Token).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountSessions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccountSessionAccountId");
        });

        modelBuilder.Entity<TodoItem>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__TodoItem__3214EC072750E51F");

            entity.ToTable("TodoItem");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasOne(d => d.Account).WithMany(p => p.TodoItems)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TodoItemAccountId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
