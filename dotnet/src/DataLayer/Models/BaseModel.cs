namespace DataLayer.Models
{
    public abstract class BaseModel
    {
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
