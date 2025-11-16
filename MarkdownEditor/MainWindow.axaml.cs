using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HtmlAgilityPack;
using Markdig;
using MarkdownEditor.Models;
using MarkdownEditor.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownEditor
{
    public partial class MainWindow : Window
    {
        private readonly MarkdownDocument _document;
        private readonly FileManager _fileManager;
        private readonly MarkdownPipeline _markdownPipeline;
        private TextBox? _markdownEditor;
        private ContentControl? _markdownPreview;
        private RadioButton? _editModeRadio;
        private RadioButton? _previewModeRadio;

        public MainWindow() : this(new string[0])
        {
        }

        public MainWindow(string[] args)
        {
            InitializeComponent();

            _document = new MarkdownDocument();
            _fileManager = new FileManager();
            _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

            _document.ContentChanged += OnContentChanged;

            Loaded += async (s, e) => await OnWindowLoaded(args);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _markdownEditor = this.FindControl<TextBox>("MarkdownEditor");
            _markdownPreview = this.FindControl<ContentControl>("MarkdownPreview");
            _editModeRadio = this.FindControl<RadioButton>("EditModeRadio");
            _previewModeRadio = this.FindControl<RadioButton>("PreviewModeRadio");

            if (_markdownEditor != null)
            {
                _markdownEditor.TextChanged += (s, e) =>
                {
                    if (_markdownEditor.Text != null)
                        _document.Content = _markdownEditor.Text;
                };
            }

            if (_editModeRadio != null)
            {
                _editModeRadio.IsCheckedChanged += (s, e) =>
                {
                    if (_editModeRadio.IsChecked == true)
                        UpdatePreview();
                };
            }

            if (_previewModeRadio != null)
            {
                _previewModeRadio.IsCheckedChanged += (s, e) =>
                {
                    if (_previewModeRadio.IsChecked == true)
                        UpdatePreview();
                };
            }
        }

        private async Task OnWindowLoaded(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                await LoadFile(args[0]);
            }
            else
            {
                _document.Content = "# Добро пожаловать в Markdown Editor\n\nНачните редактирование...";
            }
        }

        private void OnContentChanged(object? sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (_markdownPreview != null)
            {
                try
                {
                    var html = Markdown.ToHtml(_document.Content, _markdownPipeline);
                    var formattedContent = CreateFormattedContent(html);

                    _markdownPreview.Content = formattedContent;
                }
                catch
                {
                    var textBlock = new TextBlock
                    {
                        Text = _document.Content,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(10),
                        Foreground = Avalonia.Media.Brushes.Black
                    };
                    _markdownPreview.Content = textBlock;
                }
            }
        }

        private StackPanel CreateFormattedContent(string html)
        {
            var stackPanel = new StackPanel { Margin = new Avalonia.Thickness(10) };

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                foreach (var node in doc.DocumentNode.ChildNodes)
                {
                    var element = CreateElementFromNode(node);
                    if (element != null)
                        stackPanel.Children.Add(element);
                }
            }
            catch
            {
                stackPanel.Children.Add(new TextBlock
                {
                    Text = _document.Content,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Foreground = Avalonia.Media.Brushes.Black
                });
            }

            return stackPanel;
        }

        private Control? CreateElementFromNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Text)
            {
                var text = node.InnerText.Trim();
                if (string.IsNullOrEmpty(text)) return null;

                return new TextBlock
                {
                    Text = text,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Foreground = Avalonia.Media.Brushes.Black,
                    FontSize = 14,
                    Margin = new Avalonia.Thickness(0, 2)
                };
            }

            if (node.NodeType != HtmlNodeType.Element) return null;

            switch (node.Name.ToLower())
            {
                case "h1":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 24,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 15, 0, 8),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "h2":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 20,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 12, 0, 6),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "h3":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 18,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 10, 0, 5),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "h4":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 16,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 8, 0, 4),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "h5":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 15,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 6, 0, 3),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "h6":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 14,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 5, 0, 2),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "p":
                    return CreateFormattedTextBlock(node);

                case "strong":
                case "b":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontWeight = Avalonia.Media.FontWeight.Bold,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "em":
                case "i":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontStyle = Avalonia.Media.FontStyle.Italic,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "del":
                case "s":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        TextDecorations = Avalonia.Media.TextDecorations.Strikethrough,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "code":
                    return new Border
                    {
                        Background = Avalonia.Media.Brushes.LightGray,
                        CornerRadius = new Avalonia.CornerRadius(3),
                        Padding = new Avalonia.Thickness(4, 2),
                        Child = new TextBlock
                        {
                            Text = node.InnerText,
                            FontFamily = "Consolas, Monaco, monospace",
                            FontSize = 13,
                            Foreground = Avalonia.Media.Brushes.Black,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        }
                    };

                case "pre":
                    return new Border
                    {
                        Background = Avalonia.Media.Brushes.LightGray,
                        CornerRadius = new Avalonia.CornerRadius(5),
                        Padding = new Avalonia.Thickness(10),
                        Margin = new Avalonia.Thickness(0, 10),
                        Child = new TextBlock
                        {
                            Text = node.InnerText,
                            FontFamily = "Consolas, Monaco, monospace",
                            FontSize = 13,
                            Foreground = Avalonia.Media.Brushes.Black,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        }
                    };

                case "blockquote":
                    return new Border
                    {
                        BorderBrush = Avalonia.Media.Brushes.Gray,
                        BorderThickness = new Avalonia.Thickness(4, 0, 0, 0),
                        Padding = new Avalonia.Thickness(15, 10),
                        Margin = new Avalonia.Thickness(0, 10),
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(248, 248, 248)),
                        Child = new TextBlock
                        {
                            Text = node.InnerText,
                            FontStyle = Avalonia.Media.FontStyle.Italic,
                            Foreground = Avalonia.Media.Brushes.DarkGray,
                            FontSize = 14,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        }
                    };

                case "ul":
                    return CreateList(node, false);

                case "ol":
                    return CreateList(node, true);

                case "table":
                    return CreateTable(node);

                case "hr":
                    return new Border
                    {
                        Height = 1,
                        Background = Avalonia.Media.Brushes.Gray,
                        Margin = new Avalonia.Thickness(0, 15),
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
                    };

                case "a":
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        Foreground = Avalonia.Media.Brushes.Blue,
                        TextDecorations = Avalonia.Media.TextDecorations.Underline,
                        FontSize = 14,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                case "img":
                    var altText = node.GetAttributeValue("alt", "Изображение");
                    return new TextBlock
                    {
                        Text = $"[Изображение: {altText}]",
                        Foreground = Avalonia.Media.Brushes.Gray,
                        FontStyle = Avalonia.Media.FontStyle.Italic,
                        FontSize = 14,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };

                default:
                    return new TextBlock
                    {
                        Text = node.InnerText,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap
                    };
            }
        }

        private TextBlock CreateFormattedTextBlock(HtmlNode node)
        {
            var textBlock = new TextBlock
            {
                FontSize = 14,
                FontFamily = "Times New Roman",
                Foreground = Avalonia.Media.Brushes.Black,
                Margin = new Avalonia.Thickness(0, 0, 0, 10),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            foreach (var child in node.ChildNodes)
            {
                if (child.NodeType == HtmlNodeType.Text)
                {
                    var text = child.InnerText;
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        textBlock.Inlines.Add(new Run(text));
                    }
                }
                else if (child.NodeType == HtmlNodeType.Element)
                {
                    var inline = CreateInlineFromNode(child);
                    if (inline != null)
                    {
                        textBlock.Inlines.Add(inline);
                    }
                }
            }

            return textBlock;
        }

        private Inline? CreateInlineFromNode(HtmlNode node)
        {
            var text = node.InnerText;
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var run = new Run(text);

            switch (node.Name.ToLower())
            {
                case "strong":
                case "b":
                    run.FontWeight = Avalonia.Media.FontWeight.Bold;
                    break;

                case "em":
                case "i":
                    run.FontStyle = Avalonia.Media.FontStyle.Italic;
                    break;

                case "del":
                case "s":
                    run.TextDecorations = Avalonia.Media.TextDecorations.Strikethrough;
                    break;

                case "code":
                    run.FontFamily = "Consolas, Monaco, monospace";
                    break;
            }

            return run;
        }


        private StackPanel CreateList(HtmlNode listNode, bool isOrdered)
        {
            var listPanel = new StackPanel { Margin = new Avalonia.Thickness(0, 5, 0, 10) };
            var counter = 1;

            var listItems = listNode.SelectNodes(".//li");
            if (listItems != null)
            {
                foreach (var li in listItems)
                {
                    var prefix = isOrdered ? $"{counter}. " : "• ";
                    var itemPanel = new StackPanel { Orientation = Avalonia.Layout.Orientation.Horizontal };

                    itemPanel.Children.Add(new TextBlock
                    {
                        Text = prefix,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(20, 2, 5, 2),
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top
                    });

                    itemPanel.Children.Add(new TextBlock
                    {
                        Text = li.InnerText,
                        FontSize = 14,
                        Foreground = Avalonia.Media.Brushes.Black,
                        Margin = new Avalonia.Thickness(0, 2),
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        Width = 300
                    });

                    listPanel.Children.Add(itemPanel);
                    counter++;
                }
            }

            return listPanel;
        }

        private Grid CreateTable(HtmlNode tableNode)
        {
            var grid = new Grid
            {
                Margin = new Avalonia.Thickness(0, 10),
                Background = Avalonia.Media.Brushes.White
            };

            var rows = tableNode.SelectNodes(".//tr");
            if (rows == null) return grid;

            // Определяем количество столбцов
            var maxCols = 0;
            foreach (var row in rows)
            {
                var cells = row.SelectNodes(".//td | .//th");
                if (cells != null && cells.Count > maxCols)
                    maxCols = cells.Count;
            }

            // Создаем столбцы
            for (int i = 0; i < maxCols; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Star) });
            }

            // Создаем строки
            for (int i = 0; i < rows.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = Avalonia.Controls.GridLength.Auto });
            }

            // Заполняем таблицу
            for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var cells = rows[rowIndex].SelectNodes(".//td | .//th");
                if (cells == null) continue;

                for (int colIndex = 0; colIndex < cells.Count && colIndex < maxCols; colIndex++)
                {
                    var isHeader = cells[colIndex].Name.ToLower() == "th" || rowIndex == 0;

                    var border = new Border
                    {
                        BorderBrush = Avalonia.Media.Brushes.Gray,
                        BorderThickness = new Avalonia.Thickness(1),
                        Padding = new Avalonia.Thickness(8, 4),
                        Background = isHeader ? Avalonia.Media.Brushes.LightGray : Avalonia.Media.Brushes.White,
                        Child = new TextBlock
                        {
                            Text = cells[colIndex].InnerText,
                            FontWeight = isHeader ? Avalonia.Media.FontWeight.Bold : Avalonia.Media.FontWeight.Normal,
                            FontSize = 13,
                            Foreground = Avalonia.Media.Brushes.Black,
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap
                        }
                    };

                    Grid.SetRow(border, rowIndex);
                    Grid.SetColumn(border, colIndex);
                    grid.Children.Add(border);
                }
            }

            return grid;
        }

        private async void OpenFile_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Открыть Markdown файл",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Markdown файлы")
                    {
                        Patterns = new[] { "*.md", "*.markdown", "*.txt" }
                    }
                }
            });

            if (files.Count >= 1)
            {
                await LoadFile(files[0].Path.LocalPath);
            }
        }

        private async Task LoadFile(string filePath)
        {
            try
            {
                var content = await _fileManager.LoadFileAsync(filePath);
                _document.FilePath = filePath;
                _document.Content = content;

                if (_markdownEditor != null)
                    _markdownEditor.Text = content;

                Title = $"Markdown Editor - {Path.GetFileName(filePath)}";
                UpdatePreview();
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Ошибка", ex.Message);
            }
        }

        private async void SaveFile_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_document.FilePath))
            {
                await SaveFileAs();
            }
            else
            {
                await SaveFile(_document.FilePath);
            }
        }

        private async Task SaveFileAs()
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить Markdown файл",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Markdown файлы")
                    {
                        Patterns = new[] { "*.md" }
                    }
                }
            });

            if (file != null)
            {
                await SaveFile(file.Path.LocalPath);
            }
        }

        private async Task SaveFile(string filePath)
        {
            try
            {
                await _fileManager.SaveFileAsync(filePath, _document.Content);
                _document.FilePath = filePath;
                Title = $"Markdown Editor - {Path.GetFileName(filePath)}";
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Ошибка", ex.Message);
            }
        }

        private async void ExportFile_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Экспорт Markdown файла",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("Markdown файлы")
                    {
                        Patterns = new[] { "*.md" }
                    }
                }
            });

            if (file != null)
            {
                try
                {
                    await _fileManager.ExportFileAsync(file.Path.LocalPath, _document.Content);
                }
                catch (Exception ex)
                {
                    await ShowErrorDialog("Ошибка", ex.Message);
                }
            }
        }

        private void Exit_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task ShowErrorDialog(string title, string message)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        new Button { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(0, 20, 0, 0) }
                    }
                }
            };

            if (dialog.Content is StackPanel panel && panel.Children.LastOrDefault() is Button okButton)
            {
                okButton.Click += (s, e) => dialog.Close();
            }

            await dialog.ShowDialog(this);
        }
    }
}