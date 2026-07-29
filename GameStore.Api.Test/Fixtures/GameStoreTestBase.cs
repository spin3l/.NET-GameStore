using GameStore.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Test.Fixtures;

public class GameStoreTestBase
{
    protected SqliteConnection Connection { get; private set; }
    protected GameStoreContext Db { get; private set; }

    [SetUp]
    public void Setup()
    {
        // Keep the connection open for the lifetime of the test
        Connection = new SqliteConnection("DataSource=:memory:");
        Connection.Open();

        var options = new DbContextOptionsBuilder<GameStoreContext>().UseSqlite(Connection).Options;

        Db = new GameStoreContext(options);
        Db.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        Db.Dispose();
        Connection.Dispose();
    }
}
