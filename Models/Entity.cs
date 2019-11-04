namespace DataStorage.Models
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
    }

    public class Entity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}