using System;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownEditor.Services
{
    public class FileManager
    {
        public async Task<string> LoadFileAsync(string filePath)
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки файла: {ex.Message}");
            }
        }

        public async Task SaveFileAsync(string filePath, string content)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        public async Task ExportFileAsync(string filePath, string content)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка экспорта файла: {ex.Message}");
            }
        }
    }
}