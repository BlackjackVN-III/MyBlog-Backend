using Blog.Domain.Entities;
using Blog.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<BlogPost> Blogs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostTag>(x => x.HasKey(p => new { p.PostId, p.TagId }));

            modelBuilder.Entity<PostTag>()
                .HasOne(x => x.Tag)
                .WithMany(p => p.PostTags)
                .HasForeignKey(x => x.TagId);

            modelBuilder.Entity<PostTag>()
                .HasOne(x => x.BlogPost)
                .WithMany(p => p.PostTags)
                .HasForeignKey(x => x.PostId);


            // 1. Cấu hình User
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedNever(); // Bắt buộc: Nhận ID từ AppUser         
                entity.Property(u => u.AvatarUrl).HasMaxLength(500);
                entity.Property(u => u.Bio).HasMaxLength(500);
            });

            // Liên kết 1-1 giữa Identity và Domain
            modelBuilder.Entity<AppUser>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<User>(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. Cấu hình Post
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(255).IsRequired();
                entity.Property(p => p.Slug).HasMaxLength(255).IsRequired();
                entity.Property(p => p.Summary).HasMaxLength(500);

                entity.HasIndex(p => p.Slug).IsUnique(); // Bắt buộc: Đảm bảo URL không trùng lặp
            });

            // 3. Cấu hình Tag
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(t => t.Name).HasMaxLength(100).IsRequired();
                entity.Property(t => t.Slug).HasMaxLength(100).IsRequired();
                entity.HasIndex(t => t.Slug).IsUnique();
            });

            // 5. Cấu hình bảng Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).HasMaxLength(1000).IsRequired();

                // Giữ nguyên Cascade cho Post (Khi xóa bài viết thì xóa hết bình luận của bài đó)
                entity.HasOne(c => c.BlogPost)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.BlogPostId)
                      .OnDelete(DeleteBehavior.Cascade);

                // BẮT BUỘC TẮT CASCADE CHO USER (Sửa lỗi 1785 ở đây)
                entity.HasOne(c => c.   Author)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.NoAction);

                // BẮT BUỘC TẮT CASCADE CHO PARENT COMMENT (Để tránh lỗi vòng lặp)
                entity.HasOne(c => c.Parent)
                      .WithMany()
                      .HasForeignKey(c => c.ParentId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            /* // . Cấu hình bảng User (Domain)
             modelBuilder.Entity<User>(entity =>
             {
                 entity.ToTable("Users");
                 entity.HasKey(u => u.Id);
                 // Bảng này lấy ID từ AppUser truyền sang, nên không tự generate
                 entity.Property(u => u.Id).ValueGeneratedNever();
             });

             modelBuilder.Entity<AppUser>()
             .HasOne<User>()
             .WithOne()
             .HasForeignKey<User>(u => u.Id)
             .OnDelete(DeleteBehavior.Cascade);*/




        }
    }
}
