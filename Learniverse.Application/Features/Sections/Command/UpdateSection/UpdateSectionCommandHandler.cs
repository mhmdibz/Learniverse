using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.UpdateSection
{
    public sealed class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseRepository _courseRepository;
        private readonly ICourseAuthorizationService _courseAuthorizationService;
        public UpdateSectionCommandHandler(ICourseRepository courseRepository, IUnitOfWork unitOfWork, ICourseAuthorizationService courseAuthorizationService)
        {
            _courseRepository = courseRepository;
            _unitOfWork = unitOfWork;
            _courseAuthorizationService = courseAuthorizationService;
        }
        public async Task Handle(
      UpdateSectionCommand request,
      CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdWithSectionsAsync(
                request.CourseId,
                cancellationToken);

            if (course is null)
                throw new NotFoundException(
                    nameof(Course),
                    request.CourseId);

            var section = course.Sections
                .FirstOrDefault(s => s.Id == request.SectionId);

            if (section is null)
                throw new NotFoundException(
                    nameof(Section),
                    request.SectionId);
            _courseAuthorizationService.EnsureCanModify(course);
            section.UpdateDetails(
                request.Title,
                request.Description);

            if (section.Order != request.Order)
            {
                course.ChangeSectionOrder(
                    request.SectionId,
                    request.Order);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
