using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;// for File
//using System.Linq;// for Query Language
using FileHelpers;// CSVReader
using System.Data.SqlClient;// SQL Connection

namespace PACS_Analyzer
{
    public partial class MainForm : Form
    {
        /*
         * Global Variables
         * @FCSO - class for storing information between Core and Form
         */
        FormCoreShare FCSO = new FormCoreShare();

        public MainForm()
        {
            InitializeComponent();

            FCSO.filePath = "default.csv";
            FCSO.connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Navalny\Documents\нир\App\PACS Analyzer\PACS Analyzer\DatabaseMain.mdf;Integrated Security=True;MultipleActiveResultSets=True";
            // replace "Choose" with the default path
            linkLabelChoose.Text = FCSO.filePath;

            backgroundWorkerFile.WorkerReportsProgress = true;
            backgroundWorkerTable.WorkerReportsProgress = true;

        }

        /*
         * Open file dialog and choose a File
         */
        private void linkLabelChoose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*openFileDialogChooseFile.Filter = "CSV files|*.csv";
            openFileDialogChooseFile.Title = "Select a Log file";*/
            DialogResult result = openFileDialogChooseFile.ShowDialog(); // Show the dialog.
        }

        private void openFileDialogChooseFile_FileOk(object sender, CancelEventArgs e)
        {
            try
            {   //Success way: fill in FCSO (file name, path, size and lines)
                FCSO.filePath = openFileDialogChooseFile.FileName;
                FCSO.fileName = openFileDialogChooseFile.SafeFileName;
                linkLabelChoose.Text = FCSO.fileName;// replace "Choose" with the default path

                progressBarMain.Visible = true;
                backgroundWorkerFile.RunWorkerAsync(FCSO);// Start a backgroundWorkerFile and send an object
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /*
         * Work with file
         */
        private void backgroundWorkerFile_DoWork(object sender, DoWorkEventArgs e)
        {
            /*************************************************************************************
             ********          parse file and fill FormCoreShare object - START           ********
             *************************************************************************************/

            FormCoreShare FCSOBG = e.Argument as FormCoreShare;// it goes from backgroundWorkerFile.RunWorkerAsync(FCSO); object is sent from MainForm
            var engine = new FileHelperEngine<CSVReader>();
            var result = engine.ReadFile(FCSOBG.filePath);// read the file
            FCSOBG.fileLinesNumber = result.Length;
            FCSOBG.comboBoxTimes = new List<DateTime>();
            FCSOBG.userList = new Dictionary<User, int>();

            for (int i = 1; i < FCSOBG.fileLinesNumber - 2; i = i + Convert.ToInt32(FCSOBG.fileLinesNumber / 20))
            {
                FCSOBG.comboBoxTimes.Add(result[i].timestamp);
                backgroundWorkerFile.ReportProgress((i * 100) / FCSOBG.fileLinesNumber); // progressBar

            }
            FCSOBG.comboBoxTimes.Add(result[FCSOBG.fileLinesNumber - 2].timestamp);// add dates to MainForm

            var uniqueUsers = result.GroupBy(p => p.empid)// find unique Users
                            .Select(g => g.First())
                            .ToList();

            int iForUser = 1;
            foreach (var line in uniqueUsers)
            {// make a new User obj and add to FormCoreShare obj
                User temp = new User(iForUser, line.empid, line.a, line.firstname, line.lastname, line.department);
                FCSOBG.userList.Add(temp, iForUser);
                iForUser++;
            }

            e.Result = FCSOBG;// send info to backgroundWorkerFile_RunWorkerCompleted

            /*************************************************************************************
             ********          parse file and fill FormCoreShare object - FINISH          ********
             *************************************************************************************/
        }

        private void backgroundWorkerFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarMain.Value = e.ProgressPercentage;// Change the value of the ProgressBar to the BackgroundWorker progress
        }

        private void backgroundWorkerFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FCSO = e.Result as FormCoreShare;// it goes from backgroundWorkerFile

            /*
             * DataSources for DropDown Lists
             */
            BindingSource forComboBoxFrom = new BindingSource();// From
            forComboBoxFrom.DataSource = FCSO.comboBoxTimes;
            comboBoxFrom.DataSource = forComboBoxFrom;
            comboBoxFrom.Enabled = true;
            BindingSource forComboBoxTill = new BindingSource();// Till
            forComboBoxTill.DataSource = FCSO.comboBoxTimes;
            comboBoxTill.DataSource = forComboBoxTill;
            comboBoxTill.Enabled = true;
            buttonFind.Enabled = true;

            progressBarMain.Visible = false;
            progressBarMain.Value = 0;
        }

