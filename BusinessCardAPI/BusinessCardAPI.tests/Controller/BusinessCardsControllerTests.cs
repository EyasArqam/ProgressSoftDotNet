using BusinessCardAPI.Controllers;
using BusinessCardAPI.Data;
using BusinessCardAPI.Models.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace BusinessCardAPI.tests.Controller;

public class BusinessCardsControllerTests
{
    private BusinessCardDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BusinessCardDBContext>()
            .UseInMemoryDatabase(databaseName: "BusinessCardDB")
            .Options;

        var context = new BusinessCardDBContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetAll_ReturnsNoContent_WhenNoBusinessCardsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new BusinesCardsController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async Task GetAll_ReturnsOk_WhenBusinessCardsExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();

        context.BusinessCards.Add(new BusinessCard { Id = 1, Name = "Test Card", Email = "test@email.com", Phone = "0799999999", Address = "ssssss", IsDeleted = false });
        await context.SaveChangesAsync();

        var controller = new BusinesCardsController(context);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeAssignableTo<IQueryable<BusinessCard>>();
        var businessCards = okResult.Value as IQueryable<BusinessCard>;
        businessCards.Should().HaveCount(1);
        businessCards.First().Name.Should().Be("Test Card");
    }

}

