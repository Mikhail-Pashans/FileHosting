using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
using FileHosting.Database.Models;

namespace FileHosting.Database
{
    public class UnitOfWork : DbContext, IUnitOfWork
    {
        #region Private Repos (add one per entity)

        private GenericRepository<Comment> _commentRepo;
        private GenericRepository<Download> _downloadRepo;
        private GenericRepository<File> _fileRepo;
        private GenericRepository<News> _newsRepo;
        private GenericRepository<Role> _roleRepo;
        private GenericRepository<Section> _sectionRepo;
        private GenericRepository<Tag> _tagRepo;
        private GenericRepository<User> _userRepo;

        #endregion

        #region Public DbSets (add one per entity)

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Constructor

        public UnitOfWork() : base("FileHosting")
        {
        }

        #endregion

        #region IUnitOfWork Implementation (add one per entity)

        public IGenericRepository<Comment> CommentRepository
        {
            get { return _commentRepo ?? (_commentRepo = new GenericRepository<Comment>(Comments)); }
        }

        public IGenericRepository<Download> DownloadRepository
        {
            get { return _downloadRepo ?? (_downloadRepo = new GenericRepository<Download>(Downloads)); }
        }

        public IGenericRepository<File> FileRepository
        {
            get { return _fileRepo ?? (_fileRepo = new GenericRepository<File>(Files)); }
        }

        public IGenericRepository<News> NewsRepository
        {
            get { return _newsRepo ?? (_newsRepo = new GenericRepository<News>(News)); }
        }

        public IGenericRepository<Role> RoleRepository
        {
            get { return _roleRepo ?? (_roleRepo = new GenericRepository<Role>(Roles)); }
        }

        public IGenericRepository<Section> SectionRepository
        {
            get { return _sectionRepo ?? (_sectionRepo = new GenericRepository<Section>(Sections)); }
        }

        public IGenericRepository<Tag> TagRepository
        {
            get { return _tagRepo ?? (_tagRepo = new GenericRepository<Tag>(Tags)); }
        }

        public IGenericRepository<User> UserRepository
        {
            get { return _userRepo ?? (_userRepo = new GenericRepository<User>(Users)); }
        }

        #endregion

        public void Commit()
        {
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        #region Dispose

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Code First Overrides

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer<UnitOfWork>(null);

            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Comments Table
            modelBuilder.Entity<Comment>().HasKey(c => c.Id);
            modelBuilder.Entity<Comment>()
                .Property(c => c.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Comment>().Property(c => c.Text).IsRequired();
            modelBuilder.Entity<Comment>().Property(c => c.PublishDate).IsRequired();
            modelBuilder.Entity<Comment>().Property(c => c.IsActive).IsRequired();
            modelBuilder.Entity<Comment>().HasRequired(c => c.File)
                .WithMany(f => f.Comments)
                .HasForeignKey(c => c.FileId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Comment>().HasOptional(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId)
                .WillCascadeOnDelete(false);

            // Downloads Table
            modelBuilder.Entity<Download>().HasKey(d => d.Id);
            modelBuilder.Entity<Download>()
                .Property(d => d.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Download>().Property(d => d.Date).IsRequired();
            modelBuilder.Entity<Download>().HasRequired(d => d.File)
                .WithMany(f => f.Downloads)
                .HasForeignKey(d => d.FileId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<Download>().HasOptional(d => d.User)
                .WithMany(u => u.Downloads)
                .HasForeignKey(d => d.UserId)
                .WillCascadeOnDelete(false);

            // Files Table
            modelBuilder.Entity<File>().HasKey(f => f.Id);
            modelBuilder.Entity<File>()
                .Property(f => f.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<File>().Property(f => f.Name).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<File>().Property(f => f.FullName).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<File>().Property(f => f.Description).IsOptional();
            modelBuilder.Entity<File>().Property(f => f.UploadDate).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.Size).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.Path).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<File>().Property(f => f.IsAllowedAnonymousBrowsing).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.IsAllowedAnonymousAction).IsRequired();
            modelBuilder.Entity<File>().HasRequired(f => f.Section)
                .WithMany(s => s.Files)
                .HasForeignKey(f => f.SectionId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<File>().HasRequired(f => f.Owner)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.OwnerId)
                .WillCascadeOnDelete(false);

            // Files&Tags Table
            modelBuilder.Entity<File>()
                .HasMany(f => f.Tags)
                .WithMany(t => t.Files)
                .Map(ft => ft.ToTable("Files&Tags")
                    .MapLeftKey("FileId")
                    .MapRightKey("TagId"));

            // Files&PermittedUsers Table
            modelBuilder.Entity<File>()
                .HasMany(f => f.AllowedUsers)
                .WithMany(u => u.FilesWithPermission)
                .Map(fu => fu.ToTable("Files&PermittedUsers")
                    .MapLeftKey("FileId")
                    .MapRightKey("UserId"));

            // Files&SubscribedUsers Table
            modelBuilder.Entity<File>()
                .HasMany(f => f.SubscribedUsers)
                .WithMany(u => u.FilesWithSubscription)
                .Map(fu => fu.ToTable("Files&SubscribedUsers")
                    .MapLeftKey("FileId")
                    .MapRightKey("UserId"));

            // News Table
            modelBuilder.Entity<News>().HasKey(n => n.Id);
            modelBuilder.Entity<News>()
                .Property(n => n.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<News>().Property(n => n.Name).IsRequired();
            modelBuilder.Entity<News>().Property(n => n.Text).IsRequired();
            modelBuilder.Entity<News>().Property(n => n.Picture).IsRequired().HasMaxLength(300);
            modelBuilder.Entity<News>().Property(n => n.PublishDate).IsRequired();
            modelBuilder.Entity<News>().Property(n => n.IsActive).IsRequired();
            modelBuilder.Entity<News>().HasRequired(n => n.Author)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.AuthorId)
                .WillCascadeOnDelete(false);

            // Roles Table
            modelBuilder.Entity<Role>().HasKey(r => r.Id);
            modelBuilder.Entity<Role>()
                .Property(r => r.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired().HasMaxLength(50);

            // Sections Table
            modelBuilder.Entity<Section>().HasKey(s => s.Id);
            modelBuilder.Entity<Section>()
                .Property(s => s.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Section>().Property(s => s.Name).IsRequired().HasMaxLength(50);

            // Tags Table
            modelBuilder.Entity<Tag>().HasKey(t => t.Id);
            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Tag>().Property(t => t.Name).IsRequired().HasMaxLength(50);

            // Users Table
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Email).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(70);
            modelBuilder.Entity<User>().Property(u => u.CreationDate).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.DownloadAmountLimit).IsRequired().HasPrecision(10, 2);
            modelBuilder.Entity<User>().Property(u => u.DownloadSpeedLimit).IsRequired().HasPrecision(10, 2);

            // Users&Roles Table
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(ur => ur.ToTable("Users&Roles")
                    .MapLeftKey("UserId")
                    .MapRightKey("RoleId"));

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}