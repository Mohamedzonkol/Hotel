namespace Hotel.Application.Common.InterFaces
{
    public interface IUnitOfWork
    {
        IVillaRepository VillaRepository { get; }
        IVillaNumberRepository VillaNumberRepository { get; }
        IAmenityRepository AmenityRepository { get; }
    }
}
