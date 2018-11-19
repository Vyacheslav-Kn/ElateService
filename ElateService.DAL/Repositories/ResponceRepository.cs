using Dapper;
using ElateService.DAL.Entities;
using ElateService.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElateService.DAL.Repositories
{
    public class ResponceRepository:IResponceRepository
    {
        private string _connectionString { get; set; }

        public ResponceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task Create(Responce responce)
        {
            string sqlQuery = @"INSERT INTO Responce VALUES (@ResponceText, @Price, @ExecutorId, @IndentId)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await connection.QueryFirstOrDefaultAsync(sqlQuery, new
                {
                    responce.ResponceText,
                    responce.Price,
                    responce.Executor.ExecutorId,
                    responce.Indent.IndentId
                });
            }
        }


    }
}
