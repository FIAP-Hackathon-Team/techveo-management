namespace TechVeo.Management.Domain.Tests.Fixtures;

public class VideoFixture
{
    public Domain.Entities.Video CreateValidVideo(Guid userId, string fileName = "test-video.mp4")
    {
        return new Domain.Entities.Video(
            userId,
            fileName,
            snapshotCount: 5,
            intervalSeconds: 10.5,
            width: 1920,
            height: 1080);
    }

    public Domain.Entities.Video CreateVideoWithoutSnapshots(Guid userId)
    {
        return new Domain.Entities.Video(
            userId,
            "simple-video.mp4",
            snapshotCount: null,
            intervalSeconds: null,
            width: 1280,
            height: 720);
    }

    public Domain.Entities.Video Create4KVideo(Guid userId)
    {
        return new Domain.Entities.Video(
            userId,
            "4k-video.mp4",
            snapshotCount: 20,
            intervalSeconds: 5.0,
            width: 3840,
            height: 2160);
    }

    public Domain.Entities.Video CreateHDVideo(Guid userId)
    {
        return new Domain.Entities.Video(
            userId,
            "hd-video.mp4",
            snapshotCount: 8,
            intervalSeconds: 15.0,
            width: 1280,
            height: 720);
    }
}
