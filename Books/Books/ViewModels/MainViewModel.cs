using Books.Commands;
using Books.Helpers;
using BooksApiClient;
using BooksApiClient.Dto;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Books.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        readonly RestClient client;
        private ObservableCollection<BookViewModel> _Books = new();
        public ObservableCollection<BookViewModel> Books
        {
            get { return _Books; }
            set { if (_Books != value) { _Books = value; OnPropertyChanged(); } }
        }
        private CustomCommand _AddRandomBookCommand;
        public CustomCommand AddRandomBookCommand
        {
            get { return _AddRandomBookCommand; }
            set { if (_AddRandomBookCommand != value) { _AddRandomBookCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _AddBookCommand;
        public CustomCommand AddBookCommand
        {
            get { return _AddBookCommand; }
            set { if (_AddBookCommand != value) { _AddBookCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _EditBookCommand;
        public CustomCommand EditBookCommand
        {
            get { return _EditBookCommand; }
            set { if (_EditBookCommand != value) { _EditBookCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _DeleteBookCommand;
        public CustomCommand DeleteBookCommand
        {
            get { return _DeleteBookCommand; }
            set { if (_DeleteBookCommand != value) { _DeleteBookCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _BackCommand;
        public CustomCommand BackCommand
        {
            get { return _BackCommand; }
            set { if (_BackCommand != value) { _BackCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _ForwardCommand;
        public CustomCommand ForwardCommand
        {
            get { return _ForwardCommand; }
            set { if (_ForwardCommand != value) { _ForwardCommand = value; OnPropertyChanged(); } }
        }
        private CustomCommand _LoadCommand;
        public CustomCommand LoadCommand
        {
            get { return _LoadCommand; }
            set { if (_LoadCommand != value) { _LoadCommand = value; OnPropertyChanged(); } }
        }

        private int _Page;
        public int Page
        {
            get { return _Page; }
            set { if (_Page != value) { _Page = value; UpdateVisualState(); OnPropertyChanged(); } }
        }
        private int _PagesCount;
        public int PagesCount
        {
            get { return _PagesCount; }
            set { if (_PagesCount != value) { _PagesCount = value; UpdateVisualState(); OnPropertyChanged(); } }
        }
        private string _TitleFilter;
        public string TitleFilter
        {
            get { return _TitleFilter; }
            set { if (_TitleFilter != value) { _TitleFilter = value; OnPropertyChanged(); } }
        }
        private string _AuthorFilter;
        public string AuthorFilter
        {
            get { return _AuthorFilter; }
            set { if (_AuthorFilter != value) { _AuthorFilter = value; OnPropertyChanged(); } }
        }

        private BookViewModel? _SelectedBook;
        public BookViewModel? SelectedBook
        {
            get { return _SelectedBook; }
            set { if (_SelectedBook != value) { _SelectedBook = value; UpdateVisualState(); OnPropertyChanged(); } }
        }


        public MainViewModel(RestClient client, BooksDto books)
        {
            this.client = client;
            _AddRandomBookCommand = new(OnAddRandomBook);
            _AddBookCommand = new(OnAddBook);
            _EditBookCommand = new(OnEditBook, false);
            _DeleteBookCommand = new(OnDeleteBook,false);
            _BackCommand = new(OnBack, false);
            _ForwardCommand = new(OnForward, false);
            _LoadCommand = new(OnLoad, true);
            _TitleFilter = string.Empty;
            _AuthorFilter = string.Empty;
            FillBooks(books,null);
        }
        private void UpdateVisualState()
        {
            EditBookCommand.IsCanExecute=SelectedBook!=null;
            DeleteBookCommand.IsCanExecute=SelectedBook!=null;
            BackCommand.IsCanExecute = Page > 1;
            ForwardCommand.IsCanExecute = Page < PagesCount;
        }
        private async void OnAddRandomBook()
        {
            AddRandomBookCommand.IsCanExecute = false;
            var book = RandomBookGenerator.Next();
            var response=await client.PostBookAsync(book);
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Response!=null)
            {
                AuthorFilter = string.Empty;
                TitleFilter = string.Empty;
                await BooksRequest(response.Response.Id,Page);
            }
            AddRandomBookCommand.IsCanExecute = true;
        }
        private async void OnAddBook()
        {
            AddBookCommand.IsCanExecute = false;
            var book = EditBook(new BookViewModel());
            if (book != null)
            {
                var response = await client.PostBookAsync(book.ToDto());
                if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Response != null)
                {
                    AuthorFilter = string.Empty;
                    TitleFilter = string.Empty;
                    await BooksRequest(response.Response.Id, Page);
                }
            }
            AddBookCommand.IsCanExecute = true;
            UpdateVisualState();
        }
        private async void OnEditBook()
        {
            EditBookCommand.IsCanExecute = false;
            if (SelectedBook!=null)
            {
                var book = EditBook(SelectedBook);
                if (book!=null)
                {
                    var response = await client.PutBookAsync(book.Id, book.ToDto());
                    if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Response!=null)
                    {
                        AuthorFilter = string.Empty;
                        TitleFilter = string.Empty;
                        await BooksRequest(response.Response.Id, Page);
                    }
                }
            }
            UpdateVisualState();
        }
        private static BookViewModel? EditBook(BookViewModel model)
        {
            BookEditDialog dlg = new()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataContext=new BookEditViewModel(model)
            };
            if (dlg.ShowDialog()==true)
            {
                return dlg.DataContext as BookViewModel;
            }
            return null;
        }
        private async void OnDeleteBook()
        {
            DeleteBookCommand.IsCanExecute = false;
            if (SelectedBook != null)
            {
                await client.DeleteBookAsync(SelectedBook.Id);
                await BooksRequest(null,Page);
            }
            UpdateVisualState();
        }
        private async void OnBack()
        {
            BackCommand.IsCanExecute = false;
            await BooksRequest(null, Page - 1);
            UpdateVisualState();
        }
        private async void OnForward()
        {
            ForwardCommand.IsCanExecute = false;
            await BooksRequest(null, Page + 1);
            UpdateVisualState();
        }
        private async void OnLoad()
        {
            LoadCommand.IsCanExecute = false;
            await BooksRequest(null, Page);
            LoadCommand.IsCanExecute = true;
        }
        private async Task BooksRequest(long? selectId, int pageNumber)
        {
            string? authorFilter = string.IsNullOrEmpty(AuthorFilter) ? null : AuthorFilter;
            string? titleFilter = string.IsNullOrEmpty(TitleFilter) ? null : TitleFilter;
            var response = await client.GetBooksAsync(selectId==null ? pageNumber : null, 10, selectId, titleFilter, authorFilter);
            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Response != null)
            {
                FillBooks(response.Response, selectId);
            }
        }
        void FillBooks(BooksDto books, long? selectId)
        {
            _Books.Clear();
            Page= books.Page;
            PagesCount=books.PagesCount;
            if (books.PageItems != null)
            {
                foreach (var item in books.PageItems)
                {
                    _Books.Add(new BookViewModel(item.Id,item));
                }
                if (selectId != null)
                {
                    SelectedBook = _Books.FirstOrDefault(x => x.Id == selectId.Value);
                }
                else 
                {
                    SelectedBook = null;
                }
            }
        }
    }
}
