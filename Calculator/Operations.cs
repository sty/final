using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator
{
    public class Operation: BaseVM
    {
        private string _Operator = "+";
        public string Operator
        {
            get { return _Operator; }
            set { _Operator = value; OnPropertyChanged(); OnPropertyChanged("DisplayText"); }
        }

        private float _PreviousTotal;
        public float PreviousTotal
        {
            get { return _PreviousTotal; }
            set { _PreviousTotal = value; OnPropertyChanged(); OnPropertyChanged("DisplayText"); }
        }

        public virtual string DisplayText
        {
            get
            {
                return Results.ToString();
            }
        }

        public virtual ICommand Number
        {
            get { return new DeadCommand(); }
        }
        public virtual ICommand BackSpace
        {
            get { return new DeadCommand(); }
        }
        
        public virtual float Results
        {
            get
            {
                switch (Operator)
                {
                    case "sq":
                        return PreviousTotal * PreviousTotal;
                    case "sqrt":
                        return (float) Math.Sqrt( (double)PreviousTotal );
                    case "+/-":
                        return -PreviousTotal;
                    case "=":
                        return PreviousTotal;
                }
                return PreviousTotal;
            }
        }

        public override string ToString()
        {
            return Operator.ToString() + " " + PreviousTotal.ToString();
        }
    }

    public class BinaryOperation : Operation
    {
        public override ICommand BackSpace
        {
            get { return new BackSpaceCommand(this); }
        }

        public override ICommand Number
        {
            get
            {
                return new DelegateCommand {
                    ExecuteFunction = p => StrOperand += p
                };
            }
        }

        public override string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(StrOperand))
                    return PreviousTotal.ToString();
                else
                    return StrOperand;
            }
        }

        private string _StrOperand;
        public string StrOperand
        {
            get { return _StrOperand; }
            set {
                _StrOperand = value;
                OnPropertyChanged();
                OnPropertyChanged("Operand");
                OnPropertyChanged("Results");
                OnPropertyChanged("DisplayText");
            }
        }

        private float _Operand;
        public float Operand
        {
            get
            {
                if (!float.TryParse(StrOperand, out _Operand))
                {
                    return 0;
                }
                return _Operand;
            }
        }
        public override float Results
        {
            get
            {
                switch (Operator)
                {
                    case "+":
                        return PreviousTotal + Operand;
                    case "-":
                        return PreviousTotal - Operand;
                    case "x":
                        return PreviousTotal * Operand;
                    case "/":
                        return PreviousTotal / Operand;
                }
                return PreviousTotal;
            }
        }
        public override string ToString()
        {
            return Operator.ToString() + " " + Operand.ToString();
        }
    }

    public class BaseVM: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class BackSpaceCommand : ICommand
    {
        public BackSpaceCommand(BinaryOperation op)
        {
            _Op = op;
            _Op.PropertyChanged += (s, e) => {
                    if (CanExecuteChanged != null) {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
            };
        }

        private BinaryOperation _Op;

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_Op.StrOperand);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _Op.StrOperand = _Op.StrOperand.Substring(0, _Op.StrOperand.Length - 1);
        }
    }

    public class DelegateCommand: ICommand
    {

        public Predicate<object> CanExecuteFunction { get; set; }
        public Action<object> ExecuteFunction { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunction != null)
                return CanExecuteFunction(parameter);
            else
                return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;  }
        }
        public void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void Execute(object parameter)
        {
            if (ExecuteFunction != null) ExecuteFunction(parameter);
        }
    }
    public class DeadCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }
    }

}
