using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace WPF_1semGruppe5
{
    /// <summary>
    /// Interaction logic for DataWindow.xaml
    /// </summary>
    public partial class DataWindow : Window
    {

        public struct MyData
        {
            public string Dato { set; get; }
            public string Cases { set; get; }
        }

        MainWindow mWindow = new MainWindow();

        private string kNavn;

        public DataWindow(string kommuneNavn)
        {
            InitializeComponent();
            kNavn = kommuneNavn;
            komNavnTxt.Content = kommuneNavn;
            GetIncidenstal();

            GetSmittePrDag();
        }

        private void GetIncidenstal()
        {
            string connectionString;
            SqlConnection cnn;

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
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

        private void GetSmittePrDag()
        {
            string connectionString;
            SqlConnection cnn;

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            kNavn = kNavn.Replace(" ", "");
            kNavn = kNavn.Replace("-", "");
            string command = string.Format("SELECT Dato, {0} FROM SmitteTal", kNavn);
            SqlCommand cmd = new SqlCommand(command, cnn);
            cnn.Open();

            SqlDataReader sqlReader = cmd.ExecuteReader();

            List<MyData> vs = new List<MyData>();

            while (sqlReader.Read())
            {
                vs.Add(new MyData { Dato = sqlReader[0].ToString(), Cases = sqlReader[1].ToString() }); ;

            }

            dataGrid.ItemsSource = vs;

            sqlReader.Close();

        }

    }

}