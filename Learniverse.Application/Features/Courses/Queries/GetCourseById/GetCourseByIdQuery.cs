using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById
{

        public sealed record GetCourseByIdQuery(Guid Id)
            : IRequest<GetCourseByIdResponse>;
    
}
