using FlappyBird;
using OpenTK.Mathematics;
using Xunit;

namespace FlappyBird.Tests;

public sealed class MenuInputTests
{
    [Fact]
    public void Click_FormerWindowCenterIsOutsidePlayAfterEnlarging()
    {
        var menu = new Menu();

        menu.MouseClickHandler(new Vector2(400, 300), new Vector2i(1600, 1200));

        Assert.Equal(0, menu.gameStatus);
    }

    [Theory]
    [InlineData(800, 600, 400, 300, 2)]
    [InlineData(1600, 1200, 800, 600, 2)]
    [InlineData(400, 300, 200, 150, 2)]
    [InlineData(1000, 500, 500, 250, 2)]
    [InlineData(1600, 1200, 800, 672, 1)]
    [InlineData(1600, 1200, 800, 744, -1)]
    public void Click_UsesCurrentClientSizeAndPosition(int width, int height, int x, int y, int expected)
    {
        var menu = new Menu();

        menu.MouseClickHandler(new Vector2(x, y), new Vector2i(width, height));

        Assert.Equal(expected, menu.gameStatus);
    }

    [Fact]
    public void Click_SettingsFollowCurrentSizeAcrossResizes()
    {
        var menu = new Menu();
        menu.MouseClickHandler(new Vector2(800, 672), new Vector2i(1600, 1200));
        Assert.Equal(1, menu.gameStatus);

        menu.MouseClickHandler(new Vector2(200, 150), new Vector2i(400, 300));
        Assert.Equal(1, menu.birdColorCounter);
        menu.MouseClickHandler(new Vector2(500, 280), new Vector2i(1000, 500));
        Assert.Equal(1, menu.pipeColorCounter);
        menu.MouseClickHandler(new Vector2(400, 372), new Vector2i(800, 600));
        Assert.Equal(0, menu.gameStatus);
    }

    [Theory]
    [InlineData(600, 2)]
    [InlineData(672, 0)]
    public void Click_GameOverButtonsKeepRestartBehavior(int y, int expected)
    {
        var menu = new Menu { gameStatus = 3 };

        menu.MouseClickHandler(new Vector2(800, y), new Vector2i(1600, 1200));

        Assert.Equal(expected, menu.gameStatus);
        Assert.True(menu.restart);
    }

    [Theory]
    [InlineData(0, 600)]
    [InlineData(800, 0)]
    [InlineData(0, 0)]
    public void Click_EmptyClientAreaDoesNothing(int width, int height)
    {
        var menu = new Menu();

        menu.MouseClickHandler(Vector2.Zero, new Vector2i(width, height));

        Assert.Equal(0, menu.gameStatus);
        Assert.False(menu.restart);
    }

    [Fact]
    public void Click_OutsideClientDoesNotActivateMenu()
    {
        var menu = new Menu();

        menu.MouseClickHandler(new Vector2(-10, 300), new Vector2i(800, 600));

        Assert.Equal(0, menu.gameStatus);
    }
}
