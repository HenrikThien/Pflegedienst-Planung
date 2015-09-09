using PflegedientPlan.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PflegedientPlan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Patient SelectedPatient { get; set; }

        private readonly ObservableCollection<Patient> _patientList = new ObservableCollection<Patient>();
        private ObservableCollection<Activity> _activityList = new ObservableCollection<Activity>();
        private ObservableCollection<Category> _categoryList = new ObservableCollection<Category>();

        public MainWindow()
        {
            InitializeComponent();
            Init();

            userGrid.SelectionChanged += userGrid_SelectionChanged;
            activitysComboBox.SelectionChanged += activitysComboBox_SelectionChanged;
        }

        private void activitysComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && activitysComboBox.Items.Count > 0)
            {
                var activity = (e.AddedItems[0] as Activity);
                SetCategorysComboBoxItems(activity.Id);
            }

            if (categoryCombobox.Items.Count > 0 && activitysComboBox.Items.Count > 0)
            {
                categoryCombobox.SelectedIndex = 0;
            }
        }

        private void userGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var patient = (e.AddedItems[0] as Patient);

            if (patient == null)
                return;

            SelectedPatient = patient;

            // reload problemslist
            if (!StaticHolder.SelectedProblems.ContainsKey(patient.PatientId))
            {
                StaticHolder.SelectedProblems.Add(patient.PatientId, new ObservableCollection<Problem>());
            }

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[patient.PatientId];

            if (!StaticHolder.SelectedResources.ContainsKey(patient.PatientId))
            {
                StaticHolder.SelectedResources.Add(patient.PatientId, new ObservableCollection<Resource>());
            }

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[patient.PatientId];

            if (!StaticHolder.SelectedTargets.ContainsKey(patient.PatientId))
            {
                StaticHolder.SelectedTargets.Add(patient.PatientId, new ObservableCollection<Target>());
            }

            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[patient.PatientId];
        }

        #region Init program, load items async
        private async void Init()
        {
            await LoadUserAsync();
            await LoadActivitysAsync();
            await LoadCategorysAsync();

            if (_activityList.Count > 0)
            {
                activitysComboBox.SelectedIndex = 0;
                
                if (_categoryList.Count > 0)
                {
                    categoryCombobox.SelectedIndex = 0;
                }
            }

            await Task.Run(() => Thread.Sleep(1000));

            ClearValue(HeightProperty);
            ClearValue(WidthProperty);
            ClearValue(MaxHeightProperty);
            ClearValue(MaxWidthProperty);
            ClearValue(MinHeightProperty);
            ClearValue(MinWidthProperty);
            progressGrid.Visibility = System.Windows.Visibility.Hidden;
            mainGrid.Visibility = System.Windows.Visibility.Visible;
            WindowState = System.Windows.WindowState.Maximized;
        }
        #endregion

        private void SetCategorysComboBoxItems(int ParentId)
        {
            if (_activityList.Count > 0)
            {
                var categorys = _categoryList.Select(c => c).Where(c => c.ParentId == ParentId).ToList();
                categoryCombobox.ItemsSource = categorys;
            }

            if (categoryCombobox.Items.Count > 0)
                categoryCombobox.SelectedIndex = 0;
        }

        #region Load user data async
        private async Task LoadUserAsync()
        {
            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    using (var reader = await client.SelectAsync("SELECT * FROM patienten"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var patient = new Patient()
                                {
                                    PatientId = reader.GetInt32(0),
                                    PatientVorname = reader.GetString(1),
                                    PatientNachname = reader.GetString(2),
                                    PatientGeburtsdatum = reader.GetString(3),
                                    PatientGender = (Gender)reader.GetInt32(4)
                                };

                                _patientList.Add(patient);
                            }
                        }
                    }
                }
            }

            userGrid.ItemsSource = _patientList;
        }
        #endregion

        #region Load activitys async
        private async Task LoadActivitysAsync()
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
        #endregion

        #region Load categorys async
        private async Task LoadCategorysAsync()
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
        }
        #endregion

        #region Add new user to database
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(patNrBox.Text) && !string.IsNullOrEmpty(patVornameBox.Text) && !string.IsNullOrEmpty(patNachnameBox.Text) && !string.IsNullOrEmpty(patGeburtsDatumBox.Text))
                {
                    var patient = new Patient()
                    {
                        PatientId = int.Parse(patNrBox.Text),
                        PatientVorname = patVornameBox.Text,
                        PatientNachname = patNachnameBox.Text,
                        PatientGeburtsdatum = patGeburtsDatumBox.Text,
                        PatientGender = (patGender.SelectedIndex == 0) ? Gender.MALE : Gender.FEMALE
                    };

                    if (patient.PatientId != null && !string.IsNullOrEmpty(patient.PatientVorname) && !string.IsNullOrEmpty(patient.PatientNachname) && !string.IsNullOrEmpty(patient.PatientGeburtsdatum))
                    {
                        var patientExist = _patientList.Select(p => p).Where(p => p.PatientId == patient.PatientId).FirstOrDefault();

                        if (patientExist != null)
                        {
                            MessageBox.Show("Dieser Patient wurde bereits hinzugefügt! (" + patient.PatientVorname + " " + patient.PatientNachname + ")", "Doppelter Eintrag", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        await AddPatientToDatabase(patient);

                        _patientList.Add(patient);
                        userGrid.ItemsSource = _patientList;

                        patNrBox.Text = "";
                        patVornameBox.Text = "";
                        patNachnameBox.Text = "";
                        patGeburtsDatumBox.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }
        }
        #endregion

        private async void WriteException(Exception ex)
        {
            await Logger.WriteException(ex.ToString());
        }

        private async Task AddPatientToDatabase(Patient patient)
        {
            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@patnr", patient.PatientId);
                    client.AddParam<string>("@vorname", patient.PatientVorname);
                    client.AddParam<string>("@nachname", patient.PatientNachname);
                    client.AddParam<string>("@gebdat", patient.PatientGeburtsdatum);
                    client.AddParam<int>("@gender", (int)patient.PatientGender);

                    var state = await client.InsertAsync("INSERT INTO patienten (pat_nr, pat_vorname, pat_nachname, pat_geburtsdatum, pat_gender) VALUES (@patnr, @vorname, @nachname, @gebdat, @gender);");
                }
            }
        }

        private void addActivityMenu_Click(object sender, RoutedEventArgs e)
        {
            var activity = new AddActivity();
            activity.OnMainNeedsAnUpdate += activity_OnMainNeedsAnUpdate;
            activity.Show();
        }

        private void activity_OnMainNeedsAnUpdate(ObservableCollection<Activity> activityList)
        {
            _activityList = activityList;
            activitysComboBox.ItemsSource = _activityList;

            if (_activityList.Count == 0)
            {
                categoryCombobox.ItemsSource = null; // ?
            }

            // set selectedindex on 0, so the first item is displayed to the customer
            if (activitysComboBox.Items.Count > 0)
                activitysComboBox.SelectedIndex = 0;
            else
                activitysComboBox.SelectedIndex = -1;
        }

        private void addCategoryMenu_Click(object sender, RoutedEventArgs e)
        {
            var category = new AddCategory();
            category.OnMainNeedsAnUpdate += category_OnMainNeedsAnUpdate;
            category.Show();
        }

        private void category_OnMainNeedsAnUpdate(ObservableCollection<Category> categoryList)
        {
            _categoryList = categoryList;
            categoryCombobox.ItemsSource = _categoryList;

            var selectedActivity = activitysComboBox.SelectedItem;
            if (selectedActivity != null)
            {
                var activity = (selectedActivity as Activity);
                SetCategorysComboBoxItems(activity.Id);
            }
        }

        private async void contextMenuDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (userGrid.SelectedItem as Patient);

            if (selectedPatient == null)
                return;

            var result = MessageBox.Show("\"" + selectedPatient.PatientVorname + " " + selectedPatient.PatientNachname + "\"\nDen Patienten wirklich löschen?", "Löschen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await DeletePatientAsync(selectedPatient);
            }
        }

        private async Task DeletePatientAsync(Patient patient)
        {
            try
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@patient_id", patient.PatientId);
                        var result = await client.ExecuteAsync("DELETE FROM patienten WHERE pat_nr = @patient_id;");

                        if (result > 0)
                        {
                            MessageBox.Show("Der Patient wurde gelöscht!", "Gelöscht", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        _patientList.Remove(patient);
                    }
                }
            }
            catch
            {
            }
        }

        private void mainMenuClose_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Das Programm jetzt beenden?", "Beenden", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void openAddNewResourcesWindow_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (userGrid.SelectedItem as Patient);

            if (selectedPatient == null)
            {
                MessageBox.Show("Es wurde kein Patient ausgewählt.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedActivity = (activitysComboBox.SelectedItem as Activity);

            if (selectedActivity == null)
            {
                MessageBox.Show("Es wurde keine Aktivität ausgewählt.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedCategory = (categoryCombobox.SelectedItem as Category);

            if (selectedCategory == null)
            {
                MessageBox.Show("Es wurde keine Kategorie ausgewählt.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addToMaskWindow = new AddToMask(selectedPatient, selectedActivity, selectedCategory);
            addToMaskWindow.OnProblemListUpdated += addToMaskWindow_OnProblemListUpdated;
            addToMaskWindow.OnResourceListUpdated += addToMaskWindow_OnResourceListUpdated;
            addToMaskWindow.OnTargetListUpdated += addToMaskWindow_OnTargetListUpdated;
            addToMaskWindow.Show();
        }

        #region Problems listbox
        private void addToMaskWindow_OnProblemListUpdated()
        {
            if (SelectedPatient == null)
                return;

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[SelectedPatient.PatientId];
        }

        private void problemsListBoxItemUP_Click(object sender, RoutedEventArgs e)
        {
            var selectedProblem = (problemsDataGrid.SelectedItem as Problem);

            if (selectedProblem == null)
                return;

            int currentItemPosition = selectedProblem.Position;

            if (currentItemPosition == 0)
            {
                return;
            }

            var itemOnPositionAbove = (problemsDataGrid.Items[currentItemPosition - 1] as Problem);

            selectedProblem.Position -= 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position += 1;
            }

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[SelectedPatient.PatientId].OrderBy(p => p.Position).ToList();
        }

        private void problemsListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedProblem = (problemsDataGrid.SelectedItem as Problem);

            if (selectedProblem == null)
                return;

            int currentItemPosition = selectedProblem.Position;

            if (currentItemPosition == problemsDataGrid.Items.Count - 1)
            {
                return;
            }

            var itemOnPositionAbove = (problemsDataGrid.Items[currentItemPosition + 1] as Problem);

            selectedProblem.Position += 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position -= 1;
            }

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[SelectedPatient.PatientId].OrderBy(p => p.Position).ToList();
        }

        private void problemsListBoxItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedProblem = (problemsDataGrid.SelectedItem as Problem);

            if (selectedProblem == null)
                return;

            StaticHolder.SelectedProblems[SelectedPatient.PatientId].Remove(selectedProblem);
            UpdateProblemsPositions(selectedProblem.Position);
        }

        private void UpdateProblemsPositions(int fromId)
        {
            if (SelectedPatient == null)
                return;

            foreach (var item in StaticHolder.SelectedProblems[SelectedPatient.PatientId].Select(p => p).Where(p => p.Position > fromId).ToList())
            {
                item.Position -= 1;
            }

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[SelectedPatient.PatientId].OrderBy(p => p.Position).ToList();
        }
        #endregion

        #region Resources listbox
        private void addToMaskWindow_OnResourceListUpdated()
        {
            if (SelectedPatient == null)
                return;

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[SelectedPatient.PatientId];
        }

        private void resourcesListBoxItemUP_Click(object sender, RoutedEventArgs e)
        {
            var selectedResource = (resourcesDataGrid.SelectedItem as Resource);

            if (selectedResource == null)
                return;

            int currentItemPosition = selectedResource.Position;

            if (currentItemPosition == 0)
            {
                return;
            }

            var itemOnPositionAbove = (resourcesDataGrid.Items[currentItemPosition - 1] as Resource);

            selectedResource.Position -= 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position += 1;
            }

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[SelectedPatient.PatientId].OrderBy(r => r.Position).ToList();
        }

        private void resourcesListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedResource = (resourcesDataGrid.SelectedItem as Resource);

            if (selectedResource == null)
                return;

            int currentItemPosition = selectedResource.Position;

            if (currentItemPosition == resourcesDataGrid.Items.Count - 1)
            {
                return;
            }

            var itemOnPositionAbove = (resourcesDataGrid.Items[currentItemPosition + 1] as Resource);

            selectedResource.Position += 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position -= 1;
            }

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[SelectedPatient.PatientId].OrderBy(r => r.Position).ToList();
        }

        private void resourcesListBoxItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedResource = (resourcesDataGrid.SelectedItem as Resource);

            if (selectedResource == null)
                return;

            StaticHolder.SelectedResources[SelectedPatient.PatientId].Remove(selectedResource);
            UpdateResourcesPositions(selectedResource.Position);
        }

        private void UpdateResourcesPositions(int fromId)
        {
            if (SelectedPatient == null)
                return;

            foreach (var item in StaticHolder.SelectedResources[SelectedPatient.PatientId].Select(r => r).Where(r => r.Position > fromId).ToList())
            {
                item.Position -= 1;
            }

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[SelectedPatient.PatientId].OrderBy(r => r.Position).ToList();
        }
        #endregion

        #region Targets listbox
        private void addToMaskWindow_OnTargetListUpdated()
        {
            if (SelectedPatient == null)
                return;

            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[SelectedPatient.PatientId];
        }

        private void targetsListBoxItemUP_Click(object sender, RoutedEventArgs e)
        {
            var selectedTarget = (targetsDataGrid.SelectedItem as Target);

            if (selectedTarget == null)
                return;

            int currentItemPosition = selectedTarget.Position;

            if (currentItemPosition == 0)
            {
                return;
            }

            var itemOnPositionAbove = (targetsDataGrid.Items[currentItemPosition - 1] as Target);

            selectedTarget.Position -= 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position += 1;
            }

            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[SelectedPatient.PatientId].OrderBy(t => t.Position).ToList();
        }

        private void targetsListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedTarget = (targetsDataGrid.SelectedItem as Target);

            if (selectedTarget == null)
                return;

            int currentItemPosition = selectedTarget.Position;

            if (currentItemPosition == targetsDataGrid.Items.Count - 1)
            {
                return;
            }

            var itemOnPositionAbove = (targetsDataGrid.Items[currentItemPosition + 1] as Target);

            selectedTarget.Position += 1;

            if (itemOnPositionAbove != null)
            {
                itemOnPositionAbove.Position -= 1;
            }

            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[SelectedPatient.PatientId].OrderBy(t => t.Position).ToList();
        }

        private void targetsListBoxItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedTarget = (targetsDataGrid.SelectedItem as Target);

            if (selectedTarget == null)
                return;

            StaticHolder.SelectedTargets[SelectedPatient.PatientId].Remove(selectedTarget);
            UpdateTargetsPositions(selectedTarget.Position);
        }

        private void UpdateTargetsPositions(int fromId)
        {
            if (SelectedPatient == null)
                return;

            foreach (var item in StaticHolder.SelectedTargets[SelectedPatient.PatientId].Select(t => t).Where(t => t.Position > fromId).ToList())
            {
                item.Position -= 1;
            }

            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[SelectedPatient.PatientId].OrderBy(t => t.Position).ToList();
        }
        #endregion
    }
}
