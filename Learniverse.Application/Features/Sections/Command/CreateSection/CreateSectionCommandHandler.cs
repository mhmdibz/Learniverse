using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.CreateSection;

public sealed class CreateSectionCommandHandler :
    IRequestHandler<CreateSectionCommand, Guid>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseAuthorizationService _courseAuthorizationService;

    public CreateSectionCommandHandler(
      ICourseRepository courseRepository,
      IUnitOfWork unitOfWork,
      ICourseAuthorizationService courseAuthorizationService)
    {
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _courseAuthorizationService = courseAuthorizationService;
    }

    public async Task<Guid> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdWithSectionsAsync(
            request.CourseId,
            cancellationToken);
        if (course is null)
            throw new NotFoundException(
                nameof(Course),
                request.CourseId);
        _courseAuthorizationService.EnsureCanModify(course);

        var section = course.AddSection(
           request.Title,
           request.Description,
           request.Order);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return section.Id;

    }
}

