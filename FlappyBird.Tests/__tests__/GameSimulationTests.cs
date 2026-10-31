using FlappyBird;
using Xunit;

namespace FlappyBird.Tests;

public sealed class GameSimulationTests
{
    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(144)]
    public void Update_OneSecondPreservesSixtyHzMovementAndAnimation(int frames)
    {
        var simulation = new GameSimulation();

        for (int i = 0; i < frames; i++) simulation.Update(1.0 / frames);

        AssertClose(0.4f, simulation.Pipes[0].xCoordinates[0]);
        AssertClose(1.4f, simulation.Pipes[1].xCoordinates[0]);
        AssertClose(0.1f, simulation.Bird.yCoordinates[0]);
        AssertClose(-0.05f, simulation.Bird.yCoordinates[2]);
        Assert.Equal(2, simulation.Bird.AnimationFrame);
        Assert.False(simulation.IsFinished);
    }

    [Fact]
    public void Update_IrregularFramesPreserveUnconsumedTime()
    {
        var simulation = new GameSimulation();

        for (int i = 0; i < 10; i++)
            foreach (double elapsed in new[] { 0.01, 0.02, 0.04, 0.005, 0.025 })
                simulation.Update(elapsed);

        AssertClose(0.4f, simulation.Pipes[0].xCoordinates[0]);
        AssertClose(0.1f, simulation.Bird.yCoordinates[0]);
        Assert.Equal(2, simulation.Bird.AnimationFrame);
    }

    [Fact]
    public void Update_AccumulatesPartialStepWithoutInventingMovement()
    {
        var simulation = new GameSimulation();

        simulation.Update(0);
        simulation.Update(1.0 / 120);

        AssertClose(1, simulation.Pipes[0].xCoordinates[0]);
        AssertClose(0.7f, simulation.Bird.yCoordinates[0]);

        simulation.Update(1.0 / 120);

        AssertClose(0.99f, simulation.Pipes[0].xCoordinates[0]);
        AssertClose(0.69f, simulation.Bird.yCoordinates[0]);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(144)]
    public void Update_SameTimedJumpHasSameResultAcrossFrameRates(int frames)
    {
        var simulation = new GameSimulation();
        for (int i = 0; i < frames / 2; i++) simulation.Update(1.0 / frames);

        simulation.Bird.JumpHandler();
        for (int i = 0; i < frames / 2; i++) simulation.Update(1.0 / frames);

        AssertClose(0.4f, simulation.Pipes[0].xCoordinates[0]);
        AssertClose(0.46f, simulation.Bird.yCoordinates[0]);
        AssertClose(0.46f, simulation.Bird.yJumpCoordinates[0]);
        Assert.Equal(2, simulation.Bird.AnimationFrame);
    }

    [Fact]
    public void Update_PreservesJumpCeilingAndReturnToFalling()
    {
        var simulation = new GameSimulation();
        simulation.Bird.JumpHandler();

        simulation.Update(0.1);

        AssertClose(1, simulation.Bird.yCoordinates[0]);
        AssertClose(1, simulation.Bird.yJumpCoordinates[0]);

        simulation.Update(1.0 / 60);

        AssertClose(0.99f, simulation.Bird.yCoordinates[0]);
        AssertClose(0.99f, simulation.Bird.yJumpCoordinates[0]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Update_StopsCatchUpAndFurtherMovementWhenRoundFinishes(bool pipeCollision)
    {
        var simulation = new GameSimulation();
        if (pipeCollision)
        {
            simulation.Pipes[0].xCoordinates = [-0.05f, 0.15f, 0.15f, -0.05f];
            simulation.Pipes[0].yCoordinatesUp[0] = 0;
            simulation.Pipes[0].yCoordinatesDown[0] = 0.5f;
        }
        else
        {
            simulation.Bird.yCoordinates = [-0.845f, -0.845f, -0.995f, -0.995f];
            simulation.Bird.yJumpCoordinates = (float[])simulation.Bird.yCoordinates.Clone();
        }

        simulation.Update(1);

        Assert.True(simulation.IsFinished);
        AssertClose(1.99f, simulation.Pipes[1].xCoordinates[0]);
        AssertClose(pipeCollision ? 0.69f : -0.855f, simulation.Bird.yCoordinates[0]);

        simulation.Update(10);

        AssertClose(1.99f, simulation.Pipes[1].xCoordinates[0]);
        AssertClose(pipeCollision ? 0.69f : -0.855f, simulation.Bird.yCoordinates[0]);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Update_RejectsInvalidTimeWithoutPoisoningClock(double elapsed)
    {
        var simulation = new GameSimulation();

        Assert.Throws<ArgumentOutOfRangeException>(() => simulation.Update(elapsed));
        simulation.Update(1.0 / 60);

        AssertClose(0.99f, simulation.Pipes[0].xCoordinates[0]);
    }

    private static void AssertClose(float expected, float actual) =>
        Assert.InRange(actual, expected - 0.00001f, expected + 0.00001f);
}
