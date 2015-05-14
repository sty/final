using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CheckBook
{
    public class Account
    {
        public Account()
        {
            Transactions = new ObservableCollection<Transaction>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Institution { get; set; }
        public bool Business { get; set; }

        public virtual ObservableCollection<Transaction> Transactions { get; set; }
    }
}
