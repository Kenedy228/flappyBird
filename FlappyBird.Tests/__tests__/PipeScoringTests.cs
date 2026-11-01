using FlappyBird;
using Xunit;

namespace FlappyBird.Tests;

public sealed class PipeScoringTests
{
    [Fact]
    public void Update_SafePassFollowedByJumpAwardsExactlyOnePoint()
    {
        var simulation = new GameSimulation();
        var pipe = simulation.Pipes[0];
        pipe.xCoordinates = [-0.25f, -0.05f, -0.05f, -0.25f];
        SetGap(pipe);
        simulation.Bird.yCoordinates = [0.47f, 0.47f, 0.32f, 0.32f];
        simulation.Bird.yJumpCoordinates = [0.77f, 0.77f, 0.62f, 0.62f];

        for (int i = 0; i < 95; i++)
        {
            simulation.Update(1.0 / 60);
            if (simulation.Bird.yCoordinates[0] < 0.65f) simulation.Bird.JumpHandler();
        }

        Assert.False(simulation.IsFinished);
        Assert.Equal(1, simulation.Score.Value);
    }

    [Theory]
    [InlineData(0.52f, 0.37f)]
    [InlineData(0.13f, -0.02f)]
    public void Move_SafeHitboxPassDoesNotRequireWholeSpriteInsideGap(float top, float bottom)
    {
        var pipe = new Pipe([-0.15f, 0.05f, 0.05f, -0.15f]);
        SetGap(pipe);
        var bird = new Bird { yCoordinates = [top, top, bottom, bottom] };
        var score = new Score();

        for (int i = 0; i < 30; i++) pipe.Move(bird.xCoordinates, bird.yCoordinates, score);

        Assert.False(pipe.gameFinish);
        Assert.Equal(1, score.Value);
    }

    [Fact]
    public void Move_OverlappingPipeDoesNotAwardPoint()
    {
        var pipe = new Pipe([-0.1f, 0.1f, 0.1f, -0.1f]);
        SetGap(pipe);
        var bird = new Bird { yCoordinates = [0.4f, 0.4f, 0.25f, 0.25f] };
        var score = new Score();

        pipe.Move(bird.xCoordinates, bird.yCoordinates, score);

        Assert.False(pipe.gameFinish);
        Assert.Equal(0, score.Value);
    }

    [Theory]
    [InlineData(0.7f, 0.55f)]
    [InlineData(-0.15f, -0.3f)]
    public void Move_CollisionRemainsIneligibleForPointAfterPassing(float top, float bottom)
    {
        var pipe = new Pipe([-0.1f, 0.1f, 0.1f, -0.1f]);
        SetGap(pipe);
        var bird = new Bird { yCoordinates = [top, top, bottom, bottom] };
        var score = new Score();
        pipe.Move(bird.xCoordinates, bird.yCoordinates, score);
        Assert.True(pipe.gameFinish);
        Assert.Equal(0, score.Value);

        bird.yCoordinates = [0.4f, 0.4f, 0.25f, 0.25f];
        for (int i = 0; i < 30; i++) pipe.Move(bird.xCoordinates, bird.yCoordinates, score);

        Assert.True(pipe.gameFinish);
        Assert.Equal(0, score.Value);
    }

    [Fact]
    public void Move_RespawnAllowsOnePointOnNextSafePass()
    {
        var pipe = new Pipe([-0.3f, -0.1f, -0.1f, -0.3f]);
        SetGap(pipe);
        var bird = new Bird { yCoordinates = [0.4f, 0.4f, 0.25f, 0.25f] };
        var score = new Score();
        pipe.Move(bird.xCoordinates, bird.yCoordinates, score);
        Assert.Equal(1, score.Value);

        for (int i = 0; i < 100 && pipe.xCoordinates[1] >= -1; i++)
            pipe.Move(bird.xCoordinates, bird.yCoordinates, score);
        Assert.True(pipe.xCoordinates[1] < -1);
        pipe.Move(bird.xCoordinates, bird.yCoordinates, score);
        Assert.True(pipe.xCoordinates[0] > 0.9f);
        SetGap(pipe);

        for (int i = 0; i < 150; i++) pipe.Move(bird.xCoordinates, bird.yCoordinates, score);

        Assert.False(pipe.gameFinish);
        Assert.Equal(2, score.Value);
    }

    private static void SetGap(Pipe pipe)
    {
        pipe.yCoordinatesUp = [0, 0, -2, -2];
        pipe.yCoordinatesDown = [0.5f, 0.5f, 2.5f, 2.5f];
    }
}
