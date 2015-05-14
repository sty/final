using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator
{
    public class CalculatorVM: BaseVM
    {
        Operation _Op = new BinaryOperation();
        public Operation Op
        {
            get { return _Op; }
            set { _Op = value; OnPropertyChanged(); }
        }
        ObservableCollection<Operation> _Operations = new ObservableCollection<Operation>();
        public ObservableCollection<Operation> Operations
        {
            get { return _Operations; }
            set { _Operations = value; OnPropertyChanged(); }
        }

        DelegateCommand _Clear;
        public ICommand Clear
        {
            get
            {
                if (_Clear == null) {
                    _Clear = new DelegateCommand {
                        ExecuteFunction = x => {
                            Operations.Clear();
                            Op = new BinaryOperation { };
                        },
                        CanExecuteFunction = x => Operations.Any()
                    };
                    _Operations.CollectionChanged += (s, e) => _Clear.OnCanExecuteChanged();
                }
                return _Clear;
            }
        }

        public DelegateCommand DisplayFactors
        {
            get
            {
                return new DelegateCommand {
                    ExecuteFunction = x =>{
                        var t = Task.Run(() => string.Join(",", GetFactors((int)Op.PreviousTotal).ToArray()));
                        t.ContinueWith(t2 => System.Windows.MessageBox.Show(t2.Result));
                        
                        System.Windows.MessageBox.Show("Next");
                    }
                };
            }
        }

        static IEnumerable<int> GetFactors(int n)
        {
            return from a in Enumerable.Range(1, n)
                   where n % a == 0
                   select a;
        }

    }
}
