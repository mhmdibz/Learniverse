using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAtUtc { get; private set; }
        public string? DeletedBy { get; private set; }
        public void Delete(string deletedBy)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAtUtc = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }
    }
}