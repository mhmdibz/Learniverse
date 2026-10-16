using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Domain.Enum;

    public enum PaymentStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Expired = 4,
        Refunded = 5
    }

