using AutoMapper;
using BusinessCardAPI.Controllers;
using BusinessCardAPI.Data;
using BusinessCardAPI.Interfaces;
using BusinessCardAPI.Models.DTOs;
using BusinessCardAPI.Models.Entities;
using BusinessCardAPI.Models.Enums;
using BusinessCardAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;

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
        var searchParams = new Search
        {
            Name = "John",
            Gender = Gender.Male
        };

        // Act
        var result = await controller.GetFilteredBusinessCards(searchParams);

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
        var searchParams = new Search
        {
            Name = "John",
            Gender = Gender.Male
        };

        // Act
        var result = await controller.GetFilteredBusinessCards(searchParams);

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




    [Fact]
    public async Task PostForm_ShouldReturnOk_WhenFormDataIsValid()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new BusinesCardsController(context, null, null);  // Adjust this based on constructor

        var businessCard = new BusinessCard
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "123-456-7890",
            Address = "123 Main St",
            Gender = Models.Enums.Gender.Female,
            DateOfBirth = new DateTime(1990, 1, 1),
            Photo = "base64image"
        };

        // Act
        var result = await controller.PostForm(businessCard);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(new { message = "Business card processed successfully." });
    }

    [Fact]
    public async Task PostForm_ShouldReturnBadRequest_WhenFormDataIsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new BusinesCardsController(context, null, null);  // Adjust this based on constructor

        // Act
        var result = await controller.PostForm(null);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Value.Should().Be("Form data cannot be null.");
    }




    [Fact]
    public async Task DeleteBusinessCard_ShouldReturnOk_WhenBusinessCardIsDeleted()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new BusinesCardsController(context, null, null);

        // Add a business card to the in-memory database
        var businessCard = new BusinessCard
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "123-456-7890",
            Address = "123 Main St",
            Gender = Models.Enums.Gender.Male,
            DateOfBirth = new DateTime(1990, 1, 1),
            Photo = "base64image"
        };
        context.BusinessCards.Add(businessCard);
        await context.SaveChangesAsync();

        // Act
        var result = await controller.DeleteBusinessCard(businessCard.Id);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = "Business card deleted successfully." });

        // Verify that the business card is marked as deleted
        var deletedCard = await context.BusinessCards.FindAsync(businessCard.Id);
        deletedCard.Should().NotBeNull();
        deletedCard.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteBusinessCard_ShouldReturnNotFound_WhenBusinessCardDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var controller = new BusinesCardsController(context, null, null);
        var nonExistentId = 999; // An ID that does not exist

        // Act
        var result = await controller.DeleteBusinessCard(nonExistentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }




    [Fact]
    public async Task ExportXml_ReturnsNotFound_WhenBusinessCardDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        // Act
        var result = await controller.ExportXml(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ExportXml_ReturnsFileResult_WhenBusinessCardExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var businessCardServiceMock = new Mock<IBusinessCardService>();
        var mapperServiceMock = new Mock<IMapper>();

        var testBusinessCard = new BusinessCard
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            Phone = "123-456-7890",
            Address = "123 Main St, Springfield, USA"
        };

        context.BusinessCards.Add(testBusinessCard);
        await context.SaveChangesAsync();

        mapperServiceMock.Setup(m => m.Map<businessCard>(It.IsAny<BusinessCard>()))
            .Returns(new businessCard
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Address = "123 Main St, Springfield, USA"
            });

        var controller = new BusinesCardsController(context, businessCardServiceMock.Object, mapperServiceMock.Object);

        // Act
        var result = await controller.ExportXml(1);

        // Assert
        result.Should().BeOfType<FileContentResult>();

        var fileResult = result as FileContentResult;
        fileResult.ContentType.Should().Be("application/xml");
        fileResult.FileDownloadName.Should().Be("businessCard_1.xml");

        var xmlContent = Encoding.UTF8.GetString(fileResult.FileContents);

        xmlContent.Should().Contain("</businessCard>");
        xmlContent.Should().Contain("<Name>John Doe</Name>");
        xmlContent.Should().Contain("<Email>john.doe@example.com</Email>");
        xmlContent.Should().Contain("<Phone>123-456-7890</Phone>");
        xmlContent.Should().Contain("<Address>123 Main St, Springfield, USA</Address>");
    }



    [Fact]
    public async Task ExportCsv_ReturnsFileResult_WhenBusinessCardExists()
    {
        // Arrange
        int businessCardId = 1;
        var businessCard = new BusinessCard
        {
            Id = businessCardId,
            Name = "Test Card",
            Email = "test@email.com",
            Phone = "0799999999",
            Address = "123 Main St",
            Gender = Models.Enums.Gender.Male
        };
        var context = GetInMemoryDbContext();
        context.BusinessCards.Add(businessCard);
        await context.SaveChangesAsync();

        var mockBusinessCardService = new Mock<IBusinessCardService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, mockBusinessCardService.Object, mockMapper.Object);

        var csvContent = "Name,Email,Phone,DateOfBirth,Address,Gender\nTest Card,test@email.com,0799999999,,123 Main St,Male";
        mockBusinessCardService.Setup(s => s.ConvertToCsv(It.IsAny<businessCard>()))
            .Returns(csvContent);

        // Act
        var result = await controller.ExportCsv(businessCardId) as FileContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.Equal($"BusinessCard_{businessCardId}.csv", result.FileDownloadName);

        var expectedBytes = Encoding.UTF8.GetBytes(csvContent);
        Assert.Equal(expectedBytes, result.FileContents);
    }

    [Fact]
    public async Task ExportCsv_ReturnsNotFound_WhenBusinessCardDoesNotExist()
    {
        // Arrange
        int businessCardId = 999; // Non-existing ID
        var context = GetInMemoryDbContext();
        var mockBusinessCardService = new Mock<IBusinessCardService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, mockBusinessCardService.Object, mockMapper.Object);

        // Act
        var result = await controller.ExportCsv(businessCardId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ExportCsv_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        int businessCardId = 1;
        var businessCard = new BusinessCard
        {
            Id = businessCardId,
            Name = "Test Card",
            Email = "test@email.com",
            Phone = "0799999999",
            Address = "123 Main St",
            Gender = Models.Enums.Gender.Male
        };
        var context = GetInMemoryDbContext();
        context.BusinessCards.Add(businessCard);
        await context.SaveChangesAsync();

        var mockBusinessCardService = new Mock<IBusinessCardService>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BusinesCardsController(context, mockBusinessCardService.Object, mockMapper.Object);

        // Setup to throw exception during CSV conversion
        mockBusinessCardService.Setup(s => s.ConvertToCsv(It.IsAny<businessCard>()))
            .Throws(new Exception("Error generating CSV."));

        // Act
        var result = await controller.ExportCsv(businessCardId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Error generating CSV.", badRequestResult.Value);
    }
}

