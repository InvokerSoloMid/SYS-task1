using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using task1.Infrasrtucture;

namespace task1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string fileLocation;
        private string text;
        private Encoding currenEncoding;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public Encoding CurrentEncoding
        {
            get
            {
                return currenEncoding;
            }
            set
            {
                currenEncoding = value;
                OnPropertyChanged(nameof(CurrentEncoding));
                OnPropertyChanged(nameof(IsAscii));
                OnPropertyChanged(nameof(IsUnicode));
            }
        }

        public bool IsAscii => CurrentEncoding == Encoding.ASCII;
        public bool IsUnicode => !IsAscii;

        public ICommand OpenFileCommand { get; private set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand ChangeCodingCommnad { get; private set; }

        public MainViewModel()
        {
            CurrentEncoding = Encoding.ASCII;
            OpenFileCommand = new Command(OpenFile);
            SaveFileCommand = new Command(SaveFile);
            ChangeCodingCommnad = new Command(ChangeCoding);
        }

        private async void OpenFile(object parameter)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "TXT(*.TXT)|*.txt";
            if (ofd.ShowDialog() ?? true)
            {
                fileLocation = ofd.FileName;
                using (var fileStr = new StreamReader(fileLocation, CurrentEncoding))
                {
                    Text = await fileStr.ReadToEndAsync();
                }
            }
        }

        private async void SaveFile(object parameter)
        {
            using (var fileStr = new StreamWriter(File.Open(fileLocation, FileMode.Open), CurrentEncoding))
            {
                await fileStr.WriteAsync(Text);
            }
        }

        private async void ChangeCoding(object parameter)
        {
            string type = (string)parameter;
            switch (type)
            {
                case "Ascii":
                    {
                        CurrentEncoding = Encoding.ASCII;
                        if (!string.IsNullOrEmpty(fileLocation))
                        {
                            using (var fileStr = new StreamReader(fileLocation, CurrentEncoding))
                            {
                                Text = await fileStr.ReadToEndAsync();
                            }
                        }

                        break;
                    }
                case "Unicode":
                    {
                        CurrentEncoding = new UTF8Encoding(false);
                        if (!string.IsNullOrEmpty(fileLocation))
                        {
                            using (var fileStr = new StreamReader(fileLocation, CurrentEncoding))
                            {
                                Text = await fileStr.ReadToEndAsync();
                            }
                        }

                        break;
                    }
                default:
                    ChangeCoding("Unicode");
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
