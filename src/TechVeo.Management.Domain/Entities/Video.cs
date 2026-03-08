using System;
using System.Net.Mail;
using TechVeo.Management.Domain.Enums;
using TechVeo.Shared.Domain.Entities;
using TechVeo.Shared.Domain.Exceptions;

namespace TechVeo.Management.Domain.Entities;

public class Video : Entity, IAggregateRoot
{
    public Video(
        Guid userId,
        string emailAddress,
        string? fileName,
        int? snapshotCount,
        double? intervalSeconds,
        int width,
        int height)
    {
        UserId = userId;
        EmailAddress = emailAddress;
        FileName = fileName;
        Status = Status.Queued;
        CreateAt = DateTime.UtcNow;
        SnapshotCount = snapshotCount;
        IntervalSeconds = intervalSeconds;
        SetWidth(width);
        SetHeight(height);
    }

    public Guid UserId { get; private set; }
    public string EmailAddress { get; private set; }
    public Status Status { get; private set; }
    public DateTime CreateAt { get; private set; }
    public int? SnapshotCount { get; private set; }
    public double? IntervalSeconds { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string? FileKey { get; private set; }
    public string? FileName { get; private set; }

    public void SetWidth(int width)
    {
        if (width < 0)
        {
            throw new DomainException("Width must be non-negative");
        }

        Width = width;
    }

    public void SetFileKey(string fileKey)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
        {
            throw new DomainException("File key must be provided");
        }

        FileKey = fileKey;
    }

    public void SetHeight(int height)
    {
        if (height < 0)
        {
            throw new DomainException("Height must be non-negative");
        }

        Height = height;
    }
}
