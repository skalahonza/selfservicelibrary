namespace SelfServiceLibrary.BL.Exceptions
{
    public class EntityNotFoundException<TEntity> : BusinessLayerException
    {
        public EntityNotFoundException(string id)
            : base($"Entity {typeof(TEntity).Name} with id {id} was not found.")
        {
        }
    }
}
