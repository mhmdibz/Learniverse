using Learniverse.Domain.Common;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 namespace Learniverse.Domain.Entities
{
    public class Course : AuditableEntity
    {
        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public decimal Price { get; private set; }
        public CourseStatus Status { get; private set; }
        public string InstructorId { get; private set; } = default!;
        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = default!;
        private readonly List<Section> _sections = new();
        public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();
        private Course() { }

        private Course(string title, string description, decimal price, string instructorId, Guid categoryId)
        {
            Id = Guid.CreateVersion7();
            SetTitle(title);
            SetDescription(description);
            SetPrice(price);
            InstructorId = instructorId;
            CategoryId = categoryId;
            Status = CourseStatus.Draft;
        }
        public Section AddSection(string title, string description, int order)
        {
            if (_sections.Any(s => s.Order == order))
                throw new DomainException("Section order must be unique."); var section = new Section(Id, title, description, order);
            _sections.Add(section);

            return section;
        }
        public void DeleteSection(
            Guid sectionId,
            string deletedBy)
        {
            var section = _sections.FirstOrDefault(s => s.Id == sectionId);

            if (section is null)
                throw new DomainException("Section not found.");

            section.Delete(deletedBy);
            foreach (var lesson in section.Lessons)
                lesson.Delete(deletedBy);
        }
        public void DeleteWithSections(string deletedBy)
        {
            Delete(deletedBy);

            foreach (var section in _sections)
            {
                section.Delete(deletedBy);

                foreach (var lesson in section.Lessons)
                {
                    lesson.Delete(deletedBy);
                }
            }
        }
        public void ChangeSectionOrder(Guid sectionId, int newOrder)
        {
            if (_sections.Any(s => s.Order == newOrder))
                throw new DomainException("Section order must be unique.");

            var section = _sections.FirstOrDefault(s => s.Id == sectionId);

            if (section is null)
                throw new DomainException("Section not found.");

            section.SetOrder(newOrder);
        }
        public static Course Create(string title, string description, decimal price, string instructorId, Guid categoryId)
        {
            return new Course(title, description, price, instructorId, categoryId);
        }
        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Course title is required.");

            title = title.Trim();
            Title = title;
        }

        public void UpdateDetails(
     string title,
     string description,
     decimal price,
     CourseStatus status)
        {
            SetTitle(title);
            SetDescription(description);
            SetPrice(price);
            ChangeStatus(status);
        }
        public void ChangeStatus(CourseStatus status)
        {
            switch (status)
            {
                case CourseStatus.Draft:
                    Status = CourseStatus.Draft;
                    break;

                case CourseStatus.Published:
                    Publish();
                    break;

                case CourseStatus.Archived:
                    Archive();
                    break;

                default:
                    throw new DomainException("Invalid course status.");
            }
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Course description is required.");

            Description = description;
        }


        private void SetPrice(decimal price)
        {
            if (price < 0)
                throw new DomainException("Course price cannot be negative.");

            Price = price;
        }
        public void Publish()
        {
            if (Status == CourseStatus.Published)
                return;
            if (_sections.Count == 0)
                throw new DomainException("Course must have at least one section before publishing.");
            Status = CourseStatus.Published;
        }

        public void Archive()
        {
            if (Status == CourseStatus.Archived)
                return;
            Status = CourseStatus.Archived;
        }

    }
}
