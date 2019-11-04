namespace DataStorage.Models
{
    public interface IEntity<I>
    {
         I Id { get; set; }
    }

    public interface IEntity
    {
        int Id { get; set; }
    }
}