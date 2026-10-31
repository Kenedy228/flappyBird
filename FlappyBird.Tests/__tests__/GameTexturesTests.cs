using FlappyBird;
using Xunit;

namespace FlappyBird.Tests;

public sealed class GameTexturesTests : IDisposable
{
    private readonly string path = Path.Combine(Path.GetTempPath(), $"flappy-image-{Guid.NewGuid():N}");
    private static readonly byte[] Png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAYAAABytg0kAAAAFklEQVR4nGP4z8DwHwhB4H/D////HQA8mAe6UbBPpwAAAABJRU5ErkJggg==");
    private static readonly byte[] Jpeg = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wBDAAIBAQEBAQIBAQECAgICAgQDAgICAgUEBAMEBgUGBgYFBgYGBwkIBgcJBwYGCAsICQoKCgoKBggLDAsKDAkKCgr/2wBDAQICAgICAgUDAwUKBwYHCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgr/wAARCAABAAIDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD4jooor9sPyM//2Q==");

    [Fact]
    public void LoadImage_PreservesRgbaChannelsAlphaAndTopToBottomRows()
    {
        File.WriteAllBytes(path, Png);

        var image = GameTextures.LoadImage(path);

        Assert.Equal(2, image.Width);
        Assert.Equal(2, image.Height);
        Assert.Equal<byte>([
            255, 0, 0, 255, 0, 255, 0, 0,
            0, 0, 255, 128, 255, 255, 255, 64
        ], image.Data);
    }

    [Fact]
    public void LoadImage_ExpandsJpegToOpaqueRgba()
    {
        File.WriteAllBytes(path, Jpeg);

        var image = GameTextures.LoadImage(path);

        Assert.Equal(2, image.Width);
        Assert.Equal(1, image.Height);
        Assert.Equal(8, image.Data.Length);
        Assert.Equal(255, image.Data[3]);
        Assert.Equal(255, image.Data[7]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void LoadImage_ReleasesInputFileAfterSuccessOrDecodeFailure(bool malformed)
    {
        File.WriteAllBytes(path, malformed ? new byte[32] : Png);

        if (malformed)
            Assert.Throws<NotSupportedException>(() => GameTextures.LoadImage(path));
        else
            GameTextures.LoadImage(path);

        using var exclusive = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.Equal(malformed ? 32 : Png.Length, exclusive.Length);
    }

    [Fact]
    public void LoadImage_ReportsMissingFileBeforeGraphicsInitialization()
    {
        Assert.Throws<FileNotFoundException>(() => GameTextures.LoadImage(path));
    }

    public void Dispose() => File.Delete(path);
}
