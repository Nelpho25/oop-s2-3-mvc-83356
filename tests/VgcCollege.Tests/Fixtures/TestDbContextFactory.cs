using Microsoft.EntityFrameworkCore;
using oop_s2_1_mvc_83356.Data;

namespace VgcCollege.Tests.Fixtures;

public static class TestDbContextFactory
{
    public static ApplicationDbContext CreateInMemoryDbContext(string databaseName = "TestDb")
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static ApplicationDbContext CreateInMemoryDbContext(DbContextOptions<ApplicationDbContext> options)
    {
        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
