using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Application.Services;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Commands.DeleteCourse
{
    public sealed class DeleteCourseCommandHandler
    : IRequestHandler<DeleteCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICourseAuthorizationService _courseAuthorizationService;
        private readonly ICacheService _cacheService;

        public DeleteCourseCommandHandler(ICourseRepository courseRepository,
            IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ICourseAuthorizationService courseAuthorizationService, ICacheService cacheService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _courseAuthorizationService = courseAuthorizationService;
            _cacheService = cacheService;
        }
        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdWithSectionsAsync(request.Id, cancellationToken);
            if (course is null)
                throw new NotFoundException(nameof(Course), request.Id);
            _courseAuthorizationService.EnsureCanModify(course);
            course.DeleteWithSections(_currentUserService.UserId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cacheService.InvalidateTagAsync(
                CourseCacheKeys.PublicListTag,
                cancellationToken);
        }

    }
}
