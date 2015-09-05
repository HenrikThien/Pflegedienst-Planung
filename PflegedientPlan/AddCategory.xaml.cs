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
    /// Interaction logic for AddCategory.xaml
    /// </summary>
    public partial class AddCategory : Window
    {
        public delegate void MainNeedsAnUpdate(ObservableCollection<Category> categoryList);
        public event MainNeedsAnUpdate OnMainNeedsAnUpdate;

        private readonly ObservableCollection<Activity> _activityList = new ObservableCollection<Activity>();
        private readonly ObservableCollection<Category> _categoryList = new ObservableCollection<Category>();

        public AddCategory()
        {
            InitializeComponent();

            Init();
        }

        private async void Init()
        {
            await LoadCategorysFromDatabaseAsync();
            await LoadActivitysFromDatabaseAsync();

            if (_activityList.Count > 0)
                activitysComboBox.SelectedIndex = 0;
        }

        private async Task LoadCategorysFromDatabaseAsync()
        {
            try
            {
                _categoryList.Clear();

                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        using (var reader = await client.SelectAsync("SELECT categorys.Id,categorys.activity_id,categorys.description,activitys.description FROM categorys INNER JOIN activitys ON categorys.activity_id = activitys.Id;"))
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    var category = new Category()
                                    {
                                        Id = reader.GetInt32(0),
                                        ParentId = reader.GetInt32(1),
                                        CategoryDescription = reader.GetString(2),
                                        ActivityDescription = reader.GetString(3)
                                    };

                                    _categoryList.Add(category);
                                }
                            }
                        }
                    }
                }

                categoryDataGrid.ItemsSource = _categoryList;
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        private async Task LoadActivitysFromDatabaseAsync()
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

                activitysComboBox.ItemsSource = _activityList;
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }

        private void addNewCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // remove spaces in end and begin
                var description = categoryTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(description) && activitysComboBox.SelectedItem != null)
                {
                    var activityId = (activitysComboBox.SelectedItem as Activity).Id;
                    InsertNewCategory(description, activityId);
                }

                // overwrite
                categoryTextBox.Text = "";
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
                addNewCategoryButton_Click(null, null);
            }
        }

        private async void InsertNewCategory(string description, int activityId)
        {
            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@activity_id", activityId);
                    client.AddParam<string>("@desc", description);

                    var status = await client.InsertAsync("INSERT INTO categorys (activity_id,description) VALUES (@activity_id,@desc);");
                }
            }

            await LoadCategorysFromDatabaseAsync();

            OnMainNeedsAnUpdate.Invoke(_categoryList);
        }
    }
}
