using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.DeleteSection
{
    public sealed class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICourseAuthorizationService _courseAuthorizationService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSectionCommandHandler(
             ICourseRepository courseRepository,
             ICurrentUserService currentUserService,
             IUnitOfWork unitOfWork,
             ICourseAuthorizationService courseAuthorizationService)
        {
            _courseRepository = courseRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _courseAuthorizationService = courseAuthorizationService;
        }
        public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdWithSectionsAsync(request.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException(nameof(Course), request.CourseId);
            _courseAuthorizationService.EnsureCanModify(course);
            var deletedBy = _currentUserService.UserId;

            course.DeleteSection(
                request.SectionId,
                deletedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}
