using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using System;
using Learniverse.Domain.Entities;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Lessons.Commands.CreateLesson
{
    public sealed class CreateLessonCommandHandler : IRequestHandler<CreateLessonCommand, Guid>
    {
        public readonly ISectionRepository _sectionRepository;
        public readonly IUnitOfWork _unitOfWork;

        public CreateLessonCommandHandler(ISectionRepository sectionRepository, IUnitOfWork unitOfWork)
        {
            _sectionRepository = sectionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
        {
            var section = await _sectionRepository.GetByIdAsync(request.SectionId, cancellationToken);
            if (section == null)
                throw new NotFoundException(nameof(Section), request.SectionId);
            var lesson = section.AddLesson(request.Title, request.Description, request.Order, request.ContentType, request.IsPreview);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return lesson.Id;
        }

    }
}
