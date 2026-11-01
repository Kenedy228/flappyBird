using System.Reflection;
using System.Runtime.CompilerServices;
using FlappyBird;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Xunit;

namespace FlappyBird.Tests;

public sealed class KeyboardInputTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Space_InMenuDoesNotChangeFirstStepAfterPlay(bool visitSettings)
    {
        var menu = new Menu();
        var simulation = new GameSimulation();
        var game = CreateWithoutWindow(menu, simulation);
        if (visitSettings)
            menu.MouseClickHandler(new Vector2(400, 336), new Vector2i(800, 600));

        ReleaseKey(game, Keys.Space);
        if (visitSettings)
            menu.MouseClickHandler(new Vector2(400, 372), new Vector2i(800, 600));
        menu.MouseClickHandler(new Vector2(400, 300), new Vector2i(800, 600));
        Assert.Equal(2, menu.gameStatus);
        simulation.Update(1.0 / 60);

        Assert.InRange(simulation.Bird.yCoordinates[0], 0.68999f, 0.69001f);
    }

    [Fact]
    public void Space_InActiveRoundStillJumpsOnRelease()
    {
        var menu = new Menu { gameStatus = 2 };
        var simulation = new GameSimulation();
        var game = CreateWithoutWindow(menu, simulation);

        ReleaseKey(game, Keys.Space);
        simulation.Update(1.0 / 60);

        Assert.InRange(simulation.Bird.yCoordinates[0], 0.74999f, 0.75001f);
    }

    [Fact]
    public void Space_AfterGameOverDoesNotChangeJumpTarget()
    {
        var menu = new Menu { gameStatus = 3 };
        var simulation = new GameSimulation();
        simulation.Bird.gameFinish = true;
        var target = (float[])simulation.Bird.yJumpCoordinates.Clone();
        var game = CreateWithoutWindow(menu, simulation);

        ReleaseKey(game, Keys.Space);

        Assert.Equal(target, simulation.Bird.yJumpCoordinates);
    }

    [Fact]
    public void OtherKey_InActiveRoundDoesNotJump()
    {
        var menu = new Menu { gameStatus = 2 };
        var simulation = new GameSimulation();
        var game = CreateWithoutWindow(menu, simulation);

        ReleaseKey(game, Keys.A);
        simulation.Update(1.0 / 60);

        Assert.InRange(simulation.Bird.yCoordinates[0], 0.68999f, 0.69001f);
    }

    // OnKeyUp needs only these managed fields; constructing Game would open a
    // native window and upload textures. This fixture exercises the actual handler
    // without replacing input policy or requiring a display on CI runners.
    private static Game CreateWithoutWindow(Menu menu, GameSimulation simulation)
    {
        var game = (Game)RuntimeHelpers.GetUninitializedObject(typeof(Game));
        GC.SuppressFinalize(game); // No native window was allocated to dispose.
        typeof(Game).GetField("menu", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(game, menu);
        typeof(Game).GetField("simulation", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(game, simulation);
        return game;
    }

    private static void ReleaseKey(Game game, Keys key) =>
        typeof(Game).GetMethod("OnKeyUp", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(game, [new KeyboardKeyEventArgs(key, 0, default, false)]);
}
