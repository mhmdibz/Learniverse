using Learniverse.Domain.Common;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public class Section : AuditableEntity
{

    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int Order { get; private set; }
    public Guid CourseId { get; private set; }

    private readonly List<Lesson> _lessons = new();
    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();


    private Section() { }

    internal Section(
        Guid courseId,
        string title,
        string description,
        int order
        )
    {
        Id = Guid.CreateVersion7();

        SetTitle(title);
        SetDescription(description);
        SetOrder(order);

        CourseId = courseId;
    }

    public void UpdateDetails(
        string title,
        string description)
    {
        SetTitle(title);
        SetDescription(description);
    }

    public Lesson AddLesson(
     string title,
     string description,
     int order,
     ContentType contentType,
     bool isPreview = false)
    {
        if (_lessons.Any(l => l.Order == order))
            throw new DomainException("Lesson order must be unique within the section.");

        var lesson = new Lesson(Id, title, description, order, contentType, isPreview);

        _lessons.Add(lesson);

        return lesson;
    }

    public void RemoveLesson(Guid lessonId)
    {
        var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lesson is null)
            throw new DomainException("Lesson not found.");

        _lessons.Remove(lesson);
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Section title is required.");

        Title = title;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Section description is required.");

        Description = description;
    }

    internal void SetOrder(int order)
    {
        if (order < 1)
            throw new DomainException("Section order must be greater than zero.");

        Order = order;
    }
    public void ChangeLessonOrder(Guid lessonId, int newOrder)
    {
        if (_lessons.Any(l => l.Id != lessonId && l.Order == newOrder))
            throw new DomainException("Lesson order must be unique within the section.");

        var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lesson is null)
            throw new DomainException("Lesson not found.");

        lesson.SetOrder(newOrder);
    }
    public void DeleteLesson(Guid lessonId, string deletedBy)
    {
        var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lesson is null)
            throw new DomainException("Lesson not found.");

        lesson.Delete(deletedBy);
    }
}

