using PflegedientPlan.Classes;
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
    /// Interaction logic for AddToMask.xaml
    /// </summary>
    public partial class AddToMask : Window
    {
        public delegate void NewProblemAddedToMain(ObservableCollection<Problem> selectedList);
        public event NewProblemAddedToMain OnProblemListUpdated;

        private Patient SelectedPatient { get; set; }
        private Activity SelectedActivity { get; set; }
        private Category SelectedCategory { get; set; }

        private readonly ObservableCollection<Problem> _problemsList = new ObservableCollection<Problem>();
        private readonly ObservableCollection<Problem> _selectedProblems = new ObservableCollection<Problem>();

        public AddToMask(Patient patient, Activity activity, Category category, ObservableCollection<Problem> selectedProblems)
        {
            SelectedPatient = patient;
            SelectedActivity = activity;
            SelectedCategory = category;

            _selectedProblems.Clear();
            _selectedProblems = selectedProblems;

            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            await LoadProblemsAsync();

            if (_selectedProblems.Count > 0)
            {
                // set checkboxes
                foreach (var problem in _selectedProblems)
                {
                    (problemsListBox.Items.GetItemAt(problem.Id) as CheckBox).IsChecked = true;
                }
            }
        }

        #region Load problems async
        private async Task LoadProblemsAsync()
        {
            _problemsList.Clear();

            using (var client = new DatabaseClient())
            {
                if (await client.OpenConnectionAsync())
                {
                    using (var reader = await client.SelectAsync("SELECT * FROM problems;"))
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                var problem = new Problem()
                                {
                                    Id = reader.GetInt32(0),
                                    Position = reader.GetInt32(1),
                                    Description = reader.GetString(2)
                                };

                                problem.Description = ReplacePlaceholder(problem.Description);

                                _problemsList.Add(problem);
                            }
                        }
                    }
                }
            }

            problemsListBox.ItemsSource = _problemsList;
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
                        client.ExecuteAsync("INSERT INTO problems (position, description) VALUES (@pos, @desc);");
                        client.ClearParameter();
                    }
                }
            }

            await LoadProblemsAsync();
        }
        #endregion

        #region Replace placeholder in string
        private string ReplacePlaceholder(string description)
        {
            var temp = description;
            string newDescription;

            newDescription = temp.Replace("%name%", SelectedPatient.PatientVorname + " " + SelectedPatient.PatientNachname)
                .Replace("%vorname%", SelectedPatient.PatientVorname)
                .Replace("%nachname%", SelectedPatient.PatientNachname)
                .Replace("%anrede%", (SelectedPatient.PatientGender == Gender.MALE) ? "Herr" : "Frau");

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

            problem.Position = _selectedProblems.Count;

            _selectedProblems.Add(problem);
            OnProblemListUpdated.Invoke(_selectedProblems);
        }

        private void OnProblemUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBoxObj = (sender as CheckBox);

            if (checkBoxObj == null)
                return;

            var problem = _problemsList.Select(p => p).Where(p => p.Description == checkBoxObj.Content.ToString()).FirstOrDefault();

            if (problem == null)
                return;

            _selectedProblems.Remove(problem);
            OnProblemListUpdated.Invoke(_selectedProblems);
        }
        #endregion
    }
}
