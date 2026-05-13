using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;

namespace excel_to_db
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            try
            {
                string excelFilePath = ExcelFilePathTextBox.Text;
                string connectionString = GetConnectionString(); // Move connection string to a separate method

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Open(excelFilePath);
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) // Use a transaction for better reliability
                    {
                        for (int row = 2; row <= worksheet.UsedRange.Rows.Count; row++)
                        {
                            string col1Value = ((Excel.Range)worksheet.Cells[row, 1]).Text;
                            string col2Value = ((Excel.Range)worksheet.Cells[row, 2]).Text;
                            string col3Value = ((Excel.Range)worksheet.Cells[row, 3]).Text;
                            string col4Value = ((Excel.Range)worksheet.Cells[row, 4]).Text;
                            string col5Value= ((Excel.Range)worksheet.Cells[row, 5]).Text;
                            string col6Value= ((Excel.Range)worksheet.Cells[row, 6]).Text;
                            string col7Value = ((Excel.Range)worksheet.Cells[row, 7]).Text;
                            string col8Value= ((Excel.Range)worksheet.Cells[row, 8]).Text;

                            string insertQuery = "INSERT INTO student_details (Reg_no,Name,Course,Class,emailid,acd_start,acd_end,status) VALUES (@col1, @col2,@col3, @col4,@col5, @col6,@col7,@col8)";
                            using (var command = new MySqlCommand(insertQuery, connection))
                            {
                                command.Parameters.AddWithValue("@col1", col1Value);
                                command.Parameters.AddWithValue("@col2", col2Value);
                                command.Parameters.AddWithValue("@col3", col3Value);
                                command.Parameters.AddWithValue("@col4", col4Value);
                                command.Parameters.AddWithValue("@col5", col5Value);
                                command.Parameters.AddWithValue("@col6", col6Value);
                                command.Parameters.AddWithValue("@col7", col7Value);
                                command.Parameters.AddWithValue("@col8", col8Value);
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit(); // Commit the transaction if all inserts succeed
                    }
                    MessageBox.Show("Data inserted successfully.");
                }

                // Cleanup Excel objects
                workbook.Close(false);
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            DatabaseTextBox.Text =String.Empty;
            ServerTextBox.Text = String.Empty;
            UsernameTextBox.Text = String.Empty;
            PasswordTextBox.Text = String.Empty;
            ExcelFilePathTextBox.Text = String.Empty;
        }

        private string GetConnectionString()
        {
            // Construct and return the MySQL connection string
            return "Server=localhost;Database=sample;User Id=root;Password=9557;";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an Excel File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelFilePathTextBox.Text = openFileDialog.FileName;
            }
        }
    }
}