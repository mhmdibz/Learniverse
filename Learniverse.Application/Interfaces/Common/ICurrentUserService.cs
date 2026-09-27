using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Interfaces.Common;

public interface ICurrentUserService
{
    string UserId { get; }
    IReadOnlyList<string> Roles { get; }

}