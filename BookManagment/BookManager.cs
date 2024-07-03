using BookManagment;
using System;
using System.Collections.ObjectModel;
using System.Linq;

public class BookManager : IBookManager
{
    private ObservableCollection<Book> books;

    public event EventHandler<BookEventArgs> BookAdded;
    public event EventHandler<BookEventArgs> BookRemoved;
    public event EventHandler<BookEventArgs> BookUpdated;

    public BookManager()
    {
        books = new ObservableCollection<Book>();
    }
    
    /// Hinzufügen eines Buches
    public void AddBook(Book book)
    {
        books.Add(book); // Das Buch wird zur Sammlung hinzugefügt
        OnBookAdded(new BookEventArgs { Book = book }); // Das Ereignis BookAdded wird ausgelöst
    }
    /// Entfernen eines Buches
    public void RemoveBook(Book book)
    {
        books.Remove(book);
        OnBookRemoved(new BookEventArgs { Book = book });
    }
    /// Liste erstellen
    public ObservableCollection<Book> ListBooks()
    {
        return new ObservableCollection<Book>(books);
    }

    public void EditBook(int id, string newTitle, string newAuthor, string newGenre, DateTime newPublicationDate, string newStatus)
    {
        var existingBook = books.FirstOrDefault(b => b.ID == id);
        if (existingBook != null)
        {
            existingBook.Title = newTitle;
            existingBook.Author = newAuthor;
            existingBook.Genre = newGenre;
            existingBook.PublicationDate = newPublicationDate;
            existingBook.Status = newStatus;
            OnBookUpdated(new BookEventArgs { Book = existingBook });
        }
    }
    
    public ObservableCollection<Book> FilterBooks(Func<Book, bool> predicate)
    {
        return new ObservableCollection<Book>(books.Where(predicate));
    }

    public ObservableCollection<Book> SearchBooks(string searchTerm)
    {
        return new ObservableCollection<Book>(books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                                               b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
    }

    public int GetAvailableBookCount()
    {
        return books.Count(b => b.Status.Equals("Available", StringComparison.OrdinalIgnoreCase));
    }



    // Überprüft ob es Abonnenten für das Ereignis gibt und löst es aus
    protected virtual void OnBookAdded(BookEventArgs e)
    {
        BookAdded?.Invoke(this, e);
    }

    protected virtual void OnBookRemoved(BookEventArgs e)
    {
        BookRemoved?.Invoke(this, e);
    }

    protected virtual void OnBookUpdated(BookEventArgs e)
    {
        BookUpdated?.Invoke(this, e);
    }
}
