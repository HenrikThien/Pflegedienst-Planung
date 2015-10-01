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
        // static path to appdata / roaming
        private static string PathToFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VIS VITALIS\\";

        private Patient SelectedPatient { get; set; }
        private PatientUserMask PatientMask { get; set; }

        /// <summary>
        /// Constructor of FileManager
        /// </summary>
        /// <param name="selectedPatient">The selected patient in main window</param>
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

        /// <summary>
        /// Saves a patient async in a json file.
        /// </summary>
        public async Task SavePatient()
        {
            // check if folder exists
            await CheckFolder();
            // check if file exists
            await CheckFile();

            // serialize the object to json string
            var jsonText = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(PatientMask));

            // using streamwriter to write content to .json file
            using (var writer = new StreamWriter(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
            {
                await writer.WriteLineAsync(jsonText);
            }
        }

        /// <summary>
        /// Loads the patient async from existing json file.
        /// </summary>
        public async Task<PatientUserMask> LoadPatient()
        {
            // checks if the folder exists
            var folderExists = await CheckFolder();
            if (!folderExists)
                return null;

            // checks if the file exists
            var fileExists = await CheckFile();
            if (!fileExists)
                return null;

            // using a streamreader to read content from json file
            using (var reader = new StreamReader(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
            {
                // read all text from file async
                var jsonText = await reader.ReadToEndAsync();

                // try to deserialize the object, return null if exception caught
                try
                {
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<PatientUserMask>(jsonText));
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks if a folder exists, if not creates it
        /// </summary>
        private async Task<bool> CheckFolder()
        {
            return await Task<bool>.Run(() =>
            {
                var result = true;

                if (!Directory.Exists(Path.Combine(PathToFolder, "patients")))
                {
                    Directory.CreateDirectory(Path.Combine(PathToFolder, "patients"));
                    result = false;
                }

                return result;
            });
        }

        /// <summary>
        /// Checks if a patient.json file exits, if not creates it.
        /// </summary>
        private async Task<bool> CheckFile()
        {
            return await Task.Run<bool>(() =>
            {
                var result = true;

                if (!File.Exists(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json")))
                {
                    File.Create(Path.Combine(PathToFolder, "patients", "patient-" + SelectedPatient.PatientId + ".json"));
                    result = false;
                }

                return result;
            });
        }

        /// <summary>
        /// Dispose the object, set vars to null
        /// </summary>
        public void Dispose()
        {
            PatientMask = null;
        }
    }
}
