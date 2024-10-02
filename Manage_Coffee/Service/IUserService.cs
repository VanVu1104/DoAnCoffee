namespace Manage_Coffee.Service
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}