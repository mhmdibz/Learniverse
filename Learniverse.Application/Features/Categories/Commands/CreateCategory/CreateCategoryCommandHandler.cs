using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _categoryRepository.ExistsByNameAsync(
            request.Name,
            cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                $"Category '{request.Name}' already exists.");
        }

        var category = Category.Create(request.Name);

        await _categoryRepository.AddAsync(category, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}