using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Exceptions
{
    public sealed class PaymentOutcomeUnknownException : Exception
    {
        public PaymentOutcomeUnknownException()
            : base("The payment status is unknown. Do not start another attempt yet.")
        {
        }
    }
}
