using BookManagment;
using CsvHelper;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace BookManagement
{
    /// <summary>
    /// Hauptklasse erbt von Window
    /// </summary>
    public partial class MainWindow : Window
    {
        private IBookManager bookManager; // definiert Methoden für Buchverwaltung
        private bool isLoadingBooks = false; // verhindert auslösen von events während einlesen der Datei

        /// <summary>
        /// Dynamisch Aktualisierte Sammlung
        /// </summary>
        public ObservableCollection<Book> Books { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            bookManager = new BookManager();
            bookManager.BookAdded += OnBookAdded;
            bookManager.BookRemoved += OnBookRemoved;
            bookManager.BookUpdated += OnBookUpdated;
            Books = new ObservableCollection<Book>();
            DataContext = this; // bindet Mainwindow an WPF von Books Sammlung
        }

        /// <summary>
        /// Hinzufügen Ausgabe m
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBookAdded(object sender, BookEventArgs e)
        {
            if (!isLoadingBooks)
            {
                MessageBox.Show($"Book added: {e.Book.Title}");
                UpdateAvailableBookCount(); // Ruft Methode auf um den Counter zu aktualisieren
            }
        }

        /// <summary>
        /// Entfernen Ausgabe m
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBookRemoved(object sender, BookEventArgs e)
        {
            if (!isLoadingBooks)
            {
                MessageBox.Show($"Book removed: {e.Book.Title}");
                UpdateAvailableBookCount();
            }
        }

        /// <summary>
        /// Updaten Ausgabe m
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBookUpdated(object sender, BookEventArgs e)
        {
            if (!isLoadingBooks)
            {
                MessageBox.Show($"Book updated: {e.Book.Title}");
                UpdateAvailableBookCount();
            }
        }

        /// <summary>
        /// Bücher Daten auswählen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBooksButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    isLoadingBooks = true;
                    string filePath = openFileDialog.FileName;
                    List<Book> books = ReadRecordsFromCsv<Book>(filePath);
                    Books.Clear();
                    foreach (var book in books)
                    {
                        bookManager.AddBook(book);
                        Books.Add(book);
                    }
                    UpdateAvailableBookCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beim auswählen der Datei ist ein Fehler aufgetreten: {ex.Message}");
            }
            finally
            {
                isLoadingBooks = false;
            }
        }
        /// <summary>
        /// CSV Datei einlesen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<T> ReadRecordsFromCsv<T>(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) // CultureInfo für Datum
                {
                    return csv.GetRecords<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Beim Einlesen der Daten ist ein Fehler aufgetreten: {ex.Message}");
                return new List<T>();
            }
        }

        /// <summary>
        /// Erstellung eines Buches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var newBook = new Book
            {
                ID = Books.Count > 0 ? Books.Max(b => b.ID) + 1 : 1, // Wenn ja maxID+1
                Title = AddTitleTextBox.Text,
                Author = AddAuthorTextBox.Text,
                Genre = AddGenreTextBox.Text,
                PublicationDate = AddPublicationDatePicker.SelectedDate ?? DateTime.Now,
                Status = AddStatusTextBox.Text
            };
            bookManager.AddBook(newBook);
            Books.Add(newBook);
        }

        /// <summary>
        /// Buch bearbeiten mithilde ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(EditIDTextBox.Text, out int id))
            {
                bookManager.EditBook(id, EditTitleTextBox.Text, EditAuthorTextBox.Text, EditGenreTextBox.Text, EditPublicationDatePicker.SelectedDate ?? DateTime.Now, EditStatusTextBox.Text);
               
                Books.Clear();
                foreach (var book in bookManager.ListBooks())
                {
                    Books.Add(book);
                }
                UpdateAvailableBookCount();
            }
        }

        /// <summary>
        /// Filter by Gernre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterByGenreButton_Click(object sender, RoutedEventArgs e)
        {
            string genre = FilterGenreTextBox.Text;
            var filteredBooks = bookManager.FilterBooks(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
            Books.Clear();
            foreach (var book in filteredBooks)
            {
                Books.Add(book);
            }
        }

        /// <summary>
        /// Filter by Status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterByStatusButton_Click(object sender, RoutedEventArgs e)
        {
            string status = FilterStatusTextBox.Text;
            var filteredBooks = bookManager.FilterBooks(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            Books.Clear();
            foreach (var book in filteredBooks)
            {
                Books.Add(book);
            }
        }
        /// <summary>
        /// Gesamtliste ohne Filter anzeigen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            Books.Clear();
            foreach
                 (var book in bookManager.ListBooks())
            {
                Books.Add(book);
            }
        }
        /// <summary>
        /// Suchmethode nachTitel oder Author
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBooksButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTextBox.Text;
            var searchResults = bookManager.SearchBooks(searchTerm);
            Books.Clear();
            foreach (var book in searchResults)
            {
                Books.Add(book);
            }
        }

        /// <summary>
        /// Delete by ID Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DeleteIDTextBox.Text, out int id))
            {
                var bookToDelete = Books.FirstOrDefault(b => b.ID == id); // Linq Methode
                if (bookToDelete != null)
                {
                    bookManager.RemoveBook(bookToDelete);
                    Books.Remove(bookToDelete);
                    UpdateAvailableBookCount();
                }
                else
                {
                    MessageBox.Show("This ID was not found");
                }
            }
            else
            {
                MessageBox.Show("The ID has to be an Integer");
            }
        }
        /// <summary>
        /// Speicher Button für Speicherort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBooksButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    SaveRecordsToCsv(filePath, Books);
                    MessageBox.Show("Books saved successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Saving Error: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Speicherung der Bücher in filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="books"></param>
        private void SaveRecordsToCsv(string filePath, ObservableCollection<Book> books)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(books);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error by saving the value: {ex.Message}");
            }
        }

        /// <summary>
        /// Verfügbare Bücher Counter
        /// </summary>
        private void UpdateAvailableBookCount()
        {
            int availableBookCount = bookManager.GetAvailableBookCount();
            AvailableBooksTextBlock.Text = $"Available Books: {availableBookCount}";
        }
    }

}