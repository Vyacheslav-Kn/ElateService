using ElateService.DAL.Interfaces;

namespace ElateService.DAL.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private string _connectionString { get; set; }
        private ICustomerRepository _customerRepository { get; set; }
        private IExecutorRepository _executorRepository { get; set; }
        private IIndentRepository _indentRepository { get; set; }

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }


        public ICustomerRepository Customers {
            get
            {
                if(_customerRepository == null)
                {
                    _customerRepository = new CustomerRepository(_connectionString);
                }
                return _customerRepository;
            }
        }


        public IExecutorRepository Executors {
            get
            {
                if(_executorRepository == null)
                {
                    _executorRepository = new ExecutorRepository(_connectionString);
                }
                return _executorRepository;
            }
        }


        public IIndentRepository Indents
        {
            get
            {
                if(_indentRepository == null)
                {
                    _indentRepository = new IndentRepository(_connectionString);
                }
                return _indentRepository;
            }
        }

    }
}
