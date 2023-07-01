namespace DataLayer.Models
{
    public class Ingredient : BaseModel
    {
        public int Id { get; }
        public string Name { get; set; }

        public IngredientThesaurus Thesaurus { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
    }
}
