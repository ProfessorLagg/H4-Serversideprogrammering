using System;
using System.Collections.Generic;

using API.Data.Model;

using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class H4serversideTodoContext : DbContext {
    public H4serversideTodoContext() { }

    public H4serversideTodoContext(DbContextOptions<H4serversideTodoContext> options) : base(options) { }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer("Name=ConnectionStrings:Default");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Account>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC07F712C85F");

            entity.ToTable("Account");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cpr).HasColumnName("CPR");
            entity.Property(e => e.Login).HasColumnName("login");
        });

        modelBuilder.Entity<TodoItem>(entity => {
            entity.HasKey(e => e.Id).HasName("PK__TodoItem__3214EC07D8E5E24B");

            entity.ToTable("TodoItem");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.TodoItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
