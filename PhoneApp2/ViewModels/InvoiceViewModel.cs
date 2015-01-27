using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PhoneApp2.Commands;
using PhoneApp2.Models;

namespace PhoneApp2.ViewModels
{
    public class InvoiceViewModel : INotifyPropertyChanged
    {
        private Invoice _invoice;

        public InvoiceViewModel(Invoice invoice)
        {
            _invoice = invoice;
            _amount = _invoice.Amount;
            _isPaid = _invoice.IsPaid;

            PayCommand = new PayCommand(() => IsPaid = true, () => !IsPaid);
            CancelPayCommand = new CancelPayCommand(() => IsPaid = false, () => IsPaid);
        }

        public CancelPayCommand CancelPayCommand { get; set; }

        public PayCommand PayCommand { get; set; }

        private decimal _amount;
        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                if (_amount == value)
                {
                    return;
                }

                _amount = value;

                // Todo: notify to view to do something has changed
                OnPropertyChanged("Amount");
            }
        }

        private bool _isPaid;

        public bool IsPaid
        {
            get
            {
                return _isPaid;
            }

            set
            {
                if (_isPaid == value)
                {
                    return;
                }

                _isPaid = value;

                // Todo: notify to view to do something has changed
                OnPropertyChanged("IsPaid");
            }
        }

        public void Pay()
        {
            _invoice.Pay();
        }

        public void CancelPay()
        {
            _invoice.CancelPay();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
