using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Net;

using Microsoft.Win32;

namespace Test_Client_1.ViewModels
{
    using Test_Client_1.Controllers;
    using Test_Client_1.Models;

    using Test_Client_1.Command;
    using Test_Client_1.Exstension;
    using Test_Client_1.Converters;

    class NotesViewModel : DependencyObject, INotifyPropertyChanged
    {
        private static RAController _rAController;
        private static ToastController _toastController;

        #region Commands

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand GetCommand { get; private set; }

        public ICommand LoadImageCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenMenuCommand { get; private set; }

        public ICommand EditImageCommand { get; private set; }

        #endregion

        #region Note fields

        private string _headline;

        public string NHeadline
        {
            get { return _headline; }
            set { _headline = value; OnPropertyChanged("NHeadline"); }
        }

        private string _description;

        public string NDescription
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("NDescription"); }
        }

        private string _date;

        public string NDate
        {
            get { return _date; }
            set { _date = value; OnPropertyChanged("NDate"); }
        }

        private string _image;

        public string NImage
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("NImage"); }
        }

        private void DropNoteFields()
        {
            NHeadline = string.Empty;
            NDescription = string.Empty;
            NDate = string.Empty;
            NImage = null;
        }

        #endregion

        #region Visibility fields

        private Visibility _listNotesVis;

        public Visibility ListNotesVisibility
        {
            get { return _listNotesVis; }
            set { _listNotesVis = value; OnPropertyChanged("ListNotesVisibility"); }
        }

        private Visibility _elementsManagmentVis;

        public Visibility ElementsManagmentVisibility
        {
            get { return _elementsManagmentVis; }
            set { _elementsManagmentVis = value; OnPropertyChanged("ElementsManagmentVisibility"); }
        }

        private Visibility _menuVis;

        public Visibility MenuVisibility
        {
            get { return _menuVis; }
            set { _menuVis = value; OnPropertyChanged("MenuVisibility"); }
        }

        private Visibility _searchGridVisibility;

        public Visibility SearchGridVisibility
        {
            get { return _searchGridVisibility; }
            set { _searchGridVisibility = value; OnPropertyChanged("SearchGridVisibility"); }
        }

        private Visibility _rightSectorVis;

        public Visibility RightSectorVisibility
        {
            get { return _rightSectorVis; }
            set { _rightSectorVis = value; OnPropertyChanged("RightSectorVisibility"); }
        }

        #endregion

        #region Value field

        private double _widthMainGrid;

        public double WidthMainGrid
        {
            get { return _widthMainGrid; }
            set { _widthMainGrid = value; OnPropertyChanged("WidthMainGrid"); }
        }


        #endregion

        private NoteModel _selectedNote;
        public NoteModel SelectedNote
        {
            get { return _selectedNote; }
            set
            {
                _selectedNote = value;
                OnPropertyChanged("SelectedNote");
            }
        }

        public ObservableCollection<NoteModel> Notes { get; private set; }

        public NotesViewModel() { }

        public NotesViewModel(Grid grid)
        {
            _rAController = RAController.GetInstance();
            _toastController = ToastController.GetInstance();
            _toastController.InitializeComponents(grid);

            Notes = new ObservableCollection<NoteModel>();
            SelectedNote = new NoteModel();

            Items = CollectionViewSource.GetDefaultView(Notes);
            Items.Filter = FilterPerson;

            WidthMainGrid = 360.0d;

            ListNotesVisibility = Visibility.Visible;
            ElementsManagmentVisibility = Visibility.Visible;
            MenuVisibility = Visibility.Hidden;
            SearchGridVisibility = Visibility.Visible;
            RightSectorVisibility = Visibility.Hidden;

            AddCommand = new DelegateCommand(AddNoteAsync);
            EditCommand = new DelegateCommand(EditNote);
            DeleteCommand = new DelegateCommand(DeleteNote);
            GetCommand = new DelegateCommand(GetNotes);

            LoadImageCommand = new DelegateCommand(LoadImage);
            CancelCommand = new DelegateCommand(CancelAdd);
            OpenMenuCommand = new DelegateCommand(OpenMenu);

            EditImageCommand = new DelegateCommand(EditImage);

            GetNotes(null);
        }

        private void OpenMenu(object obj)
        {
            WidthMainGrid = 0;
            NDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
            ChangeVisible();
        }
        private void CancelAdd(object obj)
        {
            WidthMainGrid = 360.0d;
            DropNoteFields();
            ChangeVisible();
        }
        private void LoadImage(object obj)
        {
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "Выберите изображение";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                using (var bitmap = new Bitmap(op.FileName))
                {
                    var pathToImage = bitmap.ScaleImage(100, 100);

                    NImage = null;

                    using (var image = new Bitmap(pathToImage))
                    {
                        NImage = ImageConverter.ImageToBase64(image);
                    }
                    
                }
            }
        }

        private async void AddNoteAsync(object obj)
        {
            if (NImage != null && !NHeadline.Equals(string.Empty) && !NDescription.Equals(string.Empty))
            {
                long ticks = DateTime.Now.Ticks;
                byte[] bytes = BitConverter.GetBytes(ticks);
                string id = Convert.ToBase64String(bytes)
                                        .Replace('+', '_')
                                        .Replace('/', '-')
                                        .TrimEnd('=');

                var model = new NoteModel() {NoteID = id, Headline = NHeadline, Description = NDescription, Date = NDate, Image = NImage };
                var result = await _rAController.CheckServer();

                if (result.Result)
                {
                    await _rAController.AddNoteAsync(model);

                    WidthMainGrid = 360.0d;
                    ChangeVisible();
                    DropNoteFields();
                    Notes.Add(model);
                }
                else
                {
                    var message = string.Empty;

                    foreach (var error in result.errorMessageList)
                        message = "\n" + error;

                    await _toastController.ViewToast("Данные не добавлены!" + message);
                }
            }
            else
            {
                await _toastController.ViewToast("Не все поля заполнены!");
            }                 
        }
        private async void EditNote(object obj)
        {
            var result = await _rAController.CheckServer();

            if (result.Result == false)
            {
                var message = string.Empty;

                foreach (var error in result.errorMessageList)
                    message = "\n" + error;

                await _toastController.ViewToast(message);
                return;
            }

            await _rAController.EditNoteAsync(SelectedNote);
        }
        private async void DeleteNote(object obj)
        {
            List<NoteModel> selectedNoteList = new List<NoteModel>();
            var searchSelectedNote = Notes.Where(x => x.IsSelected == true);
            foreach (var note in searchSelectedNote)
                selectedNoteList.Add(note);

            if (selectedNoteList.Count == 0)
            {
                await _toastController.ViewToast("Выберите запись для удаления!");
                return;
            }
                

            var msResult = MessageBox.Show("Удалить?", "Удаление", MessageBoxButton.OKCancel);
            if (msResult == MessageBoxResult.OK)
            {
                var result = await _rAController.CheckServer();

                if (result.Result == false)
                {
                    var message = string.Empty;

                    foreach (var error in result.errorMessageList)
                        message = "\n" + error;

                    MessageBox.Show(message);
                    return;
                }

                if (searchSelectedNote.Count() > 1)
                {
                    await _rAController.DeleteNotesAsync(selectedNoteList);
                }
                else
                {
                    await _rAController.DeleteNoteAsync(selectedNoteList.First());
                }

                foreach (var note in selectedNoteList)
                    Notes.Remove(note);
            }
        }
        private async void GetNotes(object obj)
        {
            var result = await _rAController.CheckServer();

            if(result.Result == false)
            {

                var message = string.Empty;

                foreach (var error in result.errorMessageList)
                    message = "\n" + error;

                await _toastController.ViewToast(message);
                return;
            }

            var content = await _rAController.GetNotesAsync();

            if (content != null)
            {
                Notes.Clear();

                foreach (var item in content)
                    Notes.Add(item);
            }
        }

        private void EditImage(object obj)
        {
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "Выберите изображение";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                using (var bitmap = new Bitmap(op.FileName))
                {
                    var pathToImage = bitmap.ScaleImage(100, 100);

                    SelectedNote.Image = "";

                    using (var image = new Bitmap(pathToImage))
                    {
                        var bs = ImageConverter.ImageToBase64(image);
                        SelectedNote.Image = bs;
                    }
                }
            }
        }

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(NotesViewModel), new PropertyMetadata("", FilterText_OnPropertyChanged));

        private static void FilterText_OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = d as NotesViewModel;

            if(current != null)
            {
                current.Items.Filter = null;
                current.Items.Filter = current.FilterPerson;
            }
        }

        public ICollectionView Items
        {
            get { return (ICollectionView)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(ICollectionView), typeof(NotesViewModel), new PropertyMetadata());

        private bool FilterPerson(object obj)
        {
            NoteModel note = obj as NoteModel;

            if(!string.IsNullOrWhiteSpace(FilterText) && note != null)
            {
                return note.Filter(FilterText, typeof(NoteModel));
            }

            return true;
        }

        private void ChangeVisible()
        {
            ListNotesVisibility = ListNotesVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            ElementsManagmentVisibility = ElementsManagmentVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            MenuVisibility = MenuVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            SearchGridVisibility = SearchGridVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            RightSectorVisibility = RightSectorVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