        private void backgroundWorkerTable_DoWork(object sender, DoWorkEventArgs e)
        {
            /*************************************************************************************
            ********                     fill table INTERVALS    - START                 ********
            *************************************************************************************/

            FormCoreShare FCSOBG = e.Argument as FormCoreShare;// it goes from backgroundWorkerTable.RunWorkerAsync(FCSO); object is sent from MainForm
            using (SqlConnection sqlConnection = new SqlConnection(FCSOBG.connectionString))
            {
                sqlConnection.Open();// open connection

                SqlCommand emptyUsers = new SqlCommand("TRUNCATE TABLE [intervals];", sqlConnection);//Empty table intervals
                try
                {
                    emptyUsers.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }// if cannot delete table. Most likely table doesn't exist

                sqlConnection.Close();// close connection
            }// end of connection


            var engine = new FileHelperEngine<CSVReader>();
            var result = engine.ReadFile(FCSOBG.filePath);// read the file
            FCSOBG.fileLinesNumber = result.Length;

            // List of lines with User(client) filter
            var timeGapList = from line in result
                              where line.timestamp >= FCSOBG.timeFrom && line.timestamp <= FCSOBG.timeTill
                              select line;

            TimeSpan interval = new TimeSpan();
            int intervalMinutes = -1;
            int iP = 1;// counter for Progress Change and ID
            int timeGapListCount = timeGapList.Count();// length of timeGapList

            using (SqlConnection sqlConnection = new SqlConnection(FCSOBG.connectionString))
            {
                sqlConnection.Open();// open connection
                foreach (var line in timeGapList)
                {
                    var nextTimeLine = (from nextTime in timeGapList
                                        where nextTime.empid == line.empid && nextTime.timestamp > line.timestamp
                                        select nextTime).FirstOrDefault();// next timeStamp for this user. FINISH time
                    try
                    {
                        interval = nextTimeLine.timestamp - line.timestamp;
                        intervalMinutes = interval.Minutes;// time difference (duration) in minutes

                        // INSERT INTO [intervals]
                        SqlCommand insertIntervals = new SqlCommand("INSERT INTO [intervals] (start_time, end_time, user_id, floor, zone, duration) VALUES (@start_time, @end_time, @user_id, @floor, @zone, @duration)", sqlConnection);
                        try
                        {
                            insertIntervals.Parameters.AddWithValue("start_time", line.timestamp);
                            insertIntervals.Parameters.AddWithValue("end_time", nextTimeLine.timestamp);
                            insertIntervals.Parameters.AddWithValue("user_id", FCSOBG.userList.First(g => g.Key.empID == line.empid).Key.id);// find UserId from FormCoreShare obj
                            insertIntervals.Parameters.AddWithValue("duration", intervalMinutes);
                            insertIntervals.Parameters.AddWithValue("floor", line.floor);
                            insertIntervals.Parameters.AddWithValue("zone", line.zone);
                            insertIntervals.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                    catch// there is no FINISH time
                    {
                        FCSOBG.errorText = "no FINISH time";
                        continue;
                    }// end of try/catch
                    backgroundWorkerTable.ReportProgress((iP * 100) / timeGapListCount);
                    iP++;
                }// end of foreach
                sqlConnection.Close();// close connection
            }// end of connection



            /*************************************************************************************
            ********          create and fill table INTERVALS    - FINISH                ********
            *************************************************************************************/


            /*************************************************************************************
            ********          create and fill table GRAPHBYDAY    - START                ********
            *************************************************************************************/
            using (SqlConnection sqlConnection = new SqlConnection(FCSOBG.connectionString))
            {
                sqlConnection.Open();// open connection


                using (SqlCommand emptyGraphByDate = new SqlCommand("TRUNCATE TABLE [GraphByDate];", sqlConnection))//Empty table intervals
                {
                    try
                    {
                        emptyGraphByDate.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }// if cannot delete table. Most likely table doesn't exist
                }


                using (SqlCommand readIntervalsParent = new SqlCommand("SELECT * FROM [intervals]", sqlConnection))// First, read all table [intervals]
                {
                    SqlDataAdapter da = new SqlDataAdapter(readIntervalsParent);
                    DataTable dt = new DataTable();
                    da.Fill(dt);// Put all data to DataTable formay
                    iP = 1;// counter for Progress Change and ID
                    foreach (DataRow lineParent in dt.Rows)// read [intervals] line by line
                    {
                        using (SqlCommand readIntervalsChild = new SqlCommand("SELECT * FROM [intervals] WHERE [start_time] > @our_start_time AND [start_time] < @our_end_time AND [floor] = @floor AND [zone] = @zone", sqlConnection))// get all lines from [intervals] where [start_time] is between ↑start_time and ↑end_time AND floor = ↑floor, zone = ↑zone
                        {// get all users who also have been there
                            readIntervalsChild.Parameters.AddWithValue("our_start_time", lineParent["start_time"]);
                            readIntervalsChild.Parameters.AddWithValue("our_end_time", lineParent["end_time"]);
                            readIntervalsChild.Parameters.AddWithValue("floor", lineParent["floor"]);
                            readIntervalsChild.Parameters.AddWithValue("zone", lineParent["zone"]);
                            SqlDataAdapter daChild = new SqlDataAdapter(readIntervalsChild);
                            DataTable dtChild = new DataTable();
                            daChild.Fill(dtChild);// Put all data to DataTable formay
                            foreach (DataRow lineChild in dtChild.Rows)// read [intervals]Child line by line
                            {// for each user who has been there
                                using (SqlCommand selectGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE ([user_source_id] = @user_source_id AND [user_target_id] = @user_target_id) OR ([user_source_id] = @user_target_id AND [user_target_id] = @user_source_id) AND [date] = @date;", sqlConnection))// check if such line exists in [GraphByDate] table
                                {
                                    selectGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// user_id
                                    selectGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id
                                    selectGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);// date should be unique
                                    int lineExist = 0;
                                    try// if there are any rows
                                    {
                                        lineExist = (int)selectGraphByDate.ExecuteScalar();// Get number of rows (check if such line exists in [GraphByDate] table)
                                    }
                                    catch
                                    {
                                        // table is empty
                                    }

                                    if (lineExist > 0)// line already exists, then UPDATE
                                    {
                                        using (SqlCommand updateGraphByDate = new SqlCommand("UPDATE [GraphByDate] SET [duration] = @duration, [times] = @times WHERE ([user_source_id] = @user_source_id AND [user_target_id] = @user_target_id) OR ([user_source_id] = @user_target_id AND [user_target_id] = @user_source_id) AND [date] = @date;", sqlConnection))// update
                                        {
                                            updateGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// user_id
                                            updateGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id
                                            updateGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);// date should be unique

                                            interval = Convert.ToDateTime(lineChild["end_time"]) - Convert.ToDateTime(lineParent["start_time"]);
                                            intervalMinutes = interval.Minutes;// time difference (duration) in minutes

                                            int durationTemp = 0;
                                            int timesTemp = 0;
                                            using (SqlDataReader selectGraphByDateReader = selectGraphByDate.ExecuteReader())
                                            {// read previous values with selectGraphByDate request
                                                if (selectGraphByDateReader.HasRows)
                                                {
                                                    selectGraphByDateReader.Read(); // read first row
                                                    durationTemp = (int)selectGraphByDateReader["duration"];// get previous value of Duration
                                                    timesTemp = (int)selectGraphByDateReader["times"];// get previous value of Times
                                                }// if
                                                selectGraphByDateReader.Close(); // <- too easy to forget
                                                selectGraphByDateReader.Dispose(); // <- too easy to forget
                                            }// end of reader
                                            updateGraphByDate.Parameters.AddWithValue("duration", durationTemp + intervalMinutes);
                                            updateGraphByDate.Parameters.AddWithValue("times", timesTemp + 1);

                                            updateGraphByDate.ExecuteNonQuery();// execute UPDATE
                                        }// UPDATE INTO [GraphByDate]
                                    }// if
                                    else// there is no such lines, then INSERT
                                    {
                                        using (SqlCommand insertGraphByDate = new SqlCommand("INSERT INTO [GraphByDate] (date, user_source_id, user_target_id, duration, times) VALUES (@date, @user_source_id, @user_target_id, @duration, @times);", sqlConnection))// get all lines from [intervals] where [start_time] is between ↑ start_time and ↑ end_time
                                        {
                                            insertGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);
                                            insertGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// user_id
                                            insertGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id

                                            interval = Convert.ToDateTime(lineChild["end_time"]) - Convert.ToDateTime(lineParent["start_time"]);
                                            intervalMinutes = interval.Minutes;// time difference (duration) in minutes

                                            insertGraphByDate.Parameters.AddWithValue("duration", intervalMinutes);
                                            insertGraphByDate.Parameters.AddWithValue("times", 1);

                                            insertGraphByDate.ExecuteNonQuery();
                                        }// INSERT INTO [GraphByDate]
                                    }// else
                                }// SELECT * FROM [GraphByDate]                                
                            }// foreach
                        }// SELECT * FROM [intervals] WHERE [start_time]
                        backgroundWorkerTable.ReportProgress((iP * 100) / dt.Rows.Count);
                        iP++;
                    }// foreach
                }// SELECT * FROM [intervals]


                sqlConnection.Close();// close connection
            }// end of connection

            /*************************************************************************************
            ********          create and fill table GRAPHBYDAY    - FINISH                ********
            *************************************************************************************/

            /*************************************************************************************
            ********          Anomalies search    - START                ********
            *************************************************************************************/
            
            using (SqlConnection sqlConnection = new SqlConnection(FCSOBG.connectionString))
            {
                sqlConnection.Open();// open connection

                using (SqlCommand readGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate]", sqlConnection))// First, read whole table [GraphByDate]
                {
                    SqlDataAdapter da = new SqlDataAdapter(readGraphByDate);
                    DataTable dt = new DataTable();
                    da.Fill(dt);// Put all data to DataTable formay
                    iP = 1;// counter for Progress Change and ID
                    Dictionary<DateTime, int> timesList = new Dictionary<DateTime, int>();

                     var uniqueUsers = result.GroupBy(p => p.empid)// find unique Users
                            .Select(g => g.First())
                            .ToList();
                    //foreach (DataRow line in dt.Rows)// read [GraphByDate] line by line
                    //{
                        /*int sevenDays = 7;// look for the day from the next week
                        var nextWeek = from date in dt.AsEnumerable()
                                               where date.Field<int>("user_source_id") == line.Field<int>("user_source_id") && date.Field<int>("user_target_id") == line.Field<int>("user_target_id") && date.Field<DateTime>("date") == Convert.ToDateTime(line.Field<DateTime>("date")).Date.AddDays(sevenDays)
                                               select date;
                        for (; nextWeek != null; sevenDays++)
                        {
                            timesList.Add(nextWeek.)
                        }
                         */
                    /*Dictionary<string, List<int>> arrayByDuration = new Dictionary<string,List<int>>();// dictionary for array of all pairs
                    foreach(User user in FCSOBG.userList.Keys){

                    while(true){
                        var listOfUsers = from date in dt.AsEnumerable()
                                          where date.Field<int>("user_source_id") == user.id || date.Field<int>("user_target_id") == user.id
                                          select date;
                        foreach()
                    }
                    }*/


                        

                    

                }// SELECT * FROM [GraphByDate]

                sqlConnection.Close();// close connection
            }// end of connection
            
            /*************************************************************************************
            ********          Anomalies search    - FINISH                ********
            *************************************************************************************/



            e.Result = FCSOBG;// send info to backgroundWorkerTable_RunWorkerCompleted
        }// end of backgroundWorkerTable_DoWork

        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(comboBoxTill.SelectedItem.ToString()).Subtract(Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString())) <= TimeSpan.Zero)
            {
                MessageBox.Show("Please make sure that your chosen left date earlier than the right one\nDebug info: Line 208, method: buttonFind_Click", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;// 
            }
            FCSO.timeFrom = Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString());
            FCSO.timeTill = Convert.ToDateTime(comboBoxTill.SelectedItem.ToString());

            buttonFind.Enabled = false;// prevent double launch of backgroundWorkerTable
            progressBarMain.Visible = true;
            labelProgress.Visible = true;
            backgroundWorkerTable.RunWorkerAsync(FCSO);
        }// end of buttonFind_Click

        private void backgroundWorkerTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonFind.Enabled = true;// prevent double launch of backgroundWorkerTable
            progressBarMain.Visible = false;
            progressBarMain.Value = 0;
            labelProgress.Visible = false;
        }// end of backgroundWorkerTable_RunWorkerCompleted

        private void backgroundWorkerTable_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarMain.Value = e.ProgressPercentage;// Change the value of the ProgressBar to the BackgroundWorker progress
            labelProgress.Text = e.ProgressPercentage.ToString() + "%";
        }// end of backgroundWorkerTable_ProgressChanged

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonFind_MouseEnter(object sender, EventArgs e)
        {
            buttonFind.UseVisualStyleBackColor = false;
            buttonFind.BackColor = Color.FromArgb(51, 149, 253);
        }

        private void buttonFind_MouseLeave(object sender, EventArgs e)
        {
            buttonFind.UseVisualStyleBackColor = true;
            buttonFind.BackColor = Color.White;
        }
        
        private void linkLabelGenerateGraphs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }
    }// end of class MainForm : Form
}
