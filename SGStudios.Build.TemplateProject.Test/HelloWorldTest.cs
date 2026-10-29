using Xunit;

namespace SGStudios.Build.TemplateProject.Test;

public class HelloWorldTest
{
    [Fact]
    public void GetMessageShouldReturnExpectedMessage()
    {
        Assert.Equal("Hello, World!", HelloWorld.GetMessage());
    }
}
