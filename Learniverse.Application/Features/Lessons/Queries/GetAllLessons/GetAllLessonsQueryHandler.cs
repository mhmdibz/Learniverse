using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Lessons.Queries.GetAllLessons
{
    public sealed class GetAllLessonsQueryHandler:IRequestHandler<GetAllLessonsQuery,IReadOnlyList<GetAllLessonsResponse>>
    {
        private readonly ILessonRepository _lessonRepository;
        public GetAllLessonsQueryHandler(ILessonRepository lessonRepository) { 
        _lessonRepository = lessonRepository;
        }
        public async Task<IReadOnlyList<GetAllLessonsResponse>> Handle(
        GetAllLessonsQuery request,
        CancellationToken cancellationToken)
        {
            var lessons = await _lessonRepository.GetAllAsync(
                cancellationToken);

            return lessons
                .Select(l => new GetAllLessonsResponse(
                    l.Id,
                    l.Title,
                    l.Description,
                    l.Order,
                    l.ContentType,
                    l.SectionId,
                    l.IsPreview))
                .ToList();
        }
    }
}
