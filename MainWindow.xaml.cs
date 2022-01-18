using System;
using System.Windows;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Input;


namespace CurrencyConverter_Database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Create an object for SqlConnection
        SqlConnection con = new SqlConnection();

        //Create an object for SqlCommand
        SqlCommand cmd = new SqlCommand();

        //Create an object for SqlDataAdapter
        SqlDataAdapter da = new SqlDataAdapter();

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }

        public void mycon()
        {
            //Database connection string
            string Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open(); //Connection Open
        }

        //method for binding the currency name and currency value using From currency and To currency ComboBox
        private void BindCurrency()
        {
            mycon();

            //create an object for the DataTable
            DataTable dt = new DataTable();

            //Write query for get data from Currency_Master table
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);

            //CommandType defines which type of command we use for writing a query
            cmd.CommandType = CommandType.Text;

            //It accepts a parameter that contains the text of the object's selectCommand property
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //Create an object for DataRow
            DataRow newRow = dt.NewRow();

            //Assign a value to the Id column
            newRow["Id"] = 0;

            //Assign value to the CurrencyName column
            newRow["CurrencyName"] = "--SELECT--";

            //Insert a new row in the dt with the data at a 0 position
            dt.Rows.InsertAt(newRow, 0);

            //Check that the dt is not null and rows count is greater than 0
            if(dt != null && dt.Rows.Count > 0)
            {
                //Assign the datatable data to from currency combobox using ItemSource property
                cmbFromCurrency.ItemsSource = dt.DefaultView;

                //Assign the datatable data to the currency combobox using ItemSource property
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close(); //close connection

            //To display the underlying datasource for cmbFromCurrency
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";

            //To use as the actual value for the items
            cmbFromCurrency.SelectedValuePath = "Id";

            //Shows the default item in combobox
            cmbFromCurrency.SelectedValue = 0;

            //Similar to as above with cmbFromCurrency
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;
        }

        //This method is used to clear all the controls input which user entered
        private void ClearControls()
        {
            try
            {
                //Clear amount in textbox text
                txtCurrency.Text = string.Empty;

                //From currency combobox items count greater than 0
                //Need to test for these so the program doesn't crash initially
                if(cmbFromCurrency.Items.Count > 0)
                {
                    //set from currency combobox selected item hint
                    cmbFromCurrency.SelectedIndex = 0;
                }

                //similar as above
                //To currency combobox items count greater than 0
                if(cmbToCurrency.Items.Count > 0)
                {
                    //set the to currency comobox selected item hint
                    cmbToCurrency.SelectedIndex = 0;
                }

                //Clear label text
                lblCurrency.Content = "";

                //Set focus on amount textbox
                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Allow only integers in the TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regular Expression to add regex.  Add library using System.Text.RegularExpressions;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Method is used to clear all input which the user has entered in the currency master tab
        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the validation
                if(txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if(txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency type", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (MessageBox.Show("Are you sure you want to Save?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                mycon(); //open the connection
                DataTable dt = new DataTable();
                cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                cmd.Parameters.AddWithValue("@CurencyName", txtCurrencyName.Text);
                cmd.ExecuteNonQuery();
                con.Close(); //close the connection
            }
        }

        //Bind Data in DataGrid View
        public void GetData()
        {
            //The method is used for connect with database and open the data based connection
            mycon();

            //Create Datatable object
            DataTable dt = new DataTable();

            //Write SQL query for GetData from database table.
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);

            //CommandType define which type of command to execute (Text, StoredProcedure, TableDirect, ...)
            cmd.CommandType = CommandType.Text;

            //The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data
            //The Fill operation then adds the rows to the destination DataTable objects in the DataSet
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //checks dt is not null and row count greater than 0
            if(dt != null && dt.Rows.Count > 0)
            {
                //Assign DataTable data to dgvCurrency using ItemSource property
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            //Database connection close
            con.Close();
        }


        


    }
}
