using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using PhoneClassLibrary1.DB;
using PhoneClassLibrary1.Models;

namespace ScheduledTaskAgent2
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private EnglishWordDataContext _context;

        private ObservableCollection<EnglishWord> _englishEnglishWords;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        public static void UpdatePrimaryTile(int count, string content)
        {
            FlipTileData primaryTileData = new FlipTileData();
            primaryTileData.Count = count;
            primaryTileData.BackContent = content;

            ShellTile primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(primaryTileData);
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
            // create data context to manage DB
            _context = new EnglishWordDataContext(EnglishWordDataContext.ConnectionString);

            // Create observable collection to display in UI            
            if (!_context.EnglishWords.Any(x => x.IsLearn == false))
            {
                _englishEnglishWords = new ObservableCollection<EnglishWord>();
                foreach (var englishWord in _englishEnglishWords)
                {
                    // Update IsLearn status
                    englishWord.IsLearn = false;
                    _context.SubmitChanges();
                }
            }

            _englishEnglishWords = new ObservableCollection<EnglishWord>(_context.EnglishWords.Where(x => x.IsLearn == false).Take(2));
            var sb = new StringBuilder();
            foreach (var englishWord in _englishEnglishWords)
            {
                string content = string.Format("{0} => {1}", englishWord.Word, englishWord.Meaning);
                sb.Append(content);
                sb.AppendLine(); // which is equal to Append(Environment.NewLine);

                // Update IsLearn status
                englishWord.IsLearn = true;
                _context.EnglishWords.Attach(englishWord);
                _context.SubmitChanges();
            }

            UpdatePrimaryTile(_englishEnglishWords.Count, sb.ToString());
            
            NotifyComplete();
        }
    }
}