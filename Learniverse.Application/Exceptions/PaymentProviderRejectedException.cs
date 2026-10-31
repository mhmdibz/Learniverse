using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Exceptions
{
    public sealed class PaymentProviderRejectedException : Exception
    {
        public PaymentProviderRejectedException()
            : base("The payment provider rejected the payment request.")
        {
        }
    }
}
