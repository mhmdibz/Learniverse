using Learniverse.Domain.Common;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public class Lesson : AuditableEntity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    public int Order { get; private set; }

    public ContentType ContentType { get; private set; }
    public Guid SectionId { get; private set; }
    public bool IsPreview { get; private set; }

    private Lesson() { }

    internal Lesson(Guid sectionId, string title, string description, int order,
      ContentType contentType,
      bool isPreview = false)
    {
        Id = Guid.CreateVersion7();
        SectionId = sectionId;
        SetTitle(title);
        SetDescription(description);
        SetOrder(order);

        ContentType = contentType;
        IsPreview = isPreview;
    }
    public void UpdateDetails(
        string title,
        string description)
    {
        SetTitle(title);
        SetDescription(description);
    }

    public void MakePreview()
    {
        IsPreview = true;
    }

    public void RemovePreview()
    {
        IsPreview = false;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Lesson title is required.");
        title = title.Trim();
        Title = title;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Lesson description is required.");

        Description = description;
    }

    internal void SetOrder(int order)
    {
        if (order < 1)
            throw new DomainException("Lesson order must be greater than zero.");

        Order = order;
    }
    public void ChangeContentType(ContentType contentType)
    {
        ContentType = contentType;
    }

    public void SetPreview(bool isPreview)
    {
        IsPreview = isPreview;
    }
}