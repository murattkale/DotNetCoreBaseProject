using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public partial class myDBContext : DbContext
{
    public myDBContext()
    {
    }

    public myDBContext(DbContextOptions<myDBContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Spec> Spec { get; set; }
    public virtual DbSet<SpecAttr> SpecAttr { get; set; }
    public virtual DbSet<SpecContentType> SpecContentType { get; set; }
    public virtual DbSet<SpecContentValue> SpecContentValue { get; set; }
    public virtual DbSet<Forms> Forms { get; set; }
    public virtual DbSet<FormDocument> FormDocument { get; set; }
    public virtual DbSet<FormType> FormType { get; set; }
    public virtual DbSet<ContentPage> ContentPage { get; set; }
    public virtual DbSet<Documents> Documents { get; set; }
    public virtual DbSet<SiteConfig> SiteConfig { get; set; }
    public virtual DbSet<Lang> Lang { get; set; }
    public virtual DbSet<LangDisplay> LangDisplay { get; set; }

    public virtual DbSet<Role> Role { get; set; }
    public virtual DbSet<UserRole> UserRole { get; set; }
    public virtual DbSet<Permission> Permission { get; set; }
    public virtual DbSet<ServiceConfigAuth> ServiceConfigAuth { get; set; }
    public virtual DbSet<ServiceConfig> ServiceConfig { get; set; }


    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Nationality> Nationality { get; set; }
    public virtual DbSet<UserAdress> UserAdress { get; set; }
    public virtual DbSet<Country> Country { get; set; }
    public virtual DbSet<City> City { get; set; }
    public virtual DbSet<District> District { get; set; }


    public virtual DbSet<Product> Product { get; set; }
    public virtual DbSet<Order> Order { get; set; }
    public virtual DbSet<OrderDetail> OrderDetail { get; set; }
    public virtual DbSet<Coupon> Coupon { get; set; }


    // banks
    public virtual DbSet<Bank> Banks { get; set; }
    public virtual DbSet<BankParameter> BankParameters { get; set; }
    public virtual DbSet<CreditCard> CreditCards { get; set; }
    public virtual DbSet<CreditCardPrefix> CreditCardPrefixes { get; set; }
    public virtual DbSet<CreditCardInstallment> CreditCardInstallments { get; set; }

    // payments
    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<BankPrefix> BankPrefix { get; set; }



    public virtual DbSet<Test> Test { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            ////test
            //optionsBuilder.UseSqlServer("server=ip;Database=db;User Id=user;Password=pass;", x => x.MigrationsHistoryTable("__EFMigrationsHistory", "mySchema"));

            ////prod
            //optionsBuilder.UseSqlServer("server=ip;Database=dbtest;User Id=usertest;Password=pass;", x => x.MigrationsHistoryTable("__EFMigrationsHistory", "mySchema"));
        }
    }

    private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(myDBContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureGlobalFiltersMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }



        modelBuilder.Entity<ContentPage>().HasOne(a => a.ThumbImage).WithOne(b => b.ThumbImage).HasForeignKey<Documents>(b => b.ThumbImageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ContentPage>().HasOne(a => a.Picture).WithOne(b => b.Picture).HasForeignKey<Documents>(b => b.PictureId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ContentPage>().HasOne(a => a.BannerImage).WithOne(b => b.BannerImage).HasForeignKey<Documents>(b => b.BannerImageId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.Gallery).WithOne(b => b.Gallery).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ContentPage>().HasMany(a => a.Documents).WithOne(b => b.Document).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ContentPage>().HasMany(a => a.Childs).WithOne(b => b.Parent).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ContentPage>().HasMany(a => a.SpecContentValue).WithOne(b => b.ContentPage).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Spec>().HasMany(a => a.SpecChilds).WithOne(b => b.Parent).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Spec>().HasMany(a => a.SpecAttrs).WithOne(b => b.Spec).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Spec>().HasMany(a => a.SpecContentValue).WithOne(b => b.Spec).OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<Product>().HasMany(a => a.MainProducts).WithOne(b => b.MainProduct).OnDelete(DeleteBehavior.Cascade);



        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetForeignKeys())
        .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        base.OnModelCreating(modelBuilder);
    }

    protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType) where TEntity : class
    {
        if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>(entityType)) return;
        var filterExpression = CreateFilterExpression<TEntity>();
        if (filterExpression == null) return;
        //if (entityType.GetType().IsInterface==true)
        //if (false)
        //    modelBuilder.Query<TEntity>().HasQueryFilter(filterExpression);
        //else
        modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
    }

    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        return typeof(IBaseModel).IsAssignableFrom(typeof(TEntity));
    }

    protected Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>() where TEntity : class
    {
        Expression<Func<TEntity, bool>> expression = null;

        if (typeof(IBaseModel).IsAssignableFrom(typeof(TEntity)))
        {
            Expression<Func<TEntity, bool>> removedFilter = e => (DateTime)((IBaseModel)e).IsDeleted == null;
            expression = expression == null ? removedFilter : CombineExpressions(expression, removedFilter);
        }

        return expression;
    }

    protected Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        return Helpers.Combine(expression1, expression2);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

