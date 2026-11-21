using System;
using System.Threading;
using System.Threading.Tasks;

namespace Learniverse.Application.Interfaces.Common;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        string tag,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan expiration,
        CancellationToken cancellationToken);

    Task InvalidateTagAsync(
        string tag,
        CancellationToken cancellationToken);
}
