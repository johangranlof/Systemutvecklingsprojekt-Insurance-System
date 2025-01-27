using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class HandleUserVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly UserController _userController;

        public HandleUserVM(MainVM mainViewModel)
        {
            _userController = new UserController();
            _mainViewModel = mainViewModel;
            Users = new ObservableCollection<User>();
            LoadUsers();
        }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private void LoadUsers()
        {
            _userController.GetUsers().ForEach(Users.Add);
        }

        private ICommand _addUserViewCommand;
        public ICommand AddUserViewCommand => _addUserViewCommand ??= new RelayCommand(() =>
        {
            var addUserVM = new AddUserVM(_mainViewModel);
            var addUserUC = new AddUserUC { DataContext = addUserVM };
            _mainViewModel.CurrentView = addUserUC;
        });
    }
}
