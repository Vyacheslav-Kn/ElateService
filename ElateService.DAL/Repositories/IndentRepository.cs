using Dapper;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using ElateService.DAL.PaginationEntity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.DAL.Repositories
{
    public class IndentRepository: IIndentRepository
    {
        private string _connectionString { get; set; }

        public IndentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<int?> Create(Indent indent)
        {
            string sqlQuery = @"INSERT INTO Indent VALUES (@Title, @IndentDescription, @City, @IndentDate,
            @Price, @CustomerId, @CategoryId, @ExecutorId, @ImgSrc);
            DECLARE @LastInsertId INT;
            SELECT @LastInsertId = SCOPE_IDENTITY()
            UPDATE Indent
            SET ImgSrc = REPLACE(ImgSrc, 'indentId', @LastInsertId)
            WHERE IndentId = @LastInsertId
            SELECT @LastInsertId AS IndentId";

            int? idOfInsertedIndent;
            int? ExecutorId = null;

            if (indent.Executor != null)
            {
                ExecutorId = indent.Executor.ExecutorId;
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                dynamic dapperRow = await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    indent.Title,
                    indent.IndentDescription,
                    indent.City,
                    indent.IndentDate,
                    indent.Price,
                    indent.Customer.CustomerId,
                    indent.CategoryId,
                    ExecutorId,
                    indent.ImgSrc
                });

                idOfInsertedIndent = dapperRow.IndentId;
            }

            return idOfInsertedIndent;
        }


        public Indent GetById(int id)
        {
            string sqlQuery = @"SELECT * FROM Indent 
            JOIN Customer ON Customer.CustomerId = Indent.CustomerId 
            LEFT JOIN Executor AS A ON A.ExecutorId = Indent.ExecutorId
            LEFT JOIN Responce ON Responce.IndentId = Indent.IndentId
            LEFT JOIN Recall ON Recall.RecallId = Indent.IndentId
			LEFT JOIN Executor AS B ON B.ExecutorId = Responce.ExecutorId
		    WHERE Indent.IndentId = @IndentId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var indents = new Dictionary<int, Indent>();

             connection.Query<Indent, Customer, Executor, Responce, Recall, Executor, Indent>(sqlQuery, (Indent, Customer, ExecutorForIndent,
                 Responce, Recall, ExecutorForResponce) =>
                {
                    Indent indentEntry;
                    if (!indents.TryGetValue(Indent.IndentId, out indentEntry))
                    {
                        indentEntry = Indent;
                        indentEntry.Customer = new Customer();
                        indentEntry.Executor = new Executor();
                        indentEntry.Responces = new HashSet<Responce>();
                        indentEntry.Recall = new Recall();
                        indents.Add(indentEntry.IndentId, indentEntry);
                    }

                    indentEntry.Customer = Customer;
                    indentEntry.Executor = ExecutorForIndent;
                    indentEntry.Recall = Recall;

                    ResponceComparer comparerResponce = new ResponceComparer();
                    if (Responce != null && !indentEntry.Responces.Contains(Responce, comparerResponce))
                    {
                        Responce.Executor = ExecutorForResponce;
                        indentEntry.Responces.Add(Responce);
                    }

                    return indentEntry;
                }, 
                new { IndentId = id },
                splitOn: "CustomerId, ExecutorId, ResponceId, RecallId, ExecutorId"
                ).Distinct();

                Indent indent = new Indent();
                indent = indents.FirstOrDefault().Value;

                return indent;
            }
        }


        public async Task SetExecutorId(Indent indent)
        {
            string sqlQuery = @"UPDATE Indent SET ExecutorId = @ExecutorId WHERE IndentId = @IndentId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    IndentId = indent.IndentId,
                    ExecutorId = indent.Executor.ExecutorId
                });
            }
        }


        public async Task<IEnumerable<Indent>> GetIndentsByCustomerId(int id)
        {
            string sqlQuery = @" SELECT * FROM Indent                                  
                                 LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId 
                                 WHERE CustomerId = @CustomerId ORDER BY IndentId DESC";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                IEnumerable<Indent> indents = await connection.QueryAsync<Indent, Executor, Indent>(sqlQuery, (Indent, Executor) =>
                {
                    Indent.Executor = new Executor();
                    Indent.Executor = Executor;
                    return Indent;
                },
                new { CustomerId = id },
                splitOn: "IndentId, ExecutorId");

                return indents;
            }
        }


        public async Task<IEnumerable<Indent>> GetIndentsWithExecutorResponce(int executorId)
        {
            string sqlQuery = @" SELECT * FROM Indent            
                                 LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId                       
                                 JOIN Responce ON Responce.IndentId = Indent.IndentId 
                                 WHERE Responce.ExecutorId = @ExecutorId ORDER BY ResponceId DESC";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var indents = new Dictionary<int, Indent>();
                 
                await connection.QueryAsync<Indent, Executor, Responce, Indent>(sqlQuery, (Indent, Executor, Responce) =>
                {
                    Indent indentEntry;
                    if (!indents.TryGetValue(Indent.IndentId, out indentEntry))
                    {
                        indentEntry = Indent;
                        indentEntry.Executor = new Executor();
                        indentEntry.Responces = new HashSet<Responce>();
                        indents.Add(indentEntry.IndentId, indentEntry);
                    }

                    indentEntry.Executor = Executor;

                    ResponceComparer comparerResponce = new ResponceComparer();
                    if (Responce != null && !indentEntry.Responces.Contains(Responce, comparerResponce))
                    {
                        indentEntry.Responces.Add(Responce);
                    }

                    return indentEntry;
                },
                new { ExecutorId = executorId },
                splitOn: "IndentId, ExecutorId, ResponceId");

                return indents.Values;
            }
        }


        public async Task<IndentPage> GetIndentsPerPage(int page, int pageSize, List<int> categories)
        {
            string sqlQuery = @" 
                              DECLARE @NumberOfIndents INT;
                              SET @NumberOfIndents = (
                              SELECT Count(Indent.IndentId) FROM Indent
                              LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId
                              WHERE CategoryId IN @CategoryList
                              )
                              SELECT @NumberOfIndents AS Number;
                              
                              SELECT * FROM Indent                                  
                              LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId 
                              WHERE CategoryId IN @CategoryList
                              ORDER BY IndentId DESC
                              OFFSET @Page*@PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;";

            if (categories == null)
            {
                categories = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                IEnumerable<Indent> indents = new List<Indent>();
                int numberOfAllIndents;

                using (var multi = await connection.QueryMultipleAsync(sqlQuery, new
                {
                    Page = page,
                    PageSize = pageSize,
                    CategoryList = categories.ToArray<int>()
                }))
                {
                    var numberOfIndents = await multi.ReadAsync<int>();

                    indents = multi.Read<Indent, Executor, Indent>((Indent, Executor) =>
                    {
                        Indent.Executor = new Executor();
                        Indent.Executor = Executor;
                        return Indent;
                    },
                    splitOn: "IndentId, ExecutorId");

                    numberOfAllIndents = numberOfIndents.Single();
                }

                IndentPage indentsSinglePageModel = new IndentPage()
                {
                    IndentsOnPage = indents,
                    NumberOfAllIndentsWithSomeCategory = numberOfAllIndents
                };
                        
                return indentsSinglePageModel;
            }
        }


        public async Task<IEnumerable<Indent>> SearchByTitle(string title)
        {
            string sqlQuery = @" SELECT * FROM Indent                                  
                                 LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId 
                                 WHERE Title = @Title ORDER BY IndentId DESC";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                IEnumerable<Indent> indents = await connection.QueryAsync<Indent, Executor, Indent>(sqlQuery, (Indent, Executor) =>
                {
                    Indent.Executor = new Executor();
                    Indent.Executor = Executor;
                    return Indent;
                },
                new { Title = title },
                splitOn: "IndentId, ExecutorId");

                return indents;
            }
        }


        public async Task<IEnumerable<Indent>> GetFreeCustomerIndentsForExecutor(int customerId, int executorId)
        {
            string sqlQuery = @" SELECT * FROM Indent WHERE 
	                             Indent.CustomerId = @CustomerId AND Indent.ExecutorId IS NULL AND Indent.CategoryId IN
	                             (SELECT CategoryId FROM CategoryExecutor WHERE CategoryExecutor.ExecutorId = @ExecutorId)";

            IEnumerable<Indent> indents = new List<Indent>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                indents = await connection.QueryAsync<Indent>(sqlQuery, new { CustomerId = customerId, ExecutorId = executorId });
            }

            return indents;
        }

    }
}
