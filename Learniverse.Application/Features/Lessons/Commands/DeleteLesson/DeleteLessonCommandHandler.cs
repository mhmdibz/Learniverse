using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Lessons.Commands.DeleteLesson
{
    public sealed class DeleteLessonCommandHandler:IRequestHandler<DeleteLessonCommand>
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public DeleteLessonCommandHandler(
       ISectionRepository sectionRepository,
       IUnitOfWork unitOfWork,
       ICurrentUserService currentUserService)
        {
            _sectionRepository = sectionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken) { 
        var section = await _sectionRepository.GetByIdWithLessonsAsync(request.SectionId,cancellationToken);
            if (section == null) 
                throw new NotFoundException(nameof(Section),request.SectionId);
            var deletedBy = _currentUserService.UserId;

            section.DeleteLesson(
                request.LessonId,
                deletedBy);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
