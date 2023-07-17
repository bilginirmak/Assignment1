using System;
using System.Collections.Generic;
using System.Data;
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
using Npgsql;
using NpgsqlTypes;

namespace Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        /*
         * Database Connection & Operation
         * 
         * The working steps of the Database Connection are:
         * 
         * 1. Database connection with the Connection String (Need to Form)
         * 2. Establish the DB Connection
         * -- Open the Connection
         * 3. Generate the SQL Command for performing SQL Operations
         * 4. Execute the Command
         * 5. Close the Connection to Save the Results
         * 
         */

        //Step> 1. Database Connection Creation

        private static string getConnectionString()
        {
            // in this method, we are going to create a connection string for the PostGreSQL server..
            string host = "Host=localhost;";
            string port = "Port=5432;";
            string dbName = "Database=Assignment1;";
            string userName = "Username=postgres;";
            string password = "Password=;";
            // Now we need to add all these values/strings to one string, means we are going to merge
            // the strings together and going to create our string
            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }

        //Step> 2. Establishing DB Connection
        /*
         * to perform any database connection, we need an adapter. For the adapter, we can consider
         * it as a library and that library we need to add in our program. 
         * 
         * for any library addition/installation, we need to search for the library in NuGet package
         * manager.
         */

        // connection adapter
        public static NpgsqlConnection con;

        // sql command adapter
        public static NpgsqlCommand cmd;
        private static void establishConnection()
        {
            // in establishing database connection, you need to use exception handling, because it helps
            // to detect if somehow the connection fails or your database server is not going to be 
            // connected

            try
            {
                // create the connection
                con = new NpgsqlConnection(getConnectionString());

                /*
                 * we actually need to pass the connectionString inside the NpgsqlConnection adapter
                 * class constructor. To do so, we are calling the getConnectionString() method as 
                 * this method returns us the Database connection String we have created for 
                 * our work. 
                 */

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            // establish connection
            establishConnection();
            try
            {
                // open the connection
                con.Open();

                // create the SQL query
                string Query = "INSERT INTO admin VALUES (default, @name, @amount, @price)";

                // create sql Command 
                NpgsqlCommand cmd = new NpgsqlCommand(Query, con);

                int amount;
                if (!int.TryParse(textboxAmount.Text, out amount))
                {
                    // Invalid integer format or empty input
                    // Handle the error condition (display an error message, handle default value, etc.)
                }

                decimal price;
                if (!decimal.TryParse(textboxPrice.Text, out price))
                {
                    // Invalid decimal format or empty input
                    // Handle the error condition (display an error message, handle default value, etc.)
                }

                // Add the values for the parameters in the SQL query
                cmd.Parameters.AddWithValue("@name", textboxName.Text);
                cmd.Parameters.AddWithValue("@amount", NpgsqlTypes.NpgsqlDbType.Integer, amount);
                cmd.Parameters.AddWithValue("@price", NpgsqlTypes.NpgsqlDbType.Numeric, price); // Specify the data type as NpgsqlDbType.Numeric

                // execute the Query
                cmd.ExecuteNonQuery();

                MessageBox.Show("Book Item Successfully");

                // close the Connection
                con.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            // establish connection
            establishConnection();
            try
            {
                con.Open();
                string Query = "select * from admin";
                cmd = new NpgsqlCommand(Query, con);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataView.ItemsSource = dt.AsDataView();
                DataContext = da;
                con.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            establishConnection();
            try
            {
                con.Open();
                string Query = "UPDATE admin SET name = @name, amount = @amount, price = @price WHERE id = @id";
                cmd = new NpgsqlCommand(Query, con);

                int amount;
                if (!int.TryParse(textboxAmount.Text, out amount))
                {
                    // Invalid integer format or empty input
                    // Handle the error condition (display an error message, handle default value, etc.)
                }

                decimal price;
                if (!decimal.TryParse(textboxPrice.Text, out price))
                {
                    // Invalid decimal format or empty input
                    // Handle the error condition (display an error message, handle default value, etc.)
                }

                cmd.Parameters.AddWithValue("@id", textboxId.Text);
                cmd.Parameters.AddWithValue("@name", textboxName.Text);
                cmd.Parameters.AddWithValue("@amount", NpgsqlTypes.NpgsqlDbType.Integer, amount); // Specify the data type as NpgsqlDbType.Integer
                cmd.Parameters.AddWithValue("@price", NpgsqlTypes.NpgsqlDbType.Numeric, price);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Item Updated Successfully");
                con.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            establishConnection();
            try
            {
                con.Open();
                string Query = "DELETE FROM admin WHERE id = @id";
                cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@id", textboxId.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Item Deleted Successfully");
                con.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            establishConnection();
            try
            {
                con.Open();
                string Query = "SELECT * FROM admin WHERE id = @id";
                cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@id", textboxId.Text);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    textboxName.Text = dr["name"].ToString();
                    textboxAmount.Text = dr["amount"].ToString();
                    textboxPrice.Text = dr["price"].ToString();
                }
                con.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            establishConnection();
            try
            {
                con.Open();
                string Query = "SELECT * FROM admin WHERE name = @name";
                cmd = new NpgsqlCommand(Query, con);
                cmd.Parameters.AddWithValue("@name", textboxSearch.Text);
                NpgsqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    textboxId.Text = dr["id"].ToString();
                    textboxAmount.Text = dr["amount"].ToString();
                    textboxPrice.Text = dr["price"].ToString();
                }
                con.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);    
            }
        }
    }
    }

