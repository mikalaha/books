using BooksApiClient.Dto;
using System;
using System.Linq;

namespace Books.ViewModels
{
    public class BookViewModel : BaseViewModel
    {
		public long Id { get; }
		private string _Title;
		public string Title
		{
			get { return _Title; }
			set { if (_Title != value) { _Title = value; OnPropertyChanged(); } }
		}
		private string _Author;
		public string Author
		{
			get { return _Author; }
			set { if (_Author != value) { _Author = value; OnPropertyChanged(); } }
		}
		private int _Year;
		public int Year
		{
			get { return _Year; }
			set { if (_Year != value) { _Year = value; OnPropertyChanged(); } }
		}
		private string _ISBN;
		public string ISBN
		{
			get { return _ISBN; }
			set { if (_ISBN != value) { _ISBN = value; OnPropertyChanged(); } }
		}
		private string _Description;
		public string Description
		{
			get { return _Description; }
			set { if (_Description != value) { _Description = value; OnPropertyChanged(); } }
		}
		private byte[] _Image;
		public byte[] Image
		{
			get { return _Image; }
			set { if (_Image != value) { _Image = value; OnPropertyChanged(); } }
		}
		public BookViewModel()
		{
			_Title = "";
			_Author = "";
			_ISBN = "";
			_Description = "";
			_Image = Array.Empty<byte>();
		}
		public BookViewModel(long id, BookInformationDto book)
		{
			Id = id;
			_Title = book.Title ?? string.Empty;
			_Author = book.Author ?? string.Empty;
			_Year = book.Year ?? 0;
			_ISBN = book.ISBN ?? string.Empty;
			_Description = book.Description ?? string.Empty;
			_Image = book.Image ?? Array.Empty<byte>();
		}
		public BookInformationDto ToDto()
		{
			return new BookInformationDto()
			{
				Author = Author,
				ISBN = ISBN,
				Description = Description,
				Image = Image.ToArray(),
				Title = Title,
				Year = Year
			};
		}
	}
}
