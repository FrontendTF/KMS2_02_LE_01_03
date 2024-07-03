using BookManagment;
using System;
using System.Collections.ObjectModel;

public interface IBookManager
{
    event EventHandler<BookEventArgs> BookAdded;
    event EventHandler<BookEventArgs> BookRemoved;
    event EventHandler<BookEventArgs> BookUpdated;

    void AddBook(Book book);
    void RemoveBook(Book book);
    ObservableCollection<Book> ListBooks();
    void EditBook(int id, string newTitle, string newAuthor, string newGenre, DateTime newPublicationDate, string newStatus);
    ObservableCollection<Book> FilterBooks(Func<Book, bool> predicate); // Delegat Typ zB public delegate bool Predicate<in T>(T obj);
    ObservableCollection<Book> SearchBooks(string searchTerm);
    int GetAvailableBookCount();
}
