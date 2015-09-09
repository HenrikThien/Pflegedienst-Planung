using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PflegedientPlan
{
    sealed class DatabaseClient : IDisposable
    {
        private static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VIS VITALIS\\Storage.mdf";
        private string ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + AppDataPath + ";Integrated Security=True";

        private readonly SqlConnection _connection;
        private readonly List<SqlParameter> _parameter;

        public DatabaseClient()
        {
            _connection = new SqlConnection(ConnectionString);
            _parameter = new List<SqlParameter>();
        }


        public async Task<bool> OpenConnectionAsync()
        {
            try
            {
                await _connection.OpenAsync();
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Es konnte keine Datenbankverbindung hergestellt werden.\nDie Anwendung funktioniert möglicherweiße nicht richtig.");
                return false;
            }
        }

        private async void WriteException(Exception ex)
        {
            await Logger.WriteException(ex.ToString());
        }

        public async Task<SqlDataReader> SelectAsync(string query)
        {
            using (var command = new SqlCommand(query, _connection))
            {
                foreach (var param in _parameter)
                {
                    if (!command.Parameters.Contains(param))
                        command.Parameters.Add(param);
                }

                return await command.ExecuteReaderAsync();
            }
        }

        public async Task<int> InsertAsync(string query)
        {
            using (var command = new SqlCommand(query, _connection))
            {
                foreach (var param in _parameter)
                {
                    if (!command.Parameters.Contains(param))
                        command.Parameters.Add(param);
                }

                return await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> ExecuteAsync(string query)
        {
            using (var command = new SqlCommand(query, _connection))
            {
                foreach (var param in _parameter)
                {
                    if (!command.Parameters.Contains(param))
                        command.Parameters.Add(param);
                }

                return await command.ExecuteNonQueryAsync();
            }
        }

        public void AddParam<T>(string key, T value)
        {
            var parameter = new SqlParameter(key, value);
            _parameter.Add(parameter);
        }

        public void ClearParameter()
        {
            _parameter.Clear();
        }

        public void Dispose()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
                _connection.Close();

            _connection.Dispose();
            _parameter.Clear();
        }
    }
}
