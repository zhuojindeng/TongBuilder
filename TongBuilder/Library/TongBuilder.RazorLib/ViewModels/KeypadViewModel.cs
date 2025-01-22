using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Blazing.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;



namespace TongBuilder.RazorLib.ViewModels
{
    public class KeypadViewModel : ViewModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _inputString = "";
        private string _displayText = "";
        private char[] _specialChars = { '*', '#' };

        public ICommand AddCharCommand { get; private set; }
        public ICommand DeleteCharCommand { get; private set; }

        public string InputString
        {
            get => _inputString;
            private set
            {
                if (_inputString != value)
                {
                    _inputString = value;
                    OnPropertyChanged();
                    DisplayText = FormatText(_inputString);

                    // Perhaps the delete button must be enabled/disabled.
                    ((RelayCommand)DeleteCharCommand).NotifyCanExecuteChanged();
                }
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                if (_displayText != value)
                {
                    _displayText = value;
                    OnPropertyChanged();
                }
            }
        }

        public KeypadViewModel()
        {
            // Command to add the key to the input string
            AddCharCommand = new RelayCommand<string>((key) => InputString += key);

            // Command to delete a character from the input string when allowed
            DeleteCharCommand =
                new RelayCommand(
                    // Command strips a character from the input string
                    () => InputString = InputString.Substring(0, InputString.Length - 1),

                    // CanExecute is processed here to return true when there's something to delete
                    () => InputString.Length > 0
                );
        }

        string FormatText(string str)
        {
            bool hasNonNumbers = str.IndexOfAny(_specialChars) != -1;
            string formatted = str;

            // Format the string based on the type of data and the length
            if (hasNonNumbers || str.Length < 4 || str.Length > 10)
            {
                // Special characters exist, or the string is too small or large for special formatting
                // Do nothing
            }

            else if (str.Length < 8)
                formatted = string.Format("{0}-{1}", str.Substring(0, 3), str.Substring(3));

            else
                formatted = string.Format("({0}) {1}-{2}", str.Substring(0, 3), str.Substring(3, 3), str.Substring(6));

            return formatted;
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
