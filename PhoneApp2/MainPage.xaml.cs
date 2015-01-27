using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

using PhoneApp2.Models;
using PhoneApp2.Resources;
using PhoneApp2.ViewModels;

using PhoneClassLibrary1.DB;
using PhoneClassLibrary1.Models;

namespace PhoneApp2
{
    public partial class MainPage : PhoneApplicationPage
    {

        private EnglishWordDataContext _context;

        private ObservableCollection<EnglishWord> _englishEnglishWords;
 
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // this.NavigationCacheMode = NavigationCacheMode.Required;
            // Sample code to localize the ApplicationBar
            // BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Prepare for display here
            var invoice = new Invoice { Amount = 5.0m };

            var viewModel = new InvoiceViewModel(invoice);
            DataContext = viewModel;

            // create data context to manage DB
            _context = new EnglishWordDataContext(EnglishWordDataContext.ConnectionString);
            // Create observable collection to display in UI
            _englishEnglishWords = new ObservableCollection<EnglishWord>(_context.EnglishWords);
            EnglishWordBox.ItemsSource = _englishEnglishWords;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Clean up after the datacontext
            _context.Dispose();
            _context = null;
            base.OnNavigatedFrom(e);
        }

        private void AddClick(object sender, EventArgs e)
        {
            // avoid add same word
            bool wordExist = _englishEnglishWords.Any(w => w.Word.Equals(WordTextBox.Text));

            if (!wordExist)
            {
                // create object
                var englishWord = new EnglishWord() { Word = WordTextBox.Text, Meaning = MeaningTextBox.Text };

                // Place object is pending insert state
                _context.EnglishWords.InsertOnSubmit(englishWord);
                
                // Commit changes to DB
                _context.SubmitChanges();

                // Sync object with observable collection
                _englishEnglishWords.Add(englishWord);
                WordTextBox.Text = string.Empty;
                MeaningTextBox.Text = string.Empty;
            }
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (EnglishWordBox.SelectedItem != null)
            {
                // Get selected object
                var englishWord = EnglishWordBox.SelectedItem as EnglishWord;

                if (!_context.EnglishWords.Any(w => w.Equals(englishWord)))
                {
                    // Associate object with context
                    _context.EnglishWords.Attach(englishWord);

                }

                // Place in pending delete state
                _context.EnglishWords.DeleteOnSubmit(englishWord);
                
                // commit changes to DB
                _context.SubmitChanges();

                // Sync with list box
                _englishEnglishWords.Remove(englishWord);

            }
            
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void BackgroundButton_OnClick(object sender, EventArgs e)
        {
            const string taskName = "AdoptionTask";
            var task = ScheduledActionService.Find(taskName) as PeriodicTask;
            if (task != null)
            {
                ScheduledActionService.Remove(taskName);
            }

            task = new PeriodicTask(taskName);
            task.Description = "Test show toad message";
            try
            {
                ScheduledActionService.Add(task);
                #if DEBUG
                    ScheduledActionService.LaunchForTest(taskName, TimeSpan.FromSeconds(1));
                #endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void UpdateTile_Click(object sender, RoutedEventArgs e)
        {
            _context = new EnglishWordDataContext(EnglishWordDataContext.ConnectionString);

            // Create observable collection to display in UI            
            if (!_context.EnglishWords.Any(x => x.IsLearn == false))
            {
                _englishEnglishWords = new ObservableCollection<EnglishWord>();
                foreach (var englishWord in _englishEnglishWords)
                {
                    // Update IsLearn status
                    englishWord.IsLearn = false;
                    _context.EnglishWords.Attach(englishWord);
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
                _context.SubmitChanges();
            }


            ScheduledTaskAgent2.ScheduledAgent.UpdatePrimaryTile(_englishEnglishWords.Count, sb.ToString());
        }

        private ShellTileData CreateIconicTileData()
        {
            IconicTileData iconicTileData = new IconicTileData();
            iconicTileData.Count = 11;
            iconicTileData.IconImage = new Uri("/Assets/pizza.lockicon.png", UriKind.Relative);
            iconicTileData.SmallIconImage = new Uri("/Assets/pizza.lockicon.png", UriKind.Relative);
            iconicTileData.WideContent1 = "Wide content 1";
            iconicTileData.WideContent2 = "Wide content 2";
            iconicTileData.WideContent3 = "Wide content 3";

            return iconicTileData;
        }

        private ShellTile FindTile(string partOfUri)
        {
            ShellTile shellTile = ShellTile.ActiveTiles.FirstOrDefault(
            tile => tile.NavigationUri.ToString().Contains(partOfUri));

            return shellTile;
        }

        private ShellTileData CreateCycleTileData()
        {
            string[] imageNames =
                {
                    "bonfillet.jpg", "bucket.jpg", "burger.jpg", "caesar.jpg", "chicken.jpg", "corn.jpg",
                    "fries.jpg", "wings.jpg"
                };

            CycleTileData cycleTileData = new CycleTileData();
            cycleTileData.Title = "Cycle tile";
            cycleTileData.Count = 7;
            cycleTileData.SmallBackgroundImage = new Uri("/Assets/pizza.lockicon.png", UriKind.Relative);
            cycleTileData.CycleImages =
                imageNames.Select(imageName => new Uri(string.Concat("/Assets/Images/", imageName), UriKind.Relative));

            return cycleTileData;
        }
    }
}