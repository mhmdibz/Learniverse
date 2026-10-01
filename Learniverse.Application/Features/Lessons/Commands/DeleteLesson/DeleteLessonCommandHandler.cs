using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Lessons.Commands.DeleteLesson
{
    public sealed class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonCommand>
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAuthorizationService _courseAuthorizationService;
        public DeleteLessonCommandHandler(
       ISectionRepository sectionRepository,
       IUnitOfWork unitOfWork,
       ICurrentUserService currentUserService,
       ICourseRepository courseRepository,
       ICourseAuthorizationService courseAuthorizationService)
        {
            _sectionRepository = sectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _courseRepository = courseRepository;
            _courseAuthorizationService = courseAuthorizationService;
        }
        public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
        {
            var section = await _sectionRepository.GetByIdWithLessonsAsync(request.SectionId, cancellationToken);
            if (section == null)
                throw new NotFoundException(nameof(Section), request.SectionId);
            var course = await _courseRepository.GetByIdAsync(section.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException(nameof(Course), section.CourseId);
            _courseAuthorizationService.EnsureCanModify(course);
            var deletedBy = _currentUserService.UserId;
            var lesson = section.Lessons
                .FirstOrDefault(l => l.Id == request.LessonId);
            if (lesson is null)
                throw new NotFoundException(
                    nameof(Lesson),
                    request.LessonId);
            section.DeleteLesson(
                request.LessonId,
                deletedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
