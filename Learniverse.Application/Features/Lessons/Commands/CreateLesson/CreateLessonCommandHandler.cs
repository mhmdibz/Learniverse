using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Application.Services;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Lessons.Commands.CreateLesson
{
    public sealed class CreateLessonCommandHandler : IRequestHandler<CreateLessonCommand, Guid>
    {
        public readonly ISectionRepository _sectionRepository;
        public readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAuthorizationService _courseAuthorizationService;

        public CreateLessonCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork, ICourseAuthorizationService courseAuthorizationService, ICourseRepository courseRepository)
        {
            _sectionRepository = sectionRepository;
            _unitOfWork = unitOfWork;
            _courseAuthorizationService = courseAuthorizationService;
            _courseRepository = courseRepository;
        }

        public async Task<Guid> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
        {
            var section = await _sectionRepository.GetByIdWithLessonsAsync(request.SectionId, cancellationToken);
            if (section is null)
                throw new NotFoundException(nameof(Section), request.SectionId);

            var course = await _courseRepository.GetByIdAsync(section.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException(nameof(Course), section.CourseId);

            _courseAuthorizationService.EnsureCanModify(course);

            var lesson = section.AddLesson(request.Title, request.Description, request.Order, request.ContentType, request.IsPreview);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return lesson.Id;
        }

    }
}
