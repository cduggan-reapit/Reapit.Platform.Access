using FluentValidation;
using FluentValidation.Results;
using Reapit.Platform.Common.Exceptions;
using Reapit.Platform.Access.Core.UseCases.Dummies.DeleteDummyById;
using Reapit.Platform.Access.Data.Repositories;
using Reapit.Platform.Access.Data.Services;
using Reapit.Platform.Access.Domain.Entities;

namespace Reapit.Platform.Access.Core.UnitTests.UseCases.Dummies.DeleteDummyById;

public class DeleteDummyByIdCommandHandlerTests
{
    private readonly IValidator<DeleteDummyByIdCommand> _validator = Substitute.For<IValidator<DeleteDummyByIdCommand>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IDummyRepository _dummyRepository = Substitute.For<IDummyRepository>();

    [Fact]
    public async Task Handle_ThrowsValidationException_WhenValidationFails()
    {
        var query = GetQuery(Guid.NewGuid());
        _validator.ValidateAsync(Arg.Any<DeleteDummyByIdCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("propertyName", "errorMessage") }));

        var sut = CreateSut();
        var action = () => sut.Handle(query, default);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_ThrowsNotFoundException_WhenObjectNotFound()
    {
        var query = GetQuery(Guid.NewGuid());
        
        _validator.ValidateAsync(Arg.Any<DeleteDummyByIdCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _dummyRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(null as Dummy);

        var sut = CreateSut();
        var action = () => sut.Handle(query, default);
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_DoesNotThrow_WhenDummyDeleted()
    {
        var id = Guid.NewGuid();
        var query = GetQuery(id);
        var expected = new Dummy("test-dummy") { Id = id };
        
        _validator.ValidateAsync(Arg.Any<DeleteDummyByIdCommand>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _dummyRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        var sut = CreateSut();
        var actual = () => sut.Handle(query, default);
        await actual.Should().NotThrowAsync();

        // And it deleted it!
        await _dummyRepository.Received(1).DeleteAsync(expected, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /*
     * Private methods
     */

    private DeleteDummyByIdCommandHandler CreateSut()
    {
        _unitOfWork.Dummies.Returns(_dummyRepository);
        return new DeleteDummyByIdCommandHandler(_validator, _unitOfWork);
    }

    private static DeleteDummyByIdCommand GetQuery(Guid id)
        => new($"{id:N}");
}