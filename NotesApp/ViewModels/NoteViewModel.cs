using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace NotesApp.ViewModels
{
    public class NoteViewModel : ObservableObject, IQueryAttributable
    {
        private Models.Note _note;

        public string Identifier => this._note.Filename;

        public DateTime Date => this._note.Date;

        public string Text
        {
            get => this._note.Text;

            set
            {
                if (this._note.Text != value)
                {
                    this._note.Text = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public NoteViewModel()
        {
            this._note = new Models.Note();
            this.SaveCommand = new AsyncRelayCommand(this.Save);
            this.DeleteCommand = new AsyncRelayCommand(this.Delete);
        }

        public NoteViewModel(Models.Note note)
        {
            this._note = note;
            this.SaveCommand = new AsyncRelayCommand(this.Save);
            this.DeleteCommand = new AsyncRelayCommand(this.Delete);
        }

        private async Task Save()
        {
            this._note.Date = DateTime.Now;
            this._note.Save();
            await Shell.Current.GoToAsync($"..?saved={this._note.Filename}");
        }

        private async Task Delete()
        {
            this._note.Delete();
            await Shell.Current.GoToAsync($"..?deleted={this._note.Filename}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                var filename = query["load"].ToString();
                this._note = Models.Note.Load(filename);
                this.RefreshProperties();
            }
        }

        public void Reload()
        {
            this._note = Models.Note.Load(this._note.Filename);
            this.RefreshProperties();
        }

        private void RefreshProperties()
        {
            this.OnPropertyChanged(nameof(this.Text));
            this.OnPropertyChanged(nameof(this.Date));
        }
    }
}
