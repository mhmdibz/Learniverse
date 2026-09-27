using Learniverse.Domain.Common;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public class Category : AuditableEntity
{
    public string Name { get; private set; } = default!;
    private readonly List<Course> _courses = [];

    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Category()
    {
    }

    private Category(string name)
    {
        Id = Guid.CreateVersion7();
        SetName(name);
    }

    public static Category Create(string name)
    {
        return new Category(name);
    }

    public void SetName(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required.");

        if (name.Length > 100)
            throw new DomainException("Category name must not exceed 100 characters.");
        name = name.Trim();
        Name = name;
    }
}