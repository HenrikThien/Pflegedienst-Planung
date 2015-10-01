using PflegedientPlan.Classes;
using PflegedientPlan.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
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
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Web.UI;
using System.Windows.Controls.Primitives;
using System.Collections;
using iTextSharp.text.pdf.draw;

namespace PflegedientPlan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Patient SelectedPatient { get; set; }

        private readonly ObservableCollection<Patient> _patientList = new ObservableCollection<Patient>();
        private ObservableCollection<Activity> _activityList = new ObservableCollection<Activity>();
        private ObservableCollection<Category> _categoryList = new ObservableCollection<Category>();

        public MainWindow()
        {
            InitializeComponent();
            Init();

            userGrid.SelectionChanged += userGrid_SelectionChanged;
            activitysComboBox.SelectionChanged += activitysComboBox_SelectionChanged;

            problemsDataGrid.CellEditEnding += problemsDataGrid_CellEditEnding;
            resourcesDataGrid.CellEditEnding += resourcesDataGrid_CellEditEnding;
            targetsDataGrid.CellEditEnding += targetsDataGrid_CellEditEnding;
            measuresDataGrid.CellEditEnding += measuresDataGrid_CellEditEnding;
        }

        #region cell edit ending events
        void measuresDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header == "Maßnahme")
            {
                var measureToEdit = (e.Row.Item as Measure);
                var editedElement = (e.EditingElement as TextBox);

                if (measureToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newMeasureDescription = editedElement.Text;

                int index = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].IndexOf(measureToEdit);
                StaticHolder.SelectedMeasures[SelectedPatient.PatientId][index].Description = newMeasureDescription;
            }
            else if (e.Column.Header == "Häufigkeit")
            {
                var measureToEdit = (e.Row.Item as Measure);
                var editedElement = (e.EditingElement as TextBox);

                if (measureToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newFrequency = editedElement.Text;

                int index = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].IndexOf(measureToEdit);
                StaticHolder.SelectedMeasures[SelectedPatient.PatientId][index].Frequency = int.Parse(newFrequency);

            }
            else if (e.Column.Header == "Type")
            {
                var measureToEdit = (e.Row.Item as Measure);
                var editedElement = (e.EditingElement as TextBox);

                if (measureToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newType = editedElement.Text;

                int index = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].IndexOf(measureToEdit);
                StaticHolder.SelectedMeasures[SelectedPatient.PatientId][index].FrequencyType = (FrequencyType)int.Parse(newType);
            }
            else if (e.Column.Header == "#Pos.")
            {
                var measureToEdit = (e.Row.Item as Measure);
                var editedElement = (e.EditingElement as TextBox);

                if (measureToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newPosition = editedElement.Text;

                int index = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].IndexOf(measureToEdit);
                StaticHolder.SelectedMeasures[SelectedPatient.PatientId][index].Position = int.Parse(newPosition);
            }
        }

        void targetsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header == "Ziel")
            {
                var targetToEdit = (e.Row.Item as Target);
                var editedElement = (e.EditingElement as TextBox);

                if (targetToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newTargetDescription = editedElement.Text;

                int index = StaticHolder.SelectedTargets[SelectedPatient.PatientId].IndexOf(targetToEdit);
                StaticHolder.SelectedTargets[SelectedPatient.PatientId][index].Description = newTargetDescription;
            }
            else if (e.Column.Header == "#Pos.")
            {
                var targetToEdit = (e.Row.Item as Target);
                var editedElement = (e.EditingElement as TextBox);

                if (targetToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newTargetPosition = editedElement.Text;

                int index = StaticHolder.SelectedTargets[SelectedPatient.PatientId].IndexOf(targetToEdit);
                StaticHolder.SelectedTargets[SelectedPatient.PatientId][index].Position = int.Parse(newTargetPosition);
            }
        }

        void resourcesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header == "Ressource")
            {
                var resourceToEdit = (e.Row.Item as Resource);
                var editedElement = (e.EditingElement as TextBox);

                if (resourceToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newResourceDescription = editedElement.Text;

                int index = StaticHolder.SelectedResources[SelectedPatient.PatientId].IndexOf(resourceToEdit);
                StaticHolder.SelectedResources[SelectedPatient.PatientId][index].Description = newResourceDescription;
            }
            else if (e.Column.Header == "#Pos.")
            {
                var resourceToEdit = (e.Row.Item as Resource);
                var editedElement = (e.EditingElement as TextBox);

                if (resourceToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newResourcePosition = editedElement.Text;

                int index = StaticHolder.SelectedResources[SelectedPatient.PatientId].IndexOf(resourceToEdit);
                StaticHolder.SelectedResources[SelectedPatient.PatientId][index].Position = int.Parse(newResourcePosition);
            }
        }

        void problemsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header == "Problem")
            {
                var problemToEdit = (e.Row.Item as Problem);
                var editedElement = (e.EditingElement as TextBox);

                if (problemToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newProblemDescription = editedElement.Text;

                int index = StaticHolder.SelectedProblems[SelectedPatient.PatientId].IndexOf(problemToEdit);
                StaticHolder.SelectedProblems[SelectedPatient.PatientId][index].Description = newProblemDescription;
            }
            else if (e.Column.Header == "#Pos.")
            {
                var problemToEdit = (e.Row.Item as Problem);
                var editedElement = (e.EditingElement as TextBox);

                if (problemToEdit == null)
                    return;
                if (editedElement == null)
                    return;

                var newProblemPosition = editedElement.Text;

                int index = StaticHolder.SelectedProblems[SelectedPatient.PatientId].IndexOf(problemToEdit);
                StaticHolder.SelectedProblems[SelectedPatient.PatientId][index].Position = int.Parse(newProblemPosition);
            }
        }
        #endregion

        #region Activity combobox selection changed
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
        #endregion

        #region Usergrid selection changed
        private void userGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0)
                return;

            var patient = (e.AddedItems[0] as Patient);

            if (patient == null)
                return;

            SelectedPatient = patient;

            // reload lists
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

            if (!StaticHolder.SelectedMeasures.ContainsKey(patient.PatientId))
            {
                StaticHolder.SelectedMeasures.Add(patient.PatientId, new ObservableCollection<Measure>());
            }

            measuresDataGrid.ItemsSource = StaticHolder.SelectedMeasures[patient.PatientId];
        }
        #endregion

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
            if (handlePatientBtn.Content.ToString() == "Bearbeiten")
            {
                if (!string.IsNullOrEmpty(patNrBox.Text) && !string.IsNullOrEmpty(patVornameBox.Text) && !string.IsNullOrEmpty(patNachnameBox.Text) && !string.IsNullOrEmpty(patGeburtsDatumBox.Text))
                {
                    var patient = _patientList.Select(p => p).Where(p => p.PatientId == int.Parse(patNrBox.Text)).FirstOrDefault();

                    if (patient != null)
                    {
                        patient.PatientId = int.Parse(patNrBox.Text);
                        patient.PatientVorname = patVornameBox.Text;
                        patient.PatientNachname = patNachnameBox.Text;
                        patient.PatientGeburtsdatum = patGeburtsDatumBox.Text;
                        patient.PatientGender = (patGender.SelectedIndex == 0) ? Gender.MALE : Gender.FEMALE;

                        if (patient.PatientId != null && !string.IsNullOrEmpty(patient.PatientVorname) && !string.IsNullOrEmpty(patient.PatientNachname) && !string.IsNullOrEmpty(patient.PatientGeburtsdatum))
                        {
                            await UpdatePatientInDatabase(patient);
                            userGrid.ItemsSource = null;
                            userGrid.ItemsSource = _patientList;

                            patNrBox.Text = "";
                            patVornameBox.Text = "";
                            patNachnameBox.Text = "";
                            patGeburtsDatumBox.Text = "";
                            patGender.SelectedIndex = 0;

                            handlePatientBtn.Content = "Hinzufügen";
                        }
                    }
                }
            }

            if (handlePatientBtn.Content.ToString() == "Hinzufügen")
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
                            patGender.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                }
            }
        }
        
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

        private async Task UpdatePatientInDatabase(Patient patient)
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

                    var state = await client.InsertAsync("UPDATE patienten SET pat_nr = @patnr, pat_vorname = @vorname, pat_nachname = @nachname, pat_geburtsdatum = @gebdat, pat_gender = @gender WHERE pat_nr = @patnr;");
                }
            }
        }
        #endregion

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

        private void contextMenuEditPatient_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = (userGrid.SelectedItem as Patient);

            if (selectedPatient == null)
                return;

            handlePatientBtn.Content = "Bearbeiten";

            patNrBox.Text = selectedPatient.PatientId.ToString();
            patVornameBox.Text = selectedPatient.PatientVorname;
            patNachnameBox.Text = selectedPatient.PatientNachname;
            patGeburtsDatumBox.Text = selectedPatient.PatientGeburtsdatum;
            patGender.SelectedIndex = (selectedPatient.PatientGender == Gender.MALE) ? 0 : 1;
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

                        client.ClearParameter();

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
            addToMaskWindow.OnMeasureListUpdated += addToMaskWindow_OnMeasureListUpdated;
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

            selectedProblem.Position -= 1;

            problemsDataGrid.ItemsSource = StaticHolder.SelectedProblems[SelectedPatient.PatientId].OrderBy(p => p.Position).ToList();
        }

        private void problemsListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedProblem = (problemsDataGrid.SelectedItem as Problem);

            if (selectedProblem == null)
                return;

            int currentItemPosition = selectedProblem.Position;

            if (currentItemPosition == problemsDataGrid.Items.Count)
            {
                return;
            }

            selectedProblem.Position += 1;

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

            selectedResource.Position -= 1;

            resourcesDataGrid.ItemsSource = StaticHolder.SelectedResources[SelectedPatient.PatientId].OrderBy(r => r.Position).ToList();
        }

        private void resourcesListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedResource = (resourcesDataGrid.SelectedItem as Resource);

            if (selectedResource == null)
                return;

            int currentItemPosition = selectedResource.Position;

            if (currentItemPosition == resourcesDataGrid.Items.Count)
            {
                return;
            }

            selectedResource.Position += 1;

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

            selectedTarget.Position -= 1;
            targetsDataGrid.ItemsSource = StaticHolder.SelectedTargets[SelectedPatient.PatientId].OrderBy(t => t.Position).ToList();
        }

        private void targetsListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedTarget = (targetsDataGrid.SelectedItem as Target);

            if (selectedTarget == null)
                return;

            int currentItemPosition = selectedTarget.Position;

            if (currentItemPosition == targetsDataGrid.Items.Count)
            {
                return;
            }

            selectedTarget.Position += 1;
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

        #region Measures listbox
        private void addToMaskWindow_OnMeasureListUpdated()
        {
            if (SelectedPatient == null)
                return;

            measuresDataGrid.ItemsSource = StaticHolder.SelectedMeasures[SelectedPatient.PatientId];
        }

        private void measuresListBoxItemUP_Click(object sender, RoutedEventArgs e)
        {
            var selectedMeasure = (measuresDataGrid.SelectedItem as Measure);

            if (selectedMeasure == null)
                return;

            int currentItemPosition = selectedMeasure.Position;

            if (currentItemPosition == 0)
            {
                return;
            }

            selectedMeasure.Position -= 1;
            measuresDataGrid.ItemsSource = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].OrderBy(m => m.Position).ToList();
        }

        private void measuresListBoxItemDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedMeasure = (measuresDataGrid.SelectedItem as Measure);

            if (selectedMeasure == null)
                return;

            int currentItemPosition = selectedMeasure.Position;

            if (currentItemPosition == measuresDataGrid.Items.Count)
            {
                return;
            }

            selectedMeasure.Position += 1;
            measuresDataGrid.ItemsSource = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].OrderBy(m => m.Position).ToList();
        }

        private void measuresListBoxItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedMeasure = (measuresDataGrid.SelectedItem as Measure);

            if (selectedMeasure == null)
                return;

            StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Remove(selectedMeasure);
            UpdateMeasuresPositions(selectedMeasure.Position);
        }

        private void UpdateMeasuresPositions(int fromId)
        {
            if (SelectedPatient == null)
                return;

            foreach (var item in StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Select(m => m).Where(m => m.Position > fromId).ToList())
            {
                item.Position -= 1;
            }

            measuresDataGrid.ItemsSource = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].OrderBy(m => m.Position).ToList();
        }
        #endregion

        private void menuContextAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        #region print
        private void menuSaveAsPDF_Click(object sender, RoutedEventArgs e)
        {
            LoadExportToPDFAsync();
        }

        private async void LoadExportToPDFAsync()
        {
            await ExportSelectedToPdf();
        }

        private async Task ExportSelectedToPdf()
        {
            var allItems = new List<iSuperItem>();
            var formattedText = new Dictionary<int, Dictionary<int, List<iSuperItem>>>();

            await Task.Factory.StartNew(() =>
            {
                allItems.AddRange((from i in StaticHolder.SelectedProblems[SelectedPatient.PatientId] select i).ToList());
                allItems.AddRange((from i in StaticHolder.SelectedResources[SelectedPatient.PatientId] select i).ToList());
                allItems.AddRange((from i in StaticHolder.SelectedTargets[SelectedPatient.PatientId] select i).ToList());
                allItems.AddRange((from i in StaticHolder.SelectedMeasures[SelectedPatient.PatientId] select i).ToList());


                for (int i = 0; i < allItems.Count; i++)
                {
                    var itemsOnPos = (from x in allItems where x.SuperPosition == i select x);

                    foreach (var item in itemsOnPos)
                    {
                        if (!formattedText.ContainsKey(i))
                        {
                            formattedText.Add(i, new Dictionary<int, List<iSuperItem>>());
                        }

                        if (item.GetType() == typeof(Problem))
                        {
                            if (!formattedText[i].ContainsKey(0))
                            {
                                formattedText[i].Add(0, new List<iSuperItem>());
                                formattedText[i][0].Add(item);
                            }
                            else
                            {
                                formattedText[i][0].Add(item);
                            }
                        }
                        else if (item.GetType() == typeof(Resource))
                        {
                            if (!formattedText[i].ContainsKey(1))
                            {
                                formattedText[i].Add(1, new List<iSuperItem>());
                                formattedText[i][1].Add(item);
                            }
                            else
                            {
                                formattedText[i][1].Add(item);
                            }
                        }
                        else if (item.GetType() == typeof(Target))
                        {
                            if (!formattedText[i].ContainsKey(2))
                            {
                                formattedText[i].Add(2, new List<iSuperItem>());
                                formattedText[i][2].Add(item);
                            }
                            else
                            {
                                formattedText[i][2].Add(item);
                            }
                        }
                        else if (item.GetType() == typeof(Measure))
                        {
                            (item as Measure).SetSuperFrequency();

                            if (!formattedText[i].ContainsKey(3))
                            {
                                formattedText[i].Add(3, new List<iSuperItem>());
                                formattedText[i][3].Add(item);
                            }
                            else
                            {
                                formattedText[i][3].Add(item);
                            }
                        }
                    }
                }
            });

            await CreatePdf(formattedText);
        }

        private async Task CreatePdf(Dictionary<int, Dictionary<int, List<iSuperItem>>> FormattedText)
        {
            var table = new PdfPTable(10);

            await Task.Factory.StartNew(() =>
            {
                table.TotalWidth = PageSize.A3.Rotate().Width - 20; // -20 margin space
                table.HorizontalAlignment = 0;
                table.LockedWidth = true;
                float[] widths = new float[] { 20f, 80f, 80f, 80f, 80f, 30f, 20f, 10f, 20, 10f };
                table.SetWidths(widths);

                int fixedPosition = 1;

                foreach (var positions in FormattedText)
                {
                    int problemCount = (FormattedText.ContainsKey(positions.Key) ? FormattedText[positions.Key].ContainsKey(0) ? FormattedText[positions.Key][0].Count : 0 : 0);
                    int resourceCount = (FormattedText.ContainsKey(positions.Key) ? FormattedText[positions.Key].ContainsKey(1) ? FormattedText[positions.Key][1].Count : 0 : 0);
                    int targetCount = (FormattedText.ContainsKey(positions.Key) ? FormattedText[positions.Key].ContainsKey(2) ? FormattedText[positions.Key][2].Count : 0 : 0);
                    int measureCount = (FormattedText.ContainsKey(positions.Key) ? FormattedText[positions.Key].ContainsKey(3) ? FormattedText[positions.Key][3].Count : 0 : 0);
                    int highest = problemCount;

                    if (resourceCount > highest)
                        highest = resourceCount;
                    if (targetCount > highest)
                        highest = targetCount;
                    if (measureCount > highest)
                        highest = measureCount;

                    var frequencyQueue = new Queue<string>();

                    AddCellToTable(table, fixedPosition + ".", false, highest);

                    for (int pos = 0; pos < highest; pos++)
                    {
                        for (int i = 0; i <= 8; i++)
                        {
                            bool needToAddCell = true;

                            if (i <= 3)
                            {
                                if (FormattedText.ContainsKey(positions.Key))
                                {
                                    if (FormattedText[positions.Key].ContainsKey(i))
                                    {
                                        if (FormattedText[positions.Key][i].ElementAtOrDefault(pos) != null)
                                        {
                                            if (pos == 0)
                                            {
                                                var text = FormattedText[positions.Key][i].ElementAtOrDefault(pos).SuperDescription;

                                                //if (i == 3)
                                                //{
                                                //    var frequency = FormattedText[positions.Key][i].ElementAtOrDefault(pos).SuperFrequency.ToLower();

                                                //    if (frequency != null)
                                                //    {
                                                //        frequencyQueue.Enqueue(frequency);
                                                //    }
                                                //    else
                                                //    {
                                                //        frequencyQueue.Enqueue("");
                                                //    }
                                                //}

                                                AddCellToTable(table, text, false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER);
                                                needToAddCell = false;
                                            }
                                            else
                                            {
                                                var text = FormattedText[positions.Key][i].ElementAtOrDefault(pos).SuperDescription;

                                                //if (i == 3)
                                                //{
                                                //    var frequency = FormattedText[positions.Key][i].ElementAtOrDefault(pos).SuperFrequency.ToLower();

                                                //    if (frequency != null)
                                                //    {
                                                //        frequencyQueue.Enqueue(frequency);
                                                //    }
                                                //    else
                                                //    {
                                                //        frequencyQueue.Enqueue("");
                                                //    }
                                                //}

                                                AddCellToTable(table, text, false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER);
                                                needToAddCell = false;
                                            }
                                        }
                                    }
                                }
                            }

                            //if (i == 4)
                            //{
                            //    if (pos == 0)
                            //    {
                            //        var frequency = (frequencyQueue.Count > 0) ? frequencyQueue.Dequeue() : "";
                            //        AddCellToTable(table, frequency, false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER);
                            //        needToAddCell = false;
                            //    }
                            //    else
                            //    {
                            //        var frequency = (frequencyQueue.Count > 0) ? frequencyQueue.Dequeue() : "";
                            //        AddCellToTable(table, frequency, false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER);
                            //        needToAddCell = false;
                            //    }
                            //}

                            if (needToAddCell)
                            {
                                if (pos == 0)
                                {
                                    AddCellToTable(table, "", false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.TOP_BORDER);
                                }
                                else
                                {
                                    AddCellToTable(table, "", false, 1, iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER);
                                }
                            }
                        }
                    }

                    fixedPosition++;
                }

                AddFooterToTable(table, "");
            });

            Dispatcher.Invoke(
                new Action(delegate()
                    {
                        var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                        saveFileDialog.Filter = "PDF Dateien|*.pdf";
                        saveFileDialog.Title = "Als PDF speichern";
                        saveFileDialog.ValidateNames = true;
                        saveFileDialog.FileName = SelectedPatient.PatientId + " - " + SelectedPatient.PatientVorname + " " + SelectedPatient.PatientNachname + ".pdf";
                        saveFileDialog.ShowDialog();

                        if (saveFileDialog.FileName != "")
                        {
                            var document = new Document(PageSize.A3.Rotate(), 10f, 10f, 10f, 20f);
                            var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create));
                            writer.PageEvent = new CustomPageEventHandler();
                            document.Open();

                            document.Add(table);
                            document.Close();
                        }
                    }));
        }

        private void AddFooterToTable(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell.Colspan = 10;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AddCellToTable(PdfPTable table, string text, bool header = false, int span = 1, int border = iTextSharp.text.Rectangle.TOP_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER)
        {
            FontFactory.RegisterDirectories();
            var fontArial = new Font(FontFactory.GetFont("Arial", 11, Font.NORMAL));

            if (header)
                fontArial.SetStyle(Font.BOLD);

            PdfPCell cell = new PdfPCell(new Phrase(text, fontArial));
            cell.Border = border;
            cell.Rowspan = span;
            cell.Padding = 5;
            cell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
            table.AddCell(cell);
        }
        #endregion

        #region Save & Load
        private async void mainMenuLoadPatient_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient != null)
            {
                var result = MessageBox.Show("Die Daten für den Patienten " + SelectedPatient.PatientVorname + " " + SelectedPatient.PatientNachname + " laden?", "Laden", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var fileManager = new FileManager(SelectedPatient))
                    {
                        var mask = await fileManager.LoadPatient();

                        if (mask == null)
                        {
                            return;
                        }

                        StaticHolder.SelectedProblems[SelectedPatient.PatientId] = mask.Problems;
                        StaticHolder.SelectedResources[SelectedPatient.PatientId] = mask.Resources;
                        StaticHolder.SelectedTargets[SelectedPatient.PatientId] = mask.Targets;
                        StaticHolder.SelectedMeasures[SelectedPatient.PatientId] = mask.Measures;

                        addToMaskWindow_OnProblemListUpdated();
                        addToMaskWindow_OnResourceListUpdated();
                        addToMaskWindow_OnTargetListUpdated();
                        addToMaskWindow_OnMeasureListUpdated();
                    }
                }
            }
            else
            {
                MessageBox.Show("Es wurde kein Patient ausgewählt!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void mainMenuSavePatient_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient != null)
            {
                var result = MessageBox.Show("Die Daten für den Patienten " + SelectedPatient.PatientVorname + " " + SelectedPatient.PatientNachname + " speichern?", "Speichern", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (var fileManager = new FileManager(SelectedPatient))
                    {
                        await fileManager.SavePatient();
                    }
                }
            }
            else
            {
                MessageBox.Show("Es wurde kein Patient ausgewählt!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
