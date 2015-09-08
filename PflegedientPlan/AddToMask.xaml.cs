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
        public delegate void NewProblemAddedToMain();
        public event NewProblemAddedToMain OnProblemListUpdated;

        private Patient SelectedPatient { get; set; }
        private Activity SelectedActivity { get; set; }
        private Category SelectedCategory { get; set; }

        private readonly ObservableCollection<Problem> _problemsList = new ObservableCollection<Problem>();

        public AddToMask(Patient patient, Activity activity, Category category)
        {
            SelectedPatient = patient;
            SelectedActivity = activity;
            SelectedCategory = category;

            InitializeComponent();
            Init();
        }

        private async void Init()
        {
            await LoadProblemsAsync();
            LoadSelectedProblems();
        }

        private async void LoadSelectedProblems()
        {
                if (StaticHolder.SelectedProblems.Count > 0)
                {
                    foreach (var problem in StaticHolder.SelectedProblems)
                    {
                        var item = (_problemsList.Select(p => p).Where(p => p.Id == problem.Id).FirstOrDefault());

                        if (item != null)
                        {
                            item.IsChecked = true;
                        }
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

                newProblemTextBox.Text = "";

                await LoadProblemsAsync();
                LoadSelectedProblems();
            }
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

            var exists = StaticHolder.SelectedProblems.Select(p => p).Where(p => p.Id == problem.Id).ToList();

            if (exists.Count <= 0)
            {
                problem.IsChecked = true;
                problem.Position = StaticHolder.SelectedProblems.Count;
                StaticHolder.SelectedProblems.Add(problem);
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

            var probToRemove = StaticHolder.SelectedProblems.Select(p => p).Where(p => p.Id == problem.Id).FirstOrDefault();

            if (probToRemove == null)
                return;

            probToRemove.IsChecked = false;
            StaticHolder.SelectedProblems.Remove(probToRemove);

            OnProblemListUpdated.Invoke();
        }
        #endregion
    }
}
