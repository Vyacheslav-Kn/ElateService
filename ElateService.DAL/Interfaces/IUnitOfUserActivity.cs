namespace ElateService.DAL.Interfaces
{
    public interface IUnitOfUserActivity
    {
        IIndentRepository Indents { get; }
        IResponceRepository Responces { get; }
        IRecallRepository Recalls { get; }
        INotificationRepository Notifications { get; }
    }
}
