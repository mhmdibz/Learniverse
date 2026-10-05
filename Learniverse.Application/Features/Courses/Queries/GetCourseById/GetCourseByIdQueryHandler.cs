using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryHandler
        : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResponse>
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseByIdQueryHandler(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task<GetCourseByIdResponse> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetPublishedByIdAsync(request.Id, cancellationToken);
            if (course is null)
            {
                throw new NotFoundException(nameof(Course), request.Id);
            }
            return new GetCourseByIdResponse(
                       course.Id,
                       course.Title,
                       course.Description,
                       course.Price,
                       course.Status,
                       course.InstructorId,
                       course.CategoryId,
                       course.Category.Name,
                       course.CreatedAtUtc,
                       course.UpdatedAtUtc);
        }
    }
}
