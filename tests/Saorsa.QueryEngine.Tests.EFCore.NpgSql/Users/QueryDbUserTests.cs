namespace Saorsa.QueryEngine.Tests.EFCore.NpgSql.Users;


public class QueryDbUserTests: Saorsa.QueryEngine.Tests.EFCore.Common.QueryDbUserTests
{
    protected override QueryDbContext GetQueryDbContext()
    {
        return new QueryNpgsqlDbContext();
    }
}
