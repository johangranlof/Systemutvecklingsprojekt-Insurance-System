using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace Presentation.Helpers
{
    public class MenuObject : ObservableObject
    {
        public string Name { get; set; }
        public ICommand CommandName { get; set; }
        private bool _isChecked;
        private PackIconKind _icon;
        public PackIconKind Icon
        {
            get => _icon;
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
        public MenuObject(string name, ICommand command, PackIconKind icon)
        {
            Name = name;
            CommandName = command;
            Icon = icon;
        }
    }
}
