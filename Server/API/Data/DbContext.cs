using Microsoft.EntityFrameworkCore;
using Server.API.Models;

namespace Server.API.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Main tables
    public DbSet<UserModel> Users { get; set; }
    public DbSet<RoleModel> Roles { get; set; }
    public DbSet<ContactModel> Contact { get; set; }
    public DbSet<PageModel> Pages { get; set; }
    public DbSet<BoardMemberModel> BoardMembers { get; set; }
    public DbSet<ImageModel> Images { get; set; }
    public DbSet<ExternalMediaModel> ExternalMedia { get; set; }
    public DbSet<SlideModel> Slides { get; set; }
    public DbSet<MenuItemModel> MenuItems { get; set; }

    // Translation tables
    public DbSet<PageTranslationModel> PageTranslations { get; set; }
    public DbSet<BoardMemberTranslationModel> BoardMemberTranslations { get; set; }
    public DbSet<ImageAltTranslationModel> ImageAltTranslations { get; set; }
    public DbSet<ExternalMediaTranslationModel> ExternalMediaTranslations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the PageTranslation entity.
        modelBuilder.Entity<PageTranslationModel>()
            .HasKey(pt => new { pt.PageId, pt.LanguageCode }); // Composite primary key.

        modelBuilder.Entity<PageTranslationModel>()
            .HasOne(pt => pt.Page)
            .WithMany(p => p.Translations)
            .HasForeignKey(pt => pt.PageId);

        // Configure the BoardMemberTranslation entity.
        modelBuilder.Entity<BoardMemberTranslationModel>()
            .HasKey(bmt => new { bmt.BoardMemberId, bmt.LanguageCode }); // Composite primary key.

        modelBuilder.Entity<BoardMemberTranslationModel>()
            .HasOne(bmt => bmt.BoardMember)
            .WithMany(bm => bm.Translations)
            .HasForeignKey(bmt => bmt.BoardMemberId);

        // Configure the ImageAltTranslation entity.
        modelBuilder.Entity<ImageAltTranslationModel>()
            .HasKey(iat => new { iat.ImageId, iat.LanguageCode }); // Composite primary key.

        modelBuilder.Entity<ImageAltTranslationModel>()
            .HasOne(iat => iat.Image)
            .WithMany(i => i.Translations)
            .HasForeignKey(iat => iat.ImageId);

        // Configure the ExternalMediaTranslation entity.
        modelBuilder.Entity<ExternalMediaTranslationModel>()
            .HasKey(emt => new { emt.ExternalMediaId, emt.LanguageCode }); // Composite primary key.

        modelBuilder.Entity<ExternalMediaTranslationModel>()
            .HasOne(emt => emt.ExternalMedia)
            .WithMany(em => em.Translations)
            .HasForeignKey(emt => emt.ExternalMediaId);
        
        // Configure the Slide entity.
        modelBuilder.Entity<SlideModel>()
            .HasKey(s => s.ImageId); // Primary key is ImageId.

        // Configure the saving of ExternalMedia property Type as string.
        modelBuilder.Entity<ExternalMediaModel>()
            .Property(em => em.Type)
            .HasConversion<string>();
        
        // Configure the saving of PageModel property Type as string.
        modelBuilder.Entity<PageModel>()
            .Property(p => p.Type)
            .HasConversion<string>();
    }
}