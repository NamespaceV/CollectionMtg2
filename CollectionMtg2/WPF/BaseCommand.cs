namespace CollectionMtg2.WPF
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class BaseCommand : ICommand
    {
        public Func<Task> ExecuteImpl { get; }
        public Func<bool> CanExecuteImpl { get; }

        public BaseCommand(Func<Task> execute)
            : this(execute, () => true)
        {
        }

        public BaseCommand(
            Func<Task> execute,
            Func<bool> canExecute)
        {
            ExecuteImpl = execute;
            CanExecuteImpl = canExecute;
        }

        public void CheckUpdate()
        {
            CanExecuteChanged.Invoke(this, null);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteImpl();
        }

        public async void Execute(object parameter)
        {
            try
            {
                await ExecuteAsync();
            }
            catch
            {

            }
        }

        public Task ExecuteAsync()
        {
            return ExecuteImpl();
        }
    }
}
