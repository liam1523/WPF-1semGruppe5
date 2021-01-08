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
        MainWindow mWindow = new MainWindow();

        private string kNavn;

        public DataWindow(string kommuneNavn)
        {
            InitializeComponent();
            kNavn = kommuneNavn;
        }

        private void Window_Activated(object sender, EventArgs e)
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
    }
}
