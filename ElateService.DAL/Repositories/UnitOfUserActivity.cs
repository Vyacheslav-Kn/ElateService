using ElateService.DAL.Interfaces;

namespace ElateService.DAL.Repositories
{
    public class UnitOfUserActivity: IUnitOfUserActivity
    {
        private string _connectionString { get; set; }
        private IIndentRepository _indentRepository { get; set; }
        private IResponceRepository _responceRepository { get; set; }
        private IRecallRepository _recallRepository { get; set; }
        private INotificationRepository _notificationRepository { get; set; }

        public UnitOfUserActivity(string connectionString)
        {
            _connectionString = connectionString;
        }


        public IIndentRepository Indents
        {
            get
            {
                if (_indentRepository == null)
                {
                    _indentRepository = new IndentRepository(_connectionString);
                }
                return _indentRepository;
            }
        }


        public IResponceRepository Responces
        {
            get
            {
                if (_responceRepository == null)
                {
                    _responceRepository = new ResponceRepository(_connectionString);
                }
                return _responceRepository;
            }
        }


        public IRecallRepository Recalls
        {
            get
            {
                if (_recallRepository == null)
                {
                    _recallRepository = new RecallRepository(_connectionString);
                }
                return _recallRepository;
            }
        }


        public INotificationRepository Notifications
        {
            get
            {
                if (_notificationRepository == null)
                {
                    _notificationRepository = new NotificationRepository(_connectionString);
                }
                return _notificationRepository;
            }
        }

    }
}
