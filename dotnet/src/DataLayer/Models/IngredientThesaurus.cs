namespace DataLayer.Models
{
    /// <summary>
    /// A thesaurus of ingredients with similar names
    /// </summary>
    public class IngredientThesaurus : BaseModel
    {
        public int Id { get; set; }
        public Ingredient Ingredient { get; set; }

        public ICollection<IngredientThesaurus> Synonyms { get; set; }
        public ICollection<IngredientThesaurus> SynonymOf { get; set; }
    }
}
