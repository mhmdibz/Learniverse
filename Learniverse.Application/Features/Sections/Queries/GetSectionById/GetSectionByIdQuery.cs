using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById
{
    public sealed record GetSectionByIdQuery(Guid Id) : IRequest<GetSectionByIdResponse>;
}
