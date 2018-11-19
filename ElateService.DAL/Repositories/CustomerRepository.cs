using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElateService.DAL.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private string _connectionString { get; set; }

        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<Customer> GetByEmail(string email)
        {
            string sqlQuery = "SELECT TOP 1 * FROM Customer WHERE Email = @Email AND EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, new { Email = email });

                return customer;
            }
        }


        public async Task<int?> Create(Customer customer)
        {
            string sqlQuery = @"IF NOT EXISTS ( SELECT TOP 1 * FROM Customer WHERE Email=@Email )
        INSERT INTO Customer VALUES (@FirstName, @Surname, @Patronymic, @ConfirmationCode, @Email, @EmailConfirmed, @MobilePhone,
        @ImgSrc, @PasswordHash, @Salt, @Mark, @RoleId, @Information);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS LastInsertId";

            int? idOfInsertedCustomer;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                dynamic dapperRow = await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    customer.FirstName,
                    customer.Surname,
                    customer.Patronymic,
                    customer.ConfirmationCode,
                    customer.Email,
                    customer.EmailConfirmed,
                    customer.MobilePhone,
                    customer.ImgSrc,
                    customer.PasswordHash,
                    customer.Salt,
                    customer.Mark,
                    customer.RoleId,
                    customer.Information
                });

                idOfInsertedCustomer = dapperRow.LastInsertId;
            }

            return idOfInsertedCustomer;
        }


        public async Task Update(Customer customer)
        {
            string sqlQuery = @"UPDATE Customer 
            SET FirstName = @FirstName, Surname = @Surname, Patronymic = @Patronymic, ConfirmationCode = @ConfirmationCode, Email = @Email, 
                EmailConfirmed = @EmailConfirmed, MobilePhone = @MobilePhone, ImgSrc = @ImgSrc, PasswordHash = @PasswordHash, Salt = @Salt,
                Mark = @Mark, RoleId = @RoleId, Information = @Information WHERE CustomerId = @CustomerId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync(sqlQuery, new
                {
                    customer.CustomerId,
                    customer.FirstName,
                    customer.Surname,
                    customer.Patronymic,
                    customer.ConfirmationCode,
                    customer.Email,
                    customer.EmailConfirmed,
                    customer.MobilePhone,
                    customer.ImgSrc,
                    customer.PasswordHash,
                    customer.Salt,
                    customer.Mark,
                    customer.RoleId,
                    customer.Information
                });
            }
        }


        public async Task<Customer> ConfirmRegistration(int id, string confirmationCode)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Customer WHERE CustomerId=@Id AND 
                ConfirmationCode = @ConfirmationCode AND EmailConfirmed = 0 )
                BEGIN
                UPDATE Customer SET EmailConfirmed = 1 WHERE CustomerId = @Id
                SELECT TOP 1 * FROM Customer WHERE CustomerId = @Id
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, new { Id = id,
                    ConfirmationCode = confirmationCode });
                
                return customer;
            }
        }


        public async Task<Customer> UpdateConfirmationCode(string email, string newConfirmationCode)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Customer WHERE Email=@Email AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Customer SET ConfirmationCode = @ConfirmationCode WHERE Email=@Email
                SELECT TOP 1 * FROM Customer WHERE Email=@Email
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, new {Email = email,
                    ConfirmationCode = newConfirmationCode});

                return customer;             
            }
        }


        public async Task<Customer> VerifyNewConfirmationCode(int id, string confirmationCode)
        {
            string sqlQuery = @"SELECT TOP 1 * FROM Customer WHERE CustomerId = @Id AND ConfirmationCode = @ConfirmationCode AND
                                EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, new { Id = id,
                    ConfirmationCode = confirmationCode });
                
                return customer;
            }
        }


        public async Task<Customer> UpdatePassword(int id, string NewSalt, string NewPassword)
        {
            string sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Customer WHERE CustomerId=@CustomerId AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Customer SET Salt = @NewSalt, PasswordHash = @NewHashPassword WHERE CustomerId=@CustomerId
                SELECT TOP 1 * FROM Customer WHERE CustomerId=@CustomerId
                END";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, 
                new { CustomerId = id, NewSalt = NewSalt, NewHashPassword = NewPassword });

                return customer;
            }
        }


        public async Task<Customer> GetById(int id)
        {
            string sqlQuery = "SELECT TOP 1 * FROM Customer WHERE CustomerId = @Id AND EmailConfirmed = 1";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                Customer customer = await connection.QueryFirstOrDefaultAsync<Customer>(sqlQuery, new { Id = id });

                return customer;
            }
        }


        public async Task SaveCustomerPropertiesAfterEdition(string information, string imgSrc, int id)
        {
            string sqlQuery = null;

            if(imgSrc != null)
            {
                sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Customer WHERE CustomerId=@CustomerId AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Customer SET Information = @Information, ImgSrc = @ImgSrc WHERE CustomerId = @CustomerId
                END";
            }
            else
            {
                sqlQuery = @"IF EXISTS ( SELECT TOP 1 * FROM Customer WHERE CustomerId=@CustomerId AND EmailConfirmed = 1 )
                BEGIN
                UPDATE Customer SET Information = @Information WHERE CustomerId = @CustomerId
                END";
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync(sqlQuery, new {
                    CustomerId = id,
                    Information = information,
                    ImgSrc = imgSrc
                });                
            }
        }


        public Customer GetProfile(int id)
        {
            string sqlQuery = @"SELECT * FROM Customer
            LEFT JOIN Indent ON Indent.CustomerId = Customer.CustomerId 
			LEFT JOIN Recall ON Recall.RecallId = Indent.IndentId
			LEFT JOIN Executor ON Executor.ExecutorId = Indent.ExecutorId
		    WHERE Customer.CustomerId = @CustomerId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var customers = new Dictionary<int, Customer>();

                connection.Query<Customer,Indent, Recall, Executor, Customer>(sqlQuery, (Customer, Indent, Recall, Executor) =>
                {
                    Customer customerEntry;
                    if (!customers.TryGetValue(Customer.CustomerId, out customerEntry))
                    {
                        customerEntry = Customer;
                        customerEntry.Indents = new HashSet<Indent>();
                        customers.Add(customerEntry.CustomerId, customerEntry);
                    }

                    IndentComparer comparerIndent = new IndentComparer();
                    if (Indent != null && !customerEntry.Indents.Contains(Indent, comparerIndent))
                    {
                        if (Executor != null)
                        {
                            Indent.Executor = new Executor();
                            Indent.Executor = Executor;
                        }
                        if (Recall != null)
                        {
                            if (Recall.ExecutorCommentForCustomer != null)
                            {
                                Indent.Recall = new Recall();
                                Indent.Recall = Recall;
                            }
                        }
                        customerEntry.Indents.Add(Indent);
                    }

                    return customerEntry;
                },
                   new { CustomerId = id },
                   splitOn: "CustomerId, IndentId, RecallId, ExecutorId"
                   ).Distinct();

                Customer customer = new Customer();
                customer = customers.FirstOrDefault().Value;

                return customer;
            }
        }

    }
}
