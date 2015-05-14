using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.CheckBook
{
    public class CheckBookVM : BaseVM
    {
        public CheckBookVM()
        {
        }

        CbDb _Db = new CbDb();

        private int _RowsPerPage = 5;
        private int _CurrentPage = 1;
        public int CurrentPage
        {
            get { return _CurrentPage; }
            set { _CurrentPage = value; OnPropertyChanged(); OnPropertyChanged("CurrentTransactions"); }
        }

        public ICollection<Transaction> Transactions
        {
            get {
                if (CurrentAccount == null) {
                    return new ObservableCollection<Transaction>();
                }
                return CurrentAccount.Transactions;
            }
        }

        private ObservableCollection<Account> _Accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return _Accounts; }
            set { _Accounts = value; OnPropertyChanged(); }
        }

        private Account _CurrentAccount;
        public Account CurrentAccount
        {
            get { return _CurrentAccount; }
            set { _CurrentAccount = value; OnPropertyChanged(); OnPropertyChanged("CurrentTransactions"); }
        }
        

        public IEnumerable<Transaction> CurrentTransactions
        {
            get { return Transactions.Skip((_CurrentPage - 1) * _RowsPerPage).Take(_RowsPerPage); }
        }

        private Transaction _CurrentTransaction;
        public Transaction CurrentTransaction
        {
            get { return _CurrentTransaction; }
            set { _CurrentTransaction = value; OnPropertyChanged(); }
        }
        


        public DelegateCommand MoveNext
        {
            get
            {
                return new DelegateCommand {
                    ExecuteFunction = _ => CurrentPage++,
                    CanExecuteFunction = _ => CurrentPage * _RowsPerPage < Transactions.Count
                };
            }
        }

        public DelegateCommand Save
        {
            get
            {
                return new DelegateCommand {
                    ExecuteFunction = _ => _Db.SaveChanges(),
                    CanExecuteFunction = _ => _Db.ChangeTracker.HasChanges()
                };
            }
        }

        public DelegateCommand NewTransaction
        {
            get
            {
                return new DelegateCommand {
                    ExecuteFunction = _ => {
                        Transactions.Add(new Transaction { Date = DateTime.Now });
                        _Db.SaveChanges();
                        CurrentPage = Transactions.Count / _RowsPerPage + 1;
                    }
                };
            }
        }

        private Rates _CurrentRates;

        public Rates CurrentRates
        {
            get { return _CurrentRates; }
            set { _CurrentRates = value; OnPropertyChanged(); }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; OnPropertyChanged(); }
        }
        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { _Email = value; OnPropertyChanged(); }
        }
        private string _Picture;

        public string Picture
        {
            get { return _Picture; }
            set { _Picture = value; OnPropertyChanged(); }
        }


        public async void Fill()
        {

            _Db.Accounts.ToListAsync();
            _Db.Transactions.ToListAsync();

            var wnd = new LoginWindow();
            var token = await wnd.Login();

            var http = new HttpClient();

            dynamic me = Newtonsoft.Json.JsonConvert.DeserializeObject(await http.GetStringAsync("https://graph.facebook.com/me?access_token=" + token));

            Name = me.name;
            Email = me.email;
            Picture = "https://graph.facebook.com/" + me.id + "/picture";

            Accounts = _Db.Accounts.Local;

            var results = await http.GetAsync("http://openexchangerates.org/api/latest.json?app_id=2f23629162b444b580bc03970c41caad");
            var currencies = await results.Content.ReadAsAsync<ExchageRate>();
            CurrentRates = currencies.rates;
            ExchangeRateSing.Instance.Rates = currencies.rates;
            OnPropertyChanged("CurrentTransactions"); OnPropertyChanged("Transactions");
        }
    }
}
