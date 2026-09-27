using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById
{
    public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse>
    {
        private readonly ISectionRepository _sectionRepository;
        public GetSectionByIdQueryHandler(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }
        public async Task<GetSectionByIdResponse> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            var section = await _sectionRepository.GetByIdAsync(request.Id, cancellationToken);
            if (section is null)
                throw new NotFoundException(nameof(Section), request.Id);

            return new GetSectionByIdResponse(
                        section.Id,
                        section.Title,
                        section.Description,
                        section.Order,
                        section.CourseId);
        }
    }
}
