using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Commands.UpdateCourse
{
    public sealed class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseAuthorizationService _courseAuthorizationService;

        public UpdateCourseCommandHandler(ICourseRepository courseRepository,
             ICourseAuthorizationService courseAuthorizationService,
             IUnitOfWork unitOfWork)
        {
            _courseRepository = courseRepository;
            _courseAuthorizationService = courseAuthorizationService;
            _unitOfWork = unitOfWork;

        }
        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);
            if (course is null)
            {
                throw new NotFoundException(
                            nameof(Course),
                            request.Id);
            }
            _courseAuthorizationService.EnsureCanModify(course);

            course.UpdateDetails(
                request.Title,
                request.Description,
                request.Price,
                request.Status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
