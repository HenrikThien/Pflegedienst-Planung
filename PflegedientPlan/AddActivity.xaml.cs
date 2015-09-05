using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PflegedientPlan
{
    /// <summary>
    /// Interaction logic for AddActivity.xaml
    /// </summary>
    public partial class AddActivity : Window
    {
        public delegate void MainNeedsAnUpdate(ObservableCollection<Activity> activityList);
        public event MainNeedsAnUpdate OnMainNeedsAnUpdate;

        private readonly ObservableCollection<Activity> _activityList = new ObservableCollection<Activity>();

        public AddActivity()
        {
            InitializeComponent();
            Init();
        }

        private async void Init()
        { 
            await LoadActivityFromDatabaseAsync();
        }

        private async Task LoadActivityFromDatabaseAsync()
        {
            try
            {
                _activityList.Clear();

                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        using (var reader = await client.SelectAsync("SELECT * FROM activitys"))
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    var activity = new Activity()
                                    {
                                        Id = reader.GetInt32(0),
                                        Description = reader.GetString(1)
                                    };

                                    _activityList.Add(activity);
                                }
                            }
                        }
                    }
                }

                activtyDataGrid.ItemsSource = _activityList;
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        private void addNewActivityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // remove spaces in end and begin
                var description = activityTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(description))
                {
                    InsertNewActivity(description);
                }

                // overwrite
                activityTextBox.Text = "";
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        private async void WriteException(Exception ex)
        {
            await Logger.WriteException(ex.ToString());
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                addNewActivityButton_Click(null, null);
            }
        }

        private async void InsertNewActivity(string description)
        {
            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<string>("@desc", description);
                    var status = await client.InsertAsync("INSERT INTO activitys (description) VALUES (@desc);");
                }
            }

            await LoadActivityFromDatabaseAsync();
            OnMainNeedsAnUpdate.Invoke(_activityList);
        }

        private async void contextMenuDeleteActivity_Click(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (activtyDataGrid.SelectedItem as Activity);

            if (selectedActivity == null)
                return;

            var result = MessageBox.Show("\"" + selectedActivity.Description + "\"\n Jetzt löschen?", "Löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteAsync(selectedActivity);
            }
        }

        private async Task DeleteAsync(Activity activity)
        {
            try
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@activity_id", activity.Id);
                        var count = await client.ExecuteAsync("DELETE FROM activitys WHERE Id = @activity_id;");
                        client.ClearParameter();
                        client.AddParam<int>("@parent_id", activity.Id);
                        await client.ExecuteAsync("DELETE FROM categorys WHERE activity_id = @parent_id;");
                        
                        if (count > 0)
                        {
                            MessageBox.Show("Die Aktivität wurde erfolgreich gelöscht", "Gelöscht");
                        }
                    }
                }

                var activityToRemove = _activityList.Select(a => a).Where(a => a.Id == activity.Id).FirstOrDefault();

                if (activityToRemove != null)
                {
                    _activityList.Remove(activityToRemove);
                    OnMainNeedsAnUpdate.Invoke(_activityList);
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }
    }
}
