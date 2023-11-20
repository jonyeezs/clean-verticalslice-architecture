using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class RecipeContext : DbContext
    {
        public DbSet<Ingredient> Ingredient { get; set; }

        public DbSet<IngredientThesaurus> IngredientThesaurus { get; set; }
        public DbSet<Recipe> Recipe { get; set; }

        public RecipeContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ingredient>().HasKey(i => i.Id);
            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Thesaurus)
                .WithOne(t => t.Ingredient)
                .HasForeignKey<IngredientThesaurus>("IngredientId");

            modelBuilder.Entity<Recipe>().HasKey(r => r.Id);

            // Building thesaurus
            modelBuilder.Entity<IngredientThesaurus>()
                .HasMany(ti => ti.Synonyms)
                .WithMany(ti => ti.SynonymOf)
                .UsingEntity(j => j.ToTable("IngredientSynonyms")).HasKey(i => i.Id);
        }

        public override int SaveChanges()
        {
            UpdateEntity();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            UpdateEntity();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntity()
        {
            var addedEntities = ChangeTracker.Entries<BaseModel>()
                .Where(e => e.State == EntityState.Added);

            var modifiedEntities = ChangeTracker.Entries<BaseModel>()
                .Where(e => e.State == EntityState.Modified);

            DateTimeOffset currentDate = DateTimeOffset.UtcNow;

            foreach (var addedEntity in addedEntities)
            {
                addedEntity.Entity.CreatedDate = currentDate;
                addedEntity.Entity.ModifiedDate = currentDate;
            }

            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.Entity.ModifiedDate = currentDate;
            }
        }
    }
}
