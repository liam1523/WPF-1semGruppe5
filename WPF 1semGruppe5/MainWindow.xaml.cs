using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_1semGruppe5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            string connectionString;
            SqlConnection cnn;

            connectionString = "Data Source = .;Initial Catalog = Projekt1semGruppe5; Integrated Security = True";
            cnn = new SqlConnection(connectionString);
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT KommuneNavn FROM Kommuner", cnn);
                cnn.Open();
                SqlDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    cb.Items.Add(sqlReader["KommuneNavn"].ToString());
                }
                sqlReader.Close();
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

        private void Cb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            cb.IsDropDownOpen = true;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string password = "";
            password = cb.Text + "1234";

            if (pwBox.Password.ToString() == cb.Text + "1234")
            {
                this.Hide();
                DataWindow dataWindow = new DataWindow(cb.Text);
                dataWindow.ShowDialog();
                this.Close();
            }
            else if (cb.Text == "")
            {
                MessageBox.Show("Vælg din kommune", "Prøv igen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                pwBox.Password = string.Empty;
                MessageBox.Show("Forkert kode", "Prøv igen", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }

}
