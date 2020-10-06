using HW_6.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace HW_6.Test
{
    public class Tests
    {     
        [SetUp]
        public void Setup()
        {
            var option = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Test")
            .Options;
        }

        [Test]
        public void GetBlogPostAsync()
        {
            int blogId = 1;
            Assert.Fail();
        }
    }
}