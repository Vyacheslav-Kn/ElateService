namespace ElateService.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customers { get; }
        IExecutorRepository Executors { get; }
        IIndentRepository Indents { get; }
       // IResponceRepository Responces { get; }
    }
}
