using Moq;
using API.Models;
using API.Services;

public class ContactServiceTests
{
    private readonly Mock<IContactService> _mockContactService;

    public ContactServiceTests()
    {
        _mockContactService = new Mock<IContactService>();
    }

    [Fact]
    public async Task GetContacts_ReturnsContacts()
    {
        // Arrange
        var contacts = new List<Contact> { new Contact { Id = 1, Name = "John Doe" } };
        _mockContactService.Setup(service => service.GetContacts(It.IsAny<string>())).ReturnsAsync(contacts);

        // Act
        var result = await _mockContactService.Object.GetContacts("");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
    }

    [Fact]
    public async Task AddContact_ReturnsAddedContact()
    {
        // Arrange
        var contact = new Contact { Id = 1, Name = "John Doe" };
        _mockContactService.Setup(service => service.AddContact(It.IsAny<Contact>())).ReturnsAsync(contact);

        // Act
        var result = await _mockContactService.Object.AddContact(contact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task GetContact_ReturnsContact()
    {
        // Arrange
        var contact = new Contact { Id = 1, Name = "John Doe" };
        _mockContactService.Setup(service => service.GetContact(It.IsAny<int>())).ReturnsAsync(contact);

        // Act
        var result = await _mockContactService.Object.GetContact(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task UpdateContact_ReturnsUpdatedContact()
    {
        // Arrange
        var contact = new Contact { Id = 1, Name = "John Doe" };
        _mockContactService.Setup(service => service.UpdateContact(It.IsAny<Contact>())).ReturnsAsync(contact);

        // Act
        var result = await _mockContactService.Object.UpdateContact(contact);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task DeleteContact_ReturnsTrue()
    {
        // Arrange
        _mockContactService.Setup(service => service.DeleteContact(It.IsAny<int>())).ReturnsAsync(true);

        // Act
        var result = await _mockContactService.Object.DeleteContact(1);

        // Assert
        Assert.True(result);
    }
}
