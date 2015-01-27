using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneApp2.Models
{
    public class Invoice
    {
        public decimal Amount { get; set; }

        public bool IsPaid { get; private set; }

        public Invoice()
        {
            IsPaid = false;
        }

        public void Pay()
        {
            IsPaid = true;
        }

        internal void CancelPay()
        {
            IsPaid = false;
        }
    }
}
