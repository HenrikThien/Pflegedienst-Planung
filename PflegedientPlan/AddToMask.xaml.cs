using PflegedientPlan.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Interaction logic for AddToMask.xaml
    /// </summary>
    public partial class AddToMask : Window
    {
        public delegate void NewItemAddedToMain();
        public event NewItemAddedToMain OnProblemListUpdated;
        public event NewItemAddedToMain OnResourceListUpdated;
        public event NewItemAddedToMain OnTargetListUpdated;
        public event NewItemAddedToMain OnMeasureListUpdated;

        private Patient SelectedPatient { get; set; }
        private Activity SelectedActivity { get; set; }
        private Category SelectedCategory { get; set; }

        private readonly ObservableCollection<Problem> _problemsList = new ObservableCollection<Problem>();
        private readonly ObservableCollection<Resource> _resourcesList = new ObservableCollection<Resource>();
        private readonly ObservableCollection<Target> _targetsList = new ObservableCollection<Target>();
        private readonly ObservableCollection<Measure> _measuresList = new ObservableCollection<Measure>();

        public AddToMask(Patient patient, Activity activity, Category category)
        {
            SelectedPatient = patient;
            SelectedActivity = activity;
            SelectedCategory = category;

            InitializeComponent();

            for (int i = 1; i <= 200; i++)
            {
                measureFrequencyComboBox.Items.Add(i.ToString());
            }
            measureFrequencyComboBox.SelectedIndex = 0;
            
            Init();
        }

        #region Init
        private async void Init()
        {
            await LoadProblemsAsync();
            await LoadResourcesAsync();
            await LoadTargetsAsync();
            await LoadMeasuresAsync();

            // check if patient already got a list
            if (!StaticHolder.SelectedProblems.ContainsKey(SelectedPatient.PatientId))
            {
                StaticHolder.SelectedProblems.Add(SelectedPatient.PatientId, new ObservableCollection<Problem>());
            }

            if (!StaticHolder.SelectedResources.ContainsKey(SelectedPatient.PatientId))
            {
                StaticHolder.SelectedResources.Add(SelectedPatient.PatientId, new ObservableCollection<Resource>());
            }

            if (!StaticHolder.SelectedTargets.ContainsKey(SelectedPatient.PatientId))
            {
                StaticHolder.SelectedTargets.Add(SelectedPatient.PatientId, new ObservableCollection<Target>());
            }

            if (!StaticHolder.SelectedMeasures.ContainsKey(SelectedPatient.PatientId))
            {
                StaticHolder.SelectedMeasures.Add(SelectedPatient.PatientId, new ObservableCollection<Measure>());
            }
        }

        private async Task LoadSelectedProblems()
        {
            await Task.Run(() =>
            {
                if (StaticHolder.SelectedProblems[SelectedPatient.PatientId].Count > 0)
                {
                    foreach (var problem in StaticHolder.SelectedProblems[SelectedPatient.PatientId])
                    {
                        Debug.WriteLine(problem.Description);

                        var item = (_problemsList.Select(p => p).Where(p => p.Id == problem.Id).FirstOrDefault());

                        if (item != null)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            });
        }

        private async Task LoadSelectedResources()
        {
            await Task.Run(() =>
            {
                if (StaticHolder.SelectedResources[SelectedPatient.PatientId].Count > 0)
                {
                    foreach (var resource in StaticHolder.SelectedResources[SelectedPatient.PatientId])
                    {
                        var item = (_resourcesList.Select(p => p).Where(p => p.Id == resource.Id).FirstOrDefault());

                        if (item != null)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            });
        }

        public async Task LoadSelectedTargets()
        {
            await Task.Run(() =>
            {
                if (StaticHolder.SelectedTargets[SelectedPatient.PatientId].Count > 0)
                {
                    foreach (var target in StaticHolder.SelectedTargets[SelectedPatient.PatientId])
                    {
                        var item = (_targetsList.Select(t => t).Where(t => t.Id == target.Id).FirstOrDefault());

                        if (item != null)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            });
        }

        public async Task LoadSelectedMeasures()
        {
            await Task.Run(() =>
            {
                if (StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Count > 0)
                {
                    foreach (var measure in StaticHolder.SelectedMeasures[SelectedPatient.PatientId])
                    {
                        var item = (_measuresList.Select(m => m).Where(m => m.Id == measure.Id).FirstOrDefault());

                        if (item != null)
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            });
        }
        #endregion

        #region Load problems async
        private async Task LoadProblemsAsync()
        {
            _problemsList.Clear();

            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@activity_id", SelectedActivity.Id);

                    using (var reader = await client.SelectAsync("SELECT * FROM problems WHERE activity_id = @activity_id;"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var problem = new Problem()
                                {
                                    Id = reader.GetInt32(0),
                                    Position = reader.GetInt32(1),
                                    Description = reader.GetString(2),
                                    IsChecked = false
                                };

                                problem.Description = ReplacePlaceholder(problem.Description);

                                _problemsList.Add(problem);
                                var realIndex = _problemsList.IndexOf(problem);
                                _problemsList.ElementAt(realIndex).RealListIndex = realIndex;
                            }
                        }
                    }

                    client.ClearParameter();
                }
            }

            await LoadSelectedProblems();
            problemsListBox.ItemsSource = _problemsList;
        }
        #endregion

        #region Load resources async
        private async Task LoadResourcesAsync()
        {
            _resourcesList.Clear();

            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@activity_id", SelectedActivity.Id);

                    using (var reader = await client.SelectAsync("SELECT * FROM resources WHERE activity_id = @activity_id;"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var resource = new Resource()
                                {
                                    Id = reader.GetInt32(0),
                                    Position = reader.GetInt32(1),
                                    Description = reader.GetString(2),
                                    IsChecked = false
                                };

                                resource.Description = ReplacePlaceholder(resource.Description);

                                _resourcesList.Add(resource);
                                var realIndex = _resourcesList.IndexOf(resource);
                                _resourcesList.ElementAt(realIndex).RealListIndex = realIndex;
                            }
                        }
                    }

                    client.ClearParameter();
                }
            }
            await LoadSelectedResources();
            resourcesListBox.ItemsSource = _resourcesList;
        }
        #endregion

        #region Load targets async
        private async Task LoadTargetsAsync()
        {
            _targetsList.Clear();

            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@activity_id", SelectedActivity.Id);

                    using (var reader = await client.SelectAsync("SELECT * FROM targets WHERE activity_id = @activity_id;"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var target = new Target()
                                {
                                    Id = reader.GetInt32(0),
                                    Position = reader.GetInt32(1),
                                    Description = reader.GetString(2),
                                    IsChecked = false
                                };

                                target.Description = ReplacePlaceholder(target.Description);

                                _targetsList.Add(target);
                                var realIndex = _targetsList.IndexOf(target);
                                _targetsList.ElementAt(realIndex).RealListIndex = realIndex;
                            }
                        }
                    }

                    client.ClearParameter();
                }
            }
            await LoadSelectedTargets();
            targetsListBox.ItemsSource = _targetsList;
        }
        #endregion

        #region Load measures async
        private async Task LoadMeasuresAsync()
        {
            _measuresList.Clear();

            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    client.AddParam<int>("@activity_id", SelectedActivity.Id);

                    using (var reader = await client.SelectAsync("SELECT * FROM measures WHERE activity_id = @activity_id;"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var measure = new Measure()
                                {
                                    Id = reader.GetInt32(0),
                                    Position = reader.GetInt32(1),
                                    Description = reader.GetString(2),
                                    IsChecked = false,
                                    Frequency = reader.GetInt32(4)
                                };

                                measure.Description = ReplacePlaceholder(measure.Description);

                                _measuresList.Add(measure);
                                var realIndex = _measuresList.IndexOf(measure);
                                _measuresList.ElementAt(realIndex).RealListIndex = realIndex;
                            }
                        }
                    }

                    client.ClearParameter();
                }
            }
            await LoadSelectedMeasures();
            measuresListBox.ItemsSource = _measuresList;
        }
        #endregion

        #region Add new problem to database
        private async void addNewProblemBtn_Click(object sender, RoutedEventArgs e)
        {
            var description = newProblemTextBox.Text;

            if (!string.IsNullOrEmpty(description))
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@pos", 0);
                        client.AddParam<string>("@desc", description);
                        client.AddParam<int>("@activity_id", SelectedActivity.Id);
                        client.ExecuteAsync("INSERT INTO problems (position, description, activity_id) VALUES (@pos, @desc, @activity_id);");
                        client.ClearParameter();
                    }
                }

                newProblemTextBox.Text = "";

                await LoadProblemsAsync();
                LoadSelectedProblems();
            }
        }
        #endregion

        #region Add new resource to database
        private async void addNewResourceBtn_Click(object sender, RoutedEventArgs e)
        {
            var description = newResourceTextBox.Text;

            if (!string.IsNullOrEmpty(description))
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@pos", 0);
                        client.AddParam<string>("@desc", description);
                        client.AddParam<int>("@activity_id", SelectedActivity.Id);
                        client.ExecuteAsync("INSERT INTO resources (position, description, activity_id) VALUES (@pos, @desc, @activity_id);");
                        client.ClearParameter();
                    }
                }

                newResourceTextBox.Text = "";
                await LoadResourcesAsync();
                LoadSelectedResources();
            }
        }
        #endregion

        #region Add new target to database
        private async void addNewTargetBtn_Click(object sender, RoutedEventArgs e)
        {
            var description = newTargetTextBox.Text;

            if (!string.IsNullOrEmpty(description))
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@pos", 0);
                        client.AddParam<string>("@desc", description);
                        client.AddParam<int>("@activity_id", SelectedActivity.Id);
                        client.ExecuteAsync("INSERT INTO targets (position, description, activity_id) VALUES (@pos, @desc, @activity_id);");
                        client.ClearParameter();
                    }
                }

                newTargetTextBox.Text = "";
                await LoadTargetsAsync();
                LoadSelectedTargets();
            }
        }
        #endregion

        #region Add new measure to database
        private async void addNewMeasureBtn_Click(object sender, RoutedEventArgs e)
        {
            var description = newMeasureTextBox.Text;
            var frequency = int.Parse(measureFrequencyComboBox.SelectedItem.ToString());

            if (!string.IsNullOrEmpty(description))
            {
                using (var client = new DatabaseClient())
                {
                    if (await client.OpenConnectionAsync())
                    {
                        client.AddParam<int>("@pos", 0);
                        client.AddParam<string>("@desc", description);
                        client.AddParam<int>("@activity_id", SelectedActivity.Id);
                        client.AddParam<int>("@frequency", frequency);
                        client.ExecuteAsync("INSERT INTO measures (position, description, activity_id, frequency) VALUES (@pos, @desc, @activity_id, @frequency);");
                        client.ClearParameter();
                    }
                }

                newMeasureTextBox.Text = "";
                measureFrequencyComboBox.SelectedIndex = 0;

                await LoadMeasuresAsync();
                LoadSelectedMeasures();
            }
        }
        #endregion

        #region Replace placeholder in string
        private string ReplacePlaceholder(string description)
        {
            var temp = description;
            string newDescription;

            var anrede = ((SelectedPatient.PatientGender == Gender.MALE) ? "Herr" : "Frau") + " " + SelectedPatient.PatientNachname;

            newDescription = temp.Replace("$", anrede);

            return newDescription;
        }
        #endregion

        #region Problem Check / Uncheck
        private void OnProblemChecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var problem = _problemsList.Select(p => p).Where(p => p.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (problem == null)
                return;

            var exists = StaticHolder.SelectedProblems[SelectedPatient.PatientId].Select(p => p).Where(p => p.Id == problem.Id).ToList();

            if (exists.Count <= 0)
            {
                problem.IsChecked = true;
                problem.Position = StaticHolder.SelectedProblems[SelectedPatient.PatientId].Count;
                StaticHolder.SelectedProblems[SelectedPatient.PatientId].Add(problem);
            }

            OnProblemListUpdated.Invoke();
        }

        private void OnProblemUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var problem = _problemsList.Select(p => p).Where(p => p.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (problem == null)
                return;

            var probToRemove = StaticHolder.SelectedProblems[SelectedPatient.PatientId].Select(p => p).Where(p => p.Id == problem.Id).FirstOrDefault();

            if (probToRemove == null)
                return;

            probToRemove.IsChecked = false;
            StaticHolder.SelectedProblems[SelectedPatient.PatientId].Remove(probToRemove);

            OnProblemListUpdated.Invoke();
        }
        #endregion

        #region Resource Check / Uncheck
        private void OnResourceChecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var resource = _resourcesList.Select(r => r).Where(r => r.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (resource == null)
                return;

            var exists = StaticHolder.SelectedResources[SelectedPatient.PatientId].Select(r => r).Where(r => r.Id == resource.Id).ToList();

            if (exists.Count <= 0)
            {
                resource.IsChecked = true;
                resource.Position = StaticHolder.SelectedResources[SelectedPatient.PatientId].Count;
                StaticHolder.SelectedResources[SelectedPatient.PatientId].Add(resource);
            }

            OnResourceListUpdated.Invoke();
        }
        private void OnResourceUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var resource = _resourcesList.Select(r => r).Where(r => r.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (resource == null)
                return;

            var resToRemove = StaticHolder.SelectedResources[SelectedPatient.PatientId].Select(r => r).Where(r => r.Id == resource.Id).FirstOrDefault();

            if (resToRemove == null)
                return;

            resToRemove.IsChecked = false;
            StaticHolder.SelectedResources[SelectedPatient.PatientId].Remove(resToRemove);
            OnResourceListUpdated.Invoke();
        }
        #endregion

        #region Target Check / Uncheck
        private void OnTargetChecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var target = _targetsList.Select(t => t).Where(t => t.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (target == null)
                return;

            var exists = StaticHolder.SelectedTargets[SelectedPatient.PatientId].Select(t => t).Where(t => t.Id == target.Id).ToList();

            if (exists.Count <= 0)
            {
                target.IsChecked = true;
                target.Position = StaticHolder.SelectedTargets[SelectedPatient.PatientId].Count;
                StaticHolder.SelectedTargets[SelectedPatient.PatientId].Add(target);
            }

            OnTargetListUpdated.Invoke();
        }
        private void OnTargetUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var target = _targetsList.Select(t => t).Where(t => t.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (target == null)
                return;

            var tarToRemove = StaticHolder.SelectedTargets[SelectedPatient.PatientId].Select(t => t).Where(t => t.Id == target.Id).FirstOrDefault();

            if (tarToRemove == null)
                return;

            tarToRemove.IsChecked = false;
            StaticHolder.SelectedTargets[SelectedPatient.PatientId].Remove(tarToRemove);

            OnTargetListUpdated.Invoke();
        }
        #endregion

        #region Measure Check / Uncheck
        private void OnMeasureChecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var measure = _measuresList.Select(m => m).Where(m => m.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (measure == null)
                return;

            var exists = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Select(m => m).Where(m => m.Id == measure.Id).ToList();

            if (exists.Count <= 0)
            {
                measure.IsChecked = true;
                measure.Position = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Count;
                StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Add(measure);
            }

            OnMeasureListUpdated.Invoke();
        }
        private void OnMeasureUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var measure = _measuresList.Select(m => m).Where(m => m.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (measure == null)
                return;

            var measToRemove = StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Select(m => m).Where(m => m.Id == measure.Id).FirstOrDefault();

            if (measToRemove == null)
                return;

            measToRemove.IsChecked = false;
            StaticHolder.SelectedMeasures[SelectedPatient.PatientId].Remove(measToRemove);

            OnMeasureListUpdated.Invoke();
        }
        #endregion
    }
}
