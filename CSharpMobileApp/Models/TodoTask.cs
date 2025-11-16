using System.ComponentModel;

namespace CSharpMobileApp.Models
{
    public class TodoTask : INotifyPropertyChanged
    {
        private int _id;
        private int _order;
        private string _text = string.Empty;
        private bool _completed;

        public int Id
        {
            get => _id;
            set
            {
                if (_id == value) return;
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public int Order
        {
            get => _order;
            set
            {
                if (_order == value) return;
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public bool Completed
        {
            get => _completed;
            set
            {
                if (_completed == value) return;
                _completed = value;
                OnPropertyChanged(nameof(Completed));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
