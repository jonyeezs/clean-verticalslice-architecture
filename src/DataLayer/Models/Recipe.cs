namespace DataLayer.Models
{
    public class Recipe : BaseModel
    {
        public Guid Id { get; }
        public string Title { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }
    }
}
