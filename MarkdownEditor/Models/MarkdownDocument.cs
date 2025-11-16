using System;

namespace MarkdownEditor.Models
{
    public class MarkdownDocument
    {
        private string _content = string.Empty;
        private string _filePath = string.Empty;

        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    ContentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public event EventHandler? ContentChanged;
    }
}