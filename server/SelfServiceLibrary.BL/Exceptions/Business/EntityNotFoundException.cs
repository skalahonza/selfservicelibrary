namespace SelfServiceLibrary.BL.Exceptions.Business
{
    public class EntityNotFoundException<TEntity> : BusinessLayerException
    {
        public EntityNotFoundException(string id)
            : base($"Entity {typeof(TEntity).Name} with id {id} was not found.")
        {
        }
    }
}
