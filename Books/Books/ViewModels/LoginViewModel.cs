using Books.Commands;
using Books.Models;
using BooksApiClient;
using System.Windows;

namespace Books.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
		private string _Server;
		public string Server
		{
			get { return _Server; }
			set { if (_Server != value) { _Server = value; OnPropertyChanged(); } }
		}

		private CustomCommand _ConnectCommand;
		public CustomCommand ConnectCommand
		{
			get { return _ConnectCommand; }
			set { if (_ConnectCommand != value) { _ConnectCommand = value; OnPropertyChanged(); } }
		}

		public LoginViewModel()
		{
			LoginModel lm = LoginModel.Load();
			_Server = lm.Server;
			_ConnectCommand = new(OnConnect);
		}
		private async void OnConnect()
		{
			LoginModel lm = new()
			{
				Server = Server
			};
			lm.Save();
			ConnectCommand.IsCanExecute = false;
			var client = new RestClient(lm.Server);
			var books=await client.GetBooksAsync(1, 10, null, null, null);
			if (books.StatusCode == System.Net.HttpStatusCode.OK && books.Response!=null)
			{
				var loginWindow = Application.Current.MainWindow;
				Application.Current.MainWindow = new MainWindow
				{
					DataContext = new MainViewModel(client,books.Response)
				};
				Application.Current.MainWindow.Show();
				loginWindow.Close();
			}
			ConnectCommand.IsCanExecute = true;
        }
	}
}
