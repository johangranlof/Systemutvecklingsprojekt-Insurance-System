using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class ProspectInformationVM : BaseVM, IDataErrorInfo
    {
        private readonly ViewPrivateCustomerVM _viewPrivateCustomerVM;
        private readonly ViewBusinessCustomerVM _viewBusinessCustomerVM;
        private readonly Window _popupWindow;
        private bool _isSaveAttempted = false;

        public ProspectInformationVM(ViewPrivateCustomerVM viewPrivateCustomerVM, Window popupWindow)
        {
            _viewPrivateCustomerVM = viewPrivateCustomerVM;
            _popupWindow = popupWindow;
        }

        public ProspectInformationVM(ViewBusinessCustomerVM viewBusinessCustomerVM, Window popupWindow)
        {
            _viewBusinessCustomerVM = viewBusinessCustomerVM;
            _popupWindow = popupWindow;
        }

        private string _note;
        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        private bool _isSnackbarActive;
        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set
            {
                _isSnackbarActive = value;
                OnPropertyChanged(nameof(IsSnackbarActive));
            }
        }

        private string _snackbarMessage;
        public string SnackbarMessage
        {
            get => _snackbarMessage;
            set
            {
                _snackbarMessage = value;
                OnPropertyChanged(nameof(SnackbarMessage));
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(async () =>
        {
            _isSaveAttempted = true;
            OnPropertyChanged(nameof(Note));

            if (!string.IsNullOrWhiteSpace(Error))
            {
                SnackbarMessage = Error;
                IsSnackbarActive = true;
                return;
            }
            if (_viewPrivateCustomerVM != null)
            {
                _viewPrivateCustomerVM.SaveProspectInformation(Note);
            }

            else if (_viewBusinessCustomerVM != null)
            {
                _viewBusinessCustomerVM.SaveProspectInformation(Note);
            }


            Note = string.Empty;


            SnackbarMessage = "Information sparad!";
            IsSnackbarActive = true;
            _popupWindow?.Close();
        });

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(() =>
        {
            _popupWindow?.Close();
        });


        #region IDataErrorInfo Implementation

        public string Error => this[nameof(Note)];

        public string this[string columnName]
        {
            get
            {
                if (!_isSaveAttempted) return string.Empty;

                return columnName switch
                {
                    nameof(Note) when string.IsNullOrWhiteSpace(Note) => "Kommentera utfallet innan du sparar.",
                    _ => string.Empty
                };
            }
        }

        #endregion
    }
}
