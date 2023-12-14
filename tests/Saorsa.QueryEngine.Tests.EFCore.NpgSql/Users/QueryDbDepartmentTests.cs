namespace Saorsa.QueryEngine.Tests.EFCore.NpgSql.Users;

public class QueryDbDepartmentTests : Saorsa.QueryEngine.Tests.EFCore.Common.QueryDbDepartmentTests
{
    protected override QueryDbContext GetQueryDbContext()
    {
        return new QueryNpgsqlDbContext();
    }
}
