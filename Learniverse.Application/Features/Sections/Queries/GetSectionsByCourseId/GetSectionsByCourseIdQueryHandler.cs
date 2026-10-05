using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Categories.Queries.GetAllCategories;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId
{
    public sealed class GetSectionsByCourseIdQueryHandler : IRequestHandler<GetSectionsByCourseIdQuery, IReadOnlyList<GetSectionsByCourseIdResponse>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;

        public GetSectionsByCourseIdQueryHandler(ICourseRepository courseRepository, ISectionRepository sectionRepository)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
        }

        public async Task<IReadOnlyList<GetSectionsByCourseIdResponse>> Handle(GetSectionsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetPublishedByIdAsync(
      request.CourseId,
      cancellationToken);

            if (course is null)
                throw new NotFoundException(
                    nameof(Course),
                    request.CourseId);
            var sections = await _sectionRepository.GetByCourseIdAsync(request.CourseId, cancellationToken);
            return sections
           .Select(s => new GetSectionsByCourseIdResponse(
               s.Id,
               s.Title,
               s.Description,
               s.Order,
               s.CourseId))
           .ToList();
        }
    }
}
