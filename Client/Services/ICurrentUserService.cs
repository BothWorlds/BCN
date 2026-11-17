namespace BCN.Services
{
    public interface ICurrentUserService
    {
        string UserName { get; }

        int? UserId { get; }
    }
}