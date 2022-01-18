using System.Windows;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


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

        }

    }
}
