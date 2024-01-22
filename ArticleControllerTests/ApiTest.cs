namespace ArticleControllerTests;

public class ApiTest
{
    [Fact]
    public async Task Get_Articles_Returns_200_Ok_With_List_Of_Articles()
    {
        // Arrange
        var senderMock = new Mock<ISender>();
        var controller = new ArticleController(senderMock.Object);
        var expectedResponse = new GetArticlesDto();

        senderMock.Setup(s => s.Send(It.IsAny<GetArticlesQuery>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await controller.GetArticles();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<GetArticlesResponse>(okResult.Value);
        Assert.Equal(expectedResponse, response);
    }
}
