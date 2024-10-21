using AutoMapper;
using BusinessCardAPI.Controllers;
using BusinessCardAPI.Data;
using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;
using BusinessCardAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;


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
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

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
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();


        context.BusinessCards.Add(new BusinessCard { Id = 1, Name = "Test Card", Email = "test@email.com", Phone = "0799999999", Address = "ssssss", IsDeleted = false });
        await context.SaveChangesAsync();

        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeAssignableTo<IEnumerable<BusinessCard>>();
        var businessCards = okResult.Value as IEnumerable<BusinessCard>;
        businessCards.Should().HaveCount(1);
        businessCards.First().Name.Should().Be("Test Card");
    }



    [Fact]
    public async Task PostFiles_ShouldReturnBadRequest_WhenNoFileUploaded()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        // Act
        var result = await controller.PostXmlFile(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Fact]
    public async Task PostFiles_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(0);

        // Act
        var result = await controller.PostXmlFile(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Fact]
    public async Task PostFiles_ShouldReturnOk_WhenFileIsProcessedSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns("test.xml");
        fileMock.Setup(_ => _.Length).Returns(10);
        using (var stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream);
            writer.Write("<businessCards> <businessCard> <name>John Doe</name> <gender>Male</gender> <dateOfBirth>1990-01-01</dateOfBirth> <email>john.doe@example.com</email> <phone>123-456-7890</phone> <address>123 Main St, Springfield, USA</address> </businessCard> </businessCards>");
            writer.Flush();
            stream.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
        }

        var businessCardDTOs = new Result
        {
            Ok = true,
            Data = new List<BusinessCardDTO> { new BusinessCardDTO() },
            Message = "Processed successfully."
        };

        businessCardServiceMock
            .Setup(service => service.ReadBusinessCardsFromXml(fileMock.Object))
            .ReturnsAsync(businessCardDTOs);

        // Act
        var result = await controller.PostXmlFile(fileMock.Object);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = "Business cards processed successfully.", records = businessCardDTOs.Data });
    }

    [Fact]
    public async Task PostFiles_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns("test.xml");
        fileMock.Setup(_ => _.Length).Returns(10);

        businessCardServiceMock
            .Setup(service => service.ReadBusinessCardsFromXml(fileMock.Object))
            .Throws(new Exception("Some error occurred."));

        // Act
        var result = await controller.PostXmlFile(fileMock.Object);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Which;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<string>()
            .Which.Should().Contain("Internal server error: Some error occurred.");
    }



    [Fact]
    public async Task PostCsvFile_ShouldReturnBadRequest_WhenNoFileUploaded()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        // Act
        var result = await controller.PostCsvFile(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Fact]
    public async Task PostCsvFile_ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.Length).Returns(0);

        // Act
        var result = await controller.PostCsvFile(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No file uploaded.");
    }

    [Fact]
    public async Task PostCsvFile_ShouldReturnBadRequest_WhenFileTypeIsNotCsv()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns("test.txt");
        fileMock.Setup(_ => _.Length).Returns(10);

        // Act
        var result = await controller.PostCsvFile(fileMock.Object);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("Unsupported file type. Please upload an CSV file.");
    }

    [Fact]
    public async Task ReadBusinessCardsFromCsv_ShouldReturnSuccess_WhenFileIsValid()
    {
        // Arrange
        var businessCardService = new BusinessCardService();

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns("test.csv");
        fileMock.Setup(_ => _.Length).Returns(10);

        using (var stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream);
            writer.Write("Name,Gender,DateOfBirth,Email,Phone,Address\nJohn Doe,Male,1990-01-01,john.doe@example.com,123-456-7890,123 Main St");
            writer.Flush();
            stream.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);

            // Act
            var result = await businessCardService.ReadBusinessCardsFromCsv(fileMock.Object);

            // Assert
            result.Ok.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(1);
            var businessCard = result.Data.First();
            businessCard.Name.Should().Be("John Doe");
            businessCard.Gender.Should().Be("Male");
        }
    }


    [Fact]
    public async Task PostCsvFile_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(_ => _.FileName).Returns("test.csv");
        fileMock.Setup(_ => _.Length).Returns(10);

        businessCardServiceMock
            .Setup(service => service.ReadBusinessCardsFromCsv(fileMock.Object))
            .Throws(new Exception("Some error occurred."));

        // Act
        var result = await controller.PostCsvFile(fileMock.Object);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Which;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<string>()
            .Which.Should().Contain("Internal server error: Some error occurred.");
    }

}

