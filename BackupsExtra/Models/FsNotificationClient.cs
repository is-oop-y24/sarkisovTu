using System;
using System.Linq;
using System.Text.RegularExpressions;
using Backups.Repository;

namespace BackupsExtra.Models
{
    public class FsNotificationClient : INotificationClient
    {
        private string _notificationFilePath;

        public FsNotificationClient(string notificationFilePath)
        {
            _notificationFilePath = notificationFilePath;
        }

        public void Update(NotificationMessage message)
        {
            IRepository repositorySystem = message.SenderService.RepositorySystem;
            if (!repositorySystem.IsFileExist(_notificationFilePath))
            {
                const string fileRegexPattern = @"\\[A-Za-z0-9]+\.[a-z]+";
                var fileRegex = new Regex(fileRegexPattern);
                string fileName = fileRegex.Matches(_notificationFilePath).Last().ToString().Substring(1);
                string pathToFile = _notificationFilePath.Replace($@"\{fileName}", string.Empty);
                repositorySystem.CreateFileInDirectory(pathToFile, fileName, string.Empty);
            }

            string notificationContent = repositorySystem.ReadFile(_notificationFilePath);
            string notificationMessage = $"[{message.Date.ToString("MM/dd/yyyy HH:mm:ss")}] - {message.Message} \n";
            repositorySystem.UpdateFile(_notificationFilePath, notificationContent + notificationMessage);
        }
    }
}