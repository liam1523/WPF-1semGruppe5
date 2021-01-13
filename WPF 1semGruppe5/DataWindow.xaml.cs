using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace WPF_1semGruppe5
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {

        private struct SmitteTal
        {
            public string Dato { set; get; }
            public string Cases { set; get; }
        }

        private class Branche
        {
            public int BrancheID { get; set; }
            public string Branchekode { get; set; }
            public int Niveau { get; set; }
            public string Titel { get; set; }
            public int Tilstand { get; set; }
        }

        private List<Branche> vs = new List<Branche>();

        private string kNavn;

        private double resultpro = 0;

        public DataWindow(string kommuneNavn)
        {
            InitializeComponent();
            kNavn = kommuneNavn;
            komNavnTxt.Content = kommuneNavn;
            dataGridBranche.ItemsSource = vs;
            GetIncidenstal();
            GetSmittePrDag();
            GetUdviklingenAfSmitte();
            GetLukketBrancher();
            GetRestriktionBrancher();
            GetBrancher();
            if (resultpro >= 20 && resultpro <= 30)
            {
                advarsel.Content = "Anbefales påførsel af restriktioner";
            }
            else if (resultpro > 30 && resultpro <= 45)
            {
                advarsel.Content = "Anbefales restriktioner eller lukning";
            }
            else
            {
                advarsel.Content = "Anbefales der fortages lukning";
            }

        }

        private void GetIncidenstal()
        {
            string connectionString;
            SqlConnection cnn;
            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT IncidensTal FROM Kommuner WHERE KommuneNavn = @knavn", cnn);
                cmd.Parameters.AddWithValue("@knavn", kNavn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    int tal = Convert.ToInt32(sqlReader["IncidensTal"]);
                    inciBox.Text += tal;
                }

                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }

        }

        private void GetSmittePrDag()
        {
            string connectionString;
            SqlConnection cnn;

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                kNavn = kNavn.Replace(" ", "");
                kNavn = kNavn.Replace("-", "");
                string command = string.Format("SELECT Dato, {0} FROM SmitteTal", kNavn);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                List<SmitteTal> smittes = new List<SmitteTal>();

                while (sqlReader.Read())
                {
                    smittes.Add(new SmitteTal { Dato = sqlReader[0].ToString(), Cases = sqlReader[1].ToString() }); ;

                }

                dataGrid.ItemsSource = smittes;
                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }

        }

        private void GetUdviklingenAfSmitte()
        {
            string connectionString;
            SqlConnection cnn;
            int result = 0;
            double result2 = 0;
            int count = 0;
            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("SELECT {0} FROM SmitteTal WHERE Dato not in (SELECT TOP((SELECT count(*) FROM SmitteTal) - 30) Dato from Smittetal)", kNavn);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    if (count < 15)
                    {
                        int tal = Convert.ToInt32(sqlReader[kNavn]);
                        result += tal;
                        count++;
                    }
                    else if (count >= 15)
                    {
                        int tal = Convert.ToInt32(sqlReader[kNavn]);
                        result2 += tal;
                        count++;
                    }
                    resultpro = (result - result2) / result2 * 100;
                }

                if (resultpro < 0)
                {
                    smitteBox.Text += "faldet med " + Math.Round(resultpro) + "% over de sidste 15 dage";
                }
                else if (resultpro > 0)
                {
                    smitteBox.Text += "steget med " + Math.Round(resultpro) + "% over de sidste 15 dage";
                }

                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
        }

        private void GetBrancher()
        {
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("SELECT * FROM Branche WHERE NOT EXISTS " +
                    "(SELECT * FROM Lukning WHERE Lukning.BrancheID = Branche.BrancheID and Lukning.KommuneID = {0}) and " +
                    "NOT EXISTS (SELECT * FROM Restriktion WHERE Restriktion.BrancheID = Branche.BrancheID and Restriktion.KommuneID = {0})", kommuneTal);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    vs.Add(new Branche { BrancheID = Convert.ToInt32(sqlReader[0]), Branchekode = sqlReader[1].ToString(), Niveau = Convert.ToInt32(sqlReader[2]), Titel = sqlReader[3].ToString(), Tilstand = 0 });

                }

                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
        }

        private void GetLukketBrancher()
        {
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("SELECT * FROM Branche WHERE BrancheID IN (SELECT BrancheID FROM Lukning WHERE KommuneID = {0} GROUP BY BrancheID)", kommuneTal);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    vs.Add(new Branche { BrancheID = Convert.ToInt32(sqlReader[0]), Branchekode = sqlReader[1].ToString(), Niveau = Convert.ToInt32(sqlReader[2]), Titel = sqlReader[3].ToString(), Tilstand = 1 });

                }

                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
        }

        private void GetRestriktionBrancher()
        {
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("SELECT * FROM Branche WHERE BrancheID IN (SELECT BrancheID FROM Restriktion WHERE KommuneID = {0} GROUP BY BrancheID)", kommuneTal);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    vs.Add(new Branche { BrancheID = Convert.ToInt32(sqlReader[0]), Branchekode = sqlReader[1].ToString(), Niveau = Convert.ToInt32(sqlReader[2]), Titel = sqlReader[3].ToString(), Tilstand = 2 });

                }

                sqlReader.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
        }

        private int GetKommuneID()
        {
            int kommuneTal = 0;
            string connectionString;
            SqlConnection cnn;

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("SELECT KommuneID FROM Kommuner WHERE KommuneNavn = '{0}'", kNavn);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                SqlDataReader sqlReader = cmd.ExecuteReader();
                while (sqlReader.Read())
                {
                    kommuneTal = Convert.ToInt32(sqlReader[0]);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
            return kommuneTal;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (true)
            {
                MessageBoxResult result = MessageBox.Show("Er du sikker på at du vil lukke programmet?", "Data program", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void LukningButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow dataGridRow = dataGridBranche.ItemContainerGenerator.ContainerFromIndex(dataGridBranche.SelectedIndex) as DataGridRow;
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();
            int branchetal = 0;

            Branche branche = new Branche();
            foreach (var obj in dataGridBranche.SelectedItems)
            {
                branche = obj as Branche;
                branchetal = branche.BrancheID;
            }

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            if (dataGridRow.Background == Brushes.LightGreen)
            {
                try
                {
                    string command = string.Format("INSERT INTO Lukning (BrancheID, LDatoTid, KommuneID) VALUES ({0}, GETDATE(), {1})", branchetal, kommuneTal);
                    SqlCommand cmd = new SqlCommand(command, cnn);
                    cnn.Open();

                    cmd.ExecuteNonQuery();

                    if (dataGridRow != null)
                    {
                        dataGridRow.Background = Brushes.LightPink;
                    }
                    dataGridRow.IsSelected = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
                }
            }
        }

        private void AabningButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow dataGridRow = dataGridBranche.ItemContainerGenerator.ContainerFromIndex(dataGridBranche.SelectedIndex) as DataGridRow;
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();
            int branchetal = 0;

            Branche branche = new Branche();
            foreach (var obj in dataGridBranche.SelectedItems)
            {
                branche = obj as Branche;
                branchetal = branche.BrancheID;
            }

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                string command = string.Format("DELETE FROM Restriktion WHERE BrancheID = {0} and KommuneID = {1}; DELETE FROM Lukning WHERE BrancheID = {0} and KommuneID = {1}", branchetal, kommuneTal);
                SqlCommand cmd = new SqlCommand(command, cnn);
                cnn.Open();

                cmd.ExecuteNonQuery();

                if (dataGridRow != null)
                {
                    dataGridRow.Background = Brushes.LightGreen;
                }
                dataGridRow.IsSelected = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }

        }

        private void RestriktionButton_Click(object sender, RoutedEventArgs e)
        {
            DataGridRow dataGridRow = dataGridBranche.ItemContainerGenerator.ContainerFromIndex(dataGridBranche.SelectedIndex) as DataGridRow;
            string connectionString;
            SqlConnection cnn;

            int kommuneTal = GetKommuneID();
            int branchetal = 0;

            Branche branche = new Branche();
            foreach (var obj in dataGridBranche.SelectedItems)
            {
                branche = obj as Branche;
                branchetal = branche.BrancheID;
            }
            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                if (dataGridRow.Background == Brushes.LightGreen)
                {
                    string command = string.Format("INSERT INTO Restriktion (BrancheID, RDatoTid, KommuneID) VALUES ({0}, GETDATE(), {1})", branchetal, kommuneTal);
                    SqlCommand cmd = new SqlCommand(command, cnn);
                    cnn.Open();

                    cmd.ExecuteNonQuery();

                    if (dataGridRow != null)
                    {
                        dataGridRow.Background = Brushes.Yellow;
                    }
                    dataGridRow.IsSelected = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open) cnn.Close();
            }
        }
    }
}