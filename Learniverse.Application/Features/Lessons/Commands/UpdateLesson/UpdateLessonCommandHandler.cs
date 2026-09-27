using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Commands.UpdateLesson;

public sealed class UpdateLessonCommandHandler
    : IRequestHandler<UpdateLessonCommand>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLessonCommandHandler(
        ISectionRepository sectionRepository,
        IUnitOfWork unitOfWork)
    {
        _sectionRepository = sectionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateLessonCommand request,
        CancellationToken cancellationToken)
    {
        var section = await _sectionRepository.GetByIdWithLessonsAsync(
    request.SectionId,
    cancellationToken);

        if (section is null)
            throw new NotFoundException(
                nameof(Section),
                request.SectionId);

        var lesson = section.Lessons
            .FirstOrDefault(l => l.Id == request.LessonId);

        if (lesson is null)
            throw new NotFoundException(
                nameof(Lesson),
                request.LessonId);

        lesson.UpdateDetails(
            request.Title,
            request.Description);

        lesson.ChangeContentType(
            request.ContentType);

        lesson.SetPreview(
            request.IsPreview);

        if (lesson.Order != request.Order)
        {
            section.ChangeLessonOrder(
                request.LessonId,
                request.Order);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}