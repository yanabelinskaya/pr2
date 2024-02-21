using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using static пр2.Note;

namespace пр2
{
    public partial class MainWindow : Window
    {
        private List<Note> notes;
        private FileManager fileManager;

        public MainWindow()
        {
            InitializeComponent();
            fileManager = new FileManager();
            notes = new List<Note>();

            // Загрузка заметок из файла при запуске приложения
            notes = fileManager.Deserialize<List<Note>>("notes.json");

            datePicker.SelectedDate = DateTime.Now;
            UpdateNotesList();

        }

        private Dictionary<DateTime, List<Note>> notesByDate = new Dictionary<DateTime, List<Note>>();

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Очищаем notesListBox при выборе другой даты
            notesListBox.ItemsSource = null;

            // Если есть заметки для выбранной даты, отображаем их
            if (notesByDate.ContainsKey(datePicker.SelectedDate.Value))
            {
                notesListBox.ItemsSource = notesByDate[datePicker.SelectedDate.Value];
            }
            else
            {
                // Если для выбранной даты заметок нет, создаем пустой список
                notesByDate[datePicker.SelectedDate.Value] = new List<Note>();
            }

            // Очищаем текстовые поля при выборе новой даты
            TitleTextBox.Text = "";
            DescriptionTextBox.Text = "";
        }

        private List<Note> GetNotesForSelectedDate()
        {
            List<Note> selectedDateNotes = new List<Note>();
            if (notes != null)
            {
                foreach (var note in notes)
                {
                    if (note.Date.Date == datePicker.SelectedDate.Value.Date)
                    {
                        selectedDateNotes.Add(note);
                    }
                }
            }
            return selectedDateNotes;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TitleTextBox.Text))
                {
                    string title = TitleTextBox.Text;
                    string description = DescriptionTextBox.Text;

                    DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Today;

                    // Создаем новую заметку
                    Note newNote = new Note
                    {
                        Title = title,
                        Description = description,
                        Date = selectedDate
                    };

                    // Добавляем заметку в список для выбранной даты
                    notesByDate[selectedDate].Add(newNote);

                    // Обновляем отображение списка заметок
                    notesListBox.ItemsSource = notesByDate[selectedDate];
                }
                else
                {
                    MessageBox.Show("Введите название заметки.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при создании заметки: {ex.Message}");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Удаление выбранной заметки
            Note selectedNote = notesListBox.SelectedItem as Note;
            if (notesListBox.SelectedItem != null)
            {
                notes.Remove(selectedNote);
                UpdateNotesList();
            }
            else
            {
                MessageBox.Show("Выберите заметку для удаления.");
            }
        }
        private void UpdateNotesList()
        {
            // Отображение списка заметок для выбранной даты
            notesListBox.ItemsSource = null;
            notesListBox.ItemsSource = notes;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // Сохранение заметок в файл
            try
            {
                SaveNotes();
                MessageBox.Show("Заметка успешно сохранена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении заметки: {ex.Message}");
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // Вызов метода сохранения заметок при закрытии главного окна
            SaveNotes();
        }

        private void SaveNotes()
        {
            // Сохранение заметок в файл
            fileManager.Serialize<List<Note>>(notes, "notes.json");
        }
    }

    // Класс для представления заметки
    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        // Класс для работы с файлами JSON или XML
        public class FileManager
        {
            public void Serialize<T>(T obj, string filePath)
            {
                string json = JsonConvert.SerializeObject(obj);
                File.WriteAllText(filePath, json);
            }

            public T Deserialize<T>(string filePath)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        string json = File.ReadAllText(filePath);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    else
                    {
                        // Возвращаем пустой список, если файл не существует
                        return default(T);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка ошибок чтения файла
                    MessageBox.Show($"Произошла ошибка при чтении файла: {ex.Message}");
                    return default(T);
                }
            }
        }
    }
}
