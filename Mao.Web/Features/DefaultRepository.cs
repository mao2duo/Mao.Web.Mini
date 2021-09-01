using Mao.Repository;
using Mao.Web.Features.Options;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Mao.Web.Features
{
    public class DefaultRepository : RepositoryBase
    {
        public DefaultRepository(ConnectionStrings connectionStrings)
        {
            ConnectionString = connectionStrings.Default;
            Compiler = new SqlServerCompiler();
        }
        public override string ConnectionString { get; }
        public override IDbConnection CreateConnection() => new SqlConnection(ConnectionString);
        protected override Compiler Compiler { get; }
    }
}