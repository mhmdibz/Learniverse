using Learniverse.Application.Features.Courses.Commands.CreateCourse;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Application.Exceptions;
using Learniverse.Domain.Entities;
using MediatR;

public sealed class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCourseCommandHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (!await _categoryRepository.ExistsAsync(
        request.CategoryId,
        cancellationToken))
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var course = Course.Create(
            request.Title,
            request.Description,
            request.Price,
            _currentUserService.UserId,
            request.CategoryId);

        await _courseRepository.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return course.Id;
    }
}