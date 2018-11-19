using Dapper;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using ElateService.DAL.PaginationEntity;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ElateService.DAL.Repositories
{
    public class ExecutorRepository : IExecutorRepository
    {
        private string _connectionString { get; set; }

        public ExecutorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<Executor> GetByEmail(string email)
        {
            string sqlQuery = "SELECT TOP 1 * FROM Executor WHERE Email = @Email AND EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Executor executor = await connection.QueryFirstOrDefaultAsync<Executor>(sqlQuery, new { Email = email });

                return executor;
            }
        }


        public async Task<int?> Create(Executor executor)
        {
            string sqlQuery = @"IF NOT EXISTS ( SELECT TOP 1 * FROM Executor WHERE Email=@Email )
        INSERT INTO Executor VALUES (@FirstName, @Surname, @Patronymic, @ConfirmationCode, @Email, @EmailConfirmed, @MobilePhone,
        @ImgSrc, @PasswordHash, @Salt, @Mark, @RoleId, @Information);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS LastInsertId";

            int? idOfInsertedExecutor;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                dynamic dapperRow = await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    executor.FirstName,
                    executor.Surname,
                    executor.Patronymic,
                    executor.ConfirmationCode,
                    executor.Email,
                    executor.EmailConfirmed,
                    executor.MobilePhone,
                    executor.ImgSrc,
                    executor.PasswordHash,
                    executor.Salt,
                    executor.Mark,
                    executor.RoleId,
                    executor.Information
                });

                idOfInsertedExecutor = dapperRow.LastInsertId;
            }

            return idOfInsertedExecutor;
        }


        public async Task Update(Executor executor)
        {
            string sqlQuery = @"UPDATE Executor 
            SET FirstName = @FirstName, Surname = @Surname, Patronymic = @Patronymic, ConfirmationCode = @ConfirmationCode, Email = @Email, 
                EmailConfirmed = @EmailConfirmed, MobilePhone = @MobilePhone, ImgSrc = @ImgSrc, PasswordHash = @PasswordHash, Salt = @Salt,
                Mark = @Mark, RoleId = @RoleId, Information = @Information WHERE ExecutorId = @ExecutorId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync(sqlQuery, new
                {
                    executor.ExecutorId,
                    executor.FirstName,
                    executor.Surname,
                    executor.Patronymic,
                    executor.ConfirmationCode,
                    executor.Email,
                    executor.EmailConfirmed,
                    executor.MobilePhone,
                    executor.ImgSrc,
                    executor.PasswordHash,
                    executor.Salt,
                    executor.Mark,
                    executor.RoleId,
                    executor.Information
                });
            }
        }


        public async Task<Executor> ConfirmRegistration(int id, string confirmationCode)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Executor WHERE ExecutorId=@Id AND 
                ConfirmationCode = @ConfirmationCode AND EmailConfirmed = 0 )
                BEGIN
                UPDATE Executor SET EmailConfirmed = 1 WHERE ExecutorId = @Id
                SELECT TOP 1 * FROM Executor WHERE ExecutorId = @Id
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Executor executor = await connection.QueryFirstOrDefaultAsync<Executor>(sqlQuery, new { Id = id,
                    ConfirmationCode = confirmationCode });

                return executor;
            }
        }


        public async Task<Executor> UpdateConfirmationCode(string email, string newConfirmationCode)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Executor WHERE Email=@Email AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Executor SET ConfirmationCode = @ConfirmationCode WHERE Email=@Email
                SELECT TOP 1 * FROM Executor WHERE Email=@Email
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Executor executor = await connection.QueryFirstOrDefaultAsync<Executor>(sqlQuery, new { Email = email,
                    ConfirmationCode = newConfirmationCode });

                return executor;
            }
        }


        public async Task<Executor> VerifyNewConfirmationCode(int id, string confirmationCode)
        {
            string sqlQuery = @"SELECT TOP 1 * FROM Executor WHERE ExecutorId = @Id AND ConfirmationCode = @ConfirmationCode AND
                                EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Executor executor = await connection.QueryFirstOrDefaultAsync<Executor>(sqlQuery, new { Id = id,
                    ConfirmationCode = confirmationCode });

                return executor;
            }
        }


        public async Task<Executor> UpdatePassword(int id, string NewSalt, string NewPassword)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Executor WHERE ExecutorId=@ExecutorId AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Executor SET Salt = @NewSalt, PasswordHash = @NewHashPassword WHERE ExecutorId=@ExecutorId
                SELECT TOP 1 * FROM Executor WHERE ExecutorId=@ExecutorId
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Executor executor = await connection.QueryFirstOrDefaultAsync<Executor>(sqlQuery,
                new { ExecutorId = id, NewSalt = NewSalt, NewHashPassword = NewPassword });

                return executor;
            }
        }


        public Executor GetById(int id)
        {
            string sqlQuery = @"SELECT * FROM Executor
                   LEFT JOIN CategoryExecutor ON CategoryExecutor.ExecutorId = Executor.ExecutorId 
                   WHERE Executor.ExecutorId = @Id AND Executor.EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var executorDictionary = new Dictionary<int, Executor>();

                connection.Query<Executor, CategoryExecutor, Executor>(sqlQuery, (Executor, CategoryExecutor) =>
                {
                    Executor executor;

                    if (!executorDictionary.TryGetValue(Executor.ExecutorId, out executor))
                    {
                        executor = Executor;
                        executor.Categories = new HashSet<CategoryExecutor>();
                        executorDictionary.Add(executor.ExecutorId, executor);
                    }

                    CategoryComparer comparerCategory = new CategoryComparer();
                    if (CategoryExecutor != null && !executor.Categories.Contains(CategoryExecutor, comparerCategory))
                    {
                        executor.Categories.Add(CategoryExecutor);
                    }

                    return executor;
                },
                    new { Id = id },
                    splitOn: "Id"
                  );

                Executor finalExecutor = new Executor();
                executorDictionary.TryGetValue(id, out finalExecutor);

                return finalExecutor;
            }
        }


        public async Task SaveExecutorPropertiesAfterEdition(string information, string imgSrc, List<int?> categories, int id)
        {
            string sqlQuery = null;
            int? firstCategory = null;
            int? secondCategory = null;

            if (imgSrc != null)
            {
                if(categories != null)
                {
                    sqlQuery = @"Update Executor SET ImgSrc=@ImgSrc, Information=@Information WHERE ExecutorId = @ExecutorId
                                INSERT INTO CategoryExecutor (CategoryId, ExecutorId) VALUES (@CategoryOne, @ExecutorId)
                                INSERT INTO CategoryExecutor (CategoryId, ExecutorId) VALUES (@CategoryTwo, @ExecutorId)";
                    firstCategory = categories[0];
                    secondCategory = categories[1];
                }
                else
                {
                    sqlQuery = @"Update Executor SET ImgSrc=@ImgSrc, Information=@Information WHERE ExecutorId = @ExecutorId";
                }
                
            }
            else
            {
                if (categories != null)
                {
                    sqlQuery = @"Update Executor SET Information=@Information WHERE ExecutorId = @ExecutorId
                                INSERT INTO CategoryExecutor (CategoryId, ExecutorId) VALUES (@CategoryOne, @ExecutorId)
                                INSERT INTO CategoryExecutor (CategoryId, ExecutorId) VALUES (@CategoryTwo, @ExecutorId)";
                    firstCategory = categories[0];
                    secondCategory = categories[1];
                }
                else
                {
                    sqlQuery = @"Update Executor SET Information=@Information WHERE ExecutorId = @ExecutorId";
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync(sqlQuery, new
                {
                    ExecutorId = id,
                    Information = information,
                    CategoryOne = firstCategory,
                    CategoryTwo = secondCategory,
                    ImgSrc = imgSrc
                });
            }
        }


        public Executor GetProfile(int id)
        {
            string sqlQuery = @"SELECT * FROM Executor
            LEFT JOIN Indent ON Indent.ExecutorId = Executor.ExecutorId 
			LEFT JOIN Recall ON Recall.RecallId = Indent.IndentId
			LEFT JOIN Customer ON Customer.CustomerId = Indent.CustomerId
            LEFT JOIN CategoryExecutor ON CategoryExecutor.ExecutorId = Executor.ExecutorId 
		    WHERE Executor.ExecutorId = @ExecutorId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var executors = new Dictionary<int, Executor>();

            connection.Query<Executor, Indent, Recall, Customer, CategoryExecutor, Executor>(sqlQuery, (Executor, Indent, Recall,
               Customer , Category) =>
                {
                    Executor executorEntry;
                    if (!executors.TryGetValue(Executor.ExecutorId, out executorEntry))
                    {
                        executorEntry = Executor;
                        executorEntry.Indents = new HashSet<Indent>();
                        executorEntry.Categories = new HashSet<CategoryExecutor>();
                        executors.Add(executorEntry.ExecutorId, executorEntry);
                    }

                    CategoryComparer comparerCategory = new CategoryComparer();
                    if (Category != null && !executorEntry.Categories.Contains(Category, comparerCategory))
                    {
                        executorEntry.Categories.Add(Category);
                    }

                    IndentComparer comparerIndent = new IndentComparer();
                    if (Indent != null && !executorEntry.Indents.Contains(Indent, comparerIndent))
                    {
                        if (Customer != null)
                        {
                            Indent.Customer = new Customer();
                            Indent.Customer = Customer;
                        }
                        if (Recall != null)
                        {
                            if(Recall.CustomerCommentForExecutor != null)
                            {
                                Indent.Recall = new Recall();
                                Indent.Recall = Recall;
                            }
                        }
                        executorEntry.Indents.Add(Indent);
                    }

                    return executorEntry;
                },
                   new { ExecutorId = id },
                   splitOn: "ExecutorId, IndentId, RecallId, CustomerId, Id"
                   ).Distinct();

                Executor executor = new Executor();
                executor = executors.FirstOrDefault().Value;

                return executor;
            }
        }


        public async Task<ExecutorPage> GetExecutorsPerPage(int page, int pageSize, List<int> categories)
        {
            string sqlQuery = @" 
                              SELECT COUNT(DISTINCT Executor.ExecutorId) AS Number FROM Executor 
                              JOIN CategoryExecutor ON CategoryExecutor.ExecutorId = Executor.ExecutorId
                              WHERE CategoryExecutor.CategoryId IN @CategoryList;
                              
                              SELECT DISTINCT Executor.* FROM Executor
                              JOIN CategoryExecutor ON CategoryExecutor.ExecutorId = Executor.ExecutorId
                              WHERE CategoryExecutor.CategoryId IN @CategoryList
                              ORDER BY Executor.ExecutorId DESC
                              OFFSET @Page*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";

            if (categories == null)
            {
                categories = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                IEnumerable<Executor> executors = new List<Executor>(); 
                int numberOfAllExecutors;

                using (var multi = await connection.QueryMultipleAsync(sqlQuery, new
                {
                    Page = page,
                    PageSize = pageSize,
                    CategoryList = categories.ToArray<int>()
                }))
                {
                    var numberOfExecutors = await multi.ReadAsync<int>();
                    numberOfAllExecutors = numberOfExecutors.Single();

                    executors = await multi.ReadAsync<Executor>();
                }

                ExecutorPage executorsSinglePageModel = new ExecutorPage()
                {
                    ExecutorsOnPage = executors,
                    NumberOfAllExecutorsWithSomeCategory = numberOfAllExecutors
                };

                return executorsSinglePageModel;
            }
        }


        public async Task<IEnumerable<Executor>> SearchInNames(string searchString)
        {
            string sqlQuery = @" SELECT * FROM Executor                                  
                                 JOIN CategoryExecutor ON CategoryExecutor.ExecutorId = Executor.ExecutorId
                                 WHERE Executor.FirstName = @Name OR Executor.Surname = @Name OR Executor.Patronymic = @Name";

            Dictionary<int, Executor> executors = new Dictionary<int, Executor>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryAsync<Executor, CategoryExecutor, Executor> (sqlQuery, (Executor, CategoryExecutor) =>
                {
                    Executor executor;

                    if (!executors.TryGetValue(Executor.ExecutorId, out executor))
                    {
                        executor = Executor;
                        executor.Categories = new HashSet<CategoryExecutor>();
                        executors.Add(executor.ExecutorId, executor);
                    }

                    CategoryComparer comparerCategory = new CategoryComparer();
                    if (CategoryExecutor != null && !executor.Categories.Contains(CategoryExecutor, comparerCategory))
                    {
                        executor.Categories.Add(CategoryExecutor);
                    }
                    return executor;
                },
                new { Name = searchString },
                splitOn: "ExecutorId, Id");
            }
            return executors.Values.ToList();
        }

    }
}
