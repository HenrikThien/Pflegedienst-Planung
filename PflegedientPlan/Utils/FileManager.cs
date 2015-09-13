using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Utils
{
    sealed class FileManager : IDisposable
    {
        private static string PathToFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VIS VITALIS\\";

        private Patient SelectedPatient { get; set; }
        private PatientUserMask PatientMask { get; set; }

        public FileManager(Patient selectedPatient)
        {
            SelectedPatient = selectedPatient;

            PatientMask = new PatientUserMask()
            {
                Problems = StaticHolder.SelectedProblems[selectedPatient.PatientId],
                Resources = StaticHolder.SelectedResources[selectedPatient.PatientId],
                Targets = StaticHolder.SelectedTargets[selectedPatient.PatientId],
                Measures = StaticHolder.SelectedMeasures[selectedPatient.PatientId]
            };
        }

        public async Task SavePatient()
        {
            await CheckFolder();
            await CheckFile();

            var jsonText = await JsonConvert.SerializeObjectAsync(PatientMask);

            using (var writer = new StreamWriter(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
            {
                await writer.WriteLineAsync(jsonText);
            }
        }

        public async Task<PatientUserMask> LoadPatient()
        {
            await CheckFolder();
            await CheckFile();

            using (var reader = new StreamReader(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
            {
                var jsonText = await reader.ReadToEndAsync();

                try
                {
                    return await JsonConvert.DeserializeObjectAsync<PatientUserMask>(jsonText);
                }
                catch
                {
                    return null;
                }
            }
        }

        private async Task CheckFolder()
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(Path.Combine(PathToFolder, "patients")))
                {
                    Directory.CreateDirectory(Path.Combine(PathToFolder, "patients"));
                }
            });
        }

        private async Task CheckFile()
        {
            await Task.Run(() =>
            {
                if (!File.Exists(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
                {
                    File.Create(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json"));
                }
            });
        }

        public void Dispose()
        {
            PatientMask = null;
        }
    }
}
