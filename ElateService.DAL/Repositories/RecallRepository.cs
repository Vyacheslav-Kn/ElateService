using Dapper;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ElateService.DAL.Repositories
{
    public class RecallRepository:IRecallRepository
    {
        private string _connectionString { get; set; }

        public RecallRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task AddExecutorRecallPropertiesForCustomer(Recall recall)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Recall WHERE RecallId = @RecallId )
                             BEGIN 
                             UPDATE Recall SET ExecutorCommentForCustomer = @ExecutorCommentForCustomer, 
                                ExecutorMarkForCustomer = @ExecutorMarkForCustomer WHERE RecallId = @RecallId;
                             DECLARE @CustId INT;
                             SET @CustId = (SELECT TOP 1 (CustomerId) FROM Indent WHERE IndentId = @RecallId);
                             DECLARE @CustNewMark INT;
                 SET @CustNewMark = (SELECT (SUM(Recall.ExecutorMarkForCustomer) / COUNT(Recall.ExecutorMarkForCustomer)) 
			                 FROM Recall 
			                 JOIN Indent ON Indent.IndentId = Recall.RecallId  WHERE Indent.CustomerId = @CustId);
                             UPDATE Customer SET Mark=@CustNewMark WHERE CustomerId = @CustId 
                             END

                             ELSE
                             BEGIN
                             INSERT INTO Recall VALUES (@RecallId, @ExecutorCommentForCustomer, @ExecutorMarkForCustomer,
                                @CustomerCommentForExecutor, @CustomerMarkForExecutor);
                             DECLARE @CCustId INT;
                             SET @CCustId = (SELECT TOP 1 (CustomerId) FROM Indent WHERE IndentId = @RecallId);
                             DECLARE @CCustNewMark INT;
                 SET @CCustNewMark = (SELECT (SUM(Recall.ExecutorMarkForCustomer) / COUNT(Recall.ExecutorMarkForCustomer)) 
			                 FROM Recall 
			                 JOIN Indent ON Indent.IndentId = Recall.RecallId  WHERE Indent.CustomerId = @CCustId);
                             UPDATE Customer SET Mark=@CCustNewMark WHERE CustomerId = @CCustId 
                             END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    RecallId = recall.RecallId,
                    ExecutorCommentForCustomer = recall.ExecutorCommentForCustomer,
                    ExecutorMarkForCustomer = recall.ExecutorMarkForCustomer,
                    CustomerCommentForExecutor = recall.CustomerCommentForExecutor,
                    CustomerMarkForExecutor = recall.CustomerMarkForExecutor
                });
            }
        }


        public async Task AddCustomerRecallPropertiesForExecutor(Recall recall)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Recall WHERE RecallId = @RecallId )
                             BEGIN
                             UPDATE Recall SET CustomerCommentForExecutor = @CustomerCommentForExecutor, 
                                CustomerMarkForExecutor = @CustomerMarkForExecutor WHERE RecallId = @RecallId;
                             DECLARE @ExId INT;
                             SET @ExId = (SELECT TOP 1 (ExecutorId) FROM Indent WHERE IndentId = @RecallId);
                             DECLARE @ExNewMark INT;
                 SET @ExNewMark = (SELECT (SUM(Recall.CustomerMarkForExecutor) / COUNT(Recall.CustomerMarkForExecutor)) 
			                 FROM Recall 
			                 JOIN Indent ON Indent.IndentId = Recall.RecallId  WHERE Indent.ExecutorId = @ExId);
                             UPDATE Executor SET Mark=@ExNewMark WHERE ExecutorId = @ExId
                             END

                             ELSE
                             BEGIN
                             INSERT INTO Recall VALUES (@RecallId, @ExecutorCommentForCustomer, @ExecutorMarkForCustomer,
                                @CustomerCommentForExecutor, @CustomerMarkForExecutor);
                             DECLARE @EExId INT;
                             SET @EExId = (SELECT TOP 1 (ExecutorId) FROM Indent WHERE IndentId = @RecallId);
                             DECLARE @EExNewMark INT;
                 SET @EExNewMark = (SELECT (SUM(Recall.CustomerMarkForExecutor) / COUNT(Recall.CustomerMarkForExecutor)) 
			                 FROM Recall 
			                 JOIN Indent ON Indent.IndentId = Recall.RecallId  WHERE Indent.ExecutorId = @EExId);
                             UPDATE Executor SET Mark=@EExNewMark WHERE ExecutorId = @EExId
                             END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    RecallId = recall.RecallId,
                    ExecutorCommentForCustomer = recall.ExecutorCommentForCustomer,
                    ExecutorMarkForCustomer = recall.ExecutorMarkForCustomer,
                    CustomerCommentForExecutor = recall.CustomerCommentForExecutor,
                    CustomerMarkForExecutor = recall.CustomerMarkForExecutor
                });
            }
        }


    }
}
