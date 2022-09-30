using Books.Commands;
using System;
using System.Windows;

namespace Books.ViewModels
{
    public class BookEditViewModel : BookViewModel
    {
        private CustomCommand _LoadImageCommand;
        public CustomCommand LoadImageCommand
        {
            get { return _LoadImageCommand; }
            set { if (_LoadImageCommand != value) { _LoadImageCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _AcceptCommand;
        public CustomCommand AcceptCommand
        {
            get { return _AcceptCommand; }
            set { if (_AcceptCommand != value) { _AcceptCommand = value; OnPropertyChanged(); } }
        }

        public BookEditViewModel(BookViewModel model) : base(model.Id,model.ToDto())
        {
            _LoadImageCommand = new(OnLoadImage, true);
            _AcceptCommand = new(OnAccept, true);
        }
        string? OpenFileDialog()
        {
            Microsoft.Win32.OpenFileDialog ofd = new()
            {
                Filter = "Image Files(*.JPG;*.PNG;)|*.JPG;*.PNG|All files (*.*)|*.*"
            };
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (ofd.ShowDialog() == true)
            {
                return ofd.FileName;

            }
            return null;
        }
        void OnLoadImage()
        {
            string? path = OpenFileDialog();
            if (path!=null)
            {
                try
                {
                    Image = System.IO.File.ReadAllBytes(path);
                }
                catch
                {

                }
            }
        }
        void OnAccept(object? o)
        {
            if (o is Window w)
            {
                w.DialogResult = true;
            }
        }
    }
}
