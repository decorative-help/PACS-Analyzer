using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
        private static ManualResetEvent _waitThreads;
        private static int _numerOfThreadsNotYetCompleted;

        public MainForm()
        {
            InitializeComponent();

            FCSO.filePath = "default.csv";
            FCSO.connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Navalny\Documents\нир\App\PACS Analyzer\PACS Analyzer\DatabaseMain.mdf;Integrated Security=True;MultipleActiveResultSets=True";

            backgroundWorkerFile.WorkerReportsProgress = true;
            backgroundWorkerTable.WorkerReportsProgress = true;
            progressBarSteps.Minimum = 0;
            progressBarSteps.Maximum = 3;// Number of steps on the form
            progressBarSteps.Value = 0;// Set the initial value of the ProgressBar
            progressBarSteps.Step = 1;// Set the Step value
        }
        
        private void openFileDialogChooseFile_FileOk(object sender, CancelEventArgs e)
        {
            try
            {   //Success way: fill in FCSO (file name, path, size and lines)
                FCSO.filePath = openFileDialogChooseFile.FileName;
                FCSO.fileName = openFileDialogChooseFile.SafeFileName;

                progressBarSteps.Visible = true;
                backgroundWorkerFile.RunWorkerAsync(FCSO);// Start a backgroundWorkerFile and send an object
                progressBarSteps.PerformStep();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Console.Out.WriteAsync("\n\n+------------------+\n\nError\nException: " + ex.Message);
            }
        }

        /*
         * Work with file
         */

        private void updateUserList(Object x = null)
        {
            FormCoreShare FCSObj = FCSO;
            var engine = new FileHelperEngine<CSVReader>();
            CSVReader[] result = new CSVReader[0];
            try
            {
                result = engine.ReadFile(FCSObj.filePath);// read the file
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot find the file:\n" + FCSObj.filePath + "\n\n" + e.Message);
                Console.Out.WriteAsync("\n\n+------------------+\n\nCannot find the file\nException: " + e.Message);
                return;
            }

            FCSObj.fileLinesNumber = result.Length;
            FCSObj.userList = new Dictionary<User, int>();

            var uniqueUsers = result.GroupBy(p => p.empid)// find unique Users
                            .Select(g => g.First())
                            .ToList();

            int iForUser = 1;
            foreach (var line in uniqueUsers)
            {// make a new User obj and add to FormCoreShare obj
                User temp = new User(iForUser, line.empid, line.a, line.firstname, line.lastname, line.department);
                FCSObj.userList.Add(temp, iForUser);
                iForUser++;
            }

            FCSO.userList = FCSObj.userList;// associate userList to global one
        }// updateUserList

        private FormCoreShare updateDateGap()
        {
            FormCoreShare FCSObj = FCSO;
            var engine = new FileHelperEngine<CSVReader>();
            var result = engine.ReadFile(FCSObj.filePath);// read the file
            FCSObj.fileLinesNumber = result.Length;
            FCSObj.comboBoxTimes = new List<DateTime>();

            for (int i = 0; i < FCSObj.fileLinesNumber - 2; i = i + Convert.ToInt32(FCSObj.fileLinesNumber / 20))
            {
                FCSObj.comboBoxTimes.Add(result[i].timestamp);
                backgroundWorkerFile.ReportProgress((i * 100) / FCSObj.fileLinesNumber); // progressBar

            }
            FCSObj.comboBoxTimes.Add(result[FCSObj.fileLinesNumber - 2].timestamp);// add dates to MainForm

            return FCSObj;// send info to backgroundWorkerFile_RunWorkerCompleted
        }

        private void backgroundWorkerFile_DoWork(object sender, DoWorkEventArgs e)
        {
            /*************************************************************************************
             ********          openFileDialogChooseFile - START           ********
             *************************************************************************************/

            ThreadPool.QueueUserWorkItem(new WaitCallback(updateUserList));// start gathering user information in a Thread
            e.Result = updateDateGap();// update comboBoxTimes

            /*************************************************************************************
             ********          openFileDialogChooseFile - FINISH          ********
             *************************************************************************************/
        }

        private void backgroundWorkerFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;// Change the value of the ProgressBar to the BackgroundWorker progress
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

            progressBar1.Visible = false;
            progressBar1.Value = 0;
        }

        private void fillInIntervals(object x)
        {
            // parse object x
            object[] array = x as object[];
            CSVReader line = array[0] as CSVReader;
            IOrderedEnumerable<CSVReader> timeGapList = array[1] as IOrderedEnumerable<CSVReader>;
            int iP = (int)array[2];
            int dtRowsCount = (int)array[3];
            backgroundWorkerTable.ReportProgress((iP * 100) / dtRowsCount);

            TimeSpan interval = new TimeSpan();
            int intervalMinutes = -1;
            int timeGapListCount = timeGapList.Count();// length of timeGapList

            using (SqlConnection sqlConnection = new SqlConnection(FCSO.connectionString))
            {
                sqlConnection.Open();// open connection

                var nextTimeLine = (from nextTime in timeGapList
                                    where nextTime.empid == line.empid && nextTime.timestamp > line.timestamp
                                    orderby nextTime.timestamp
                                    select nextTime).FirstOrDefault();// next timeStamp for this user. FINISH time
                if (nextTimeLine == null)// check if there is next time
                {
                    //Console.Out.WriteAsync("\n\n+------------------+\n\nNo next time\nException: ");
                    if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                        _waitThreads.Set();// Signal that work is finished
                    return;
                }
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
                        insertIntervals.Parameters.AddWithValue("user_id", FCSO.userList.First(g => g.Key.empID == line.empid).Key.id);// find UserId from FormCoreShare obj
                        insertIntervals.Parameters.AddWithValue("duration", intervalMinutes);
                        insertIntervals.Parameters.AddWithValue("floor", line.floor);
                        insertIntervals.Parameters.AddWithValue("zone", line.zone);
                        insertIntervals.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        //the end of list for this user
                        Console.Out.WriteAsync("\n\n+------------------+\n\nthe end of list for this user\nException: " + e.Message);
                    }// catch
                }// try
                catch (Exception e)// there is no FINISH time
                {
                    Console.Out.WriteAsync("\n\n+------------------+\n\nNo finish time\nException: " + e.Message);
                }// end of try/catch
                sqlConnection.Close();// close connection
            }// end of connection

            //Console.Out.WriteLineAsync("Thread left: " + _numerOfThreadsNotYetCompleted.ToString());
            if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                _waitThreads.Set();// Signal that work is finished
        }// fillInIntervals

        private void fillInGraph(object x)
        {
            // parse object x
            object[] array = x as object[];
            DataRow lineParent = array[0] as DataRow;
            int iP = (int)array[1];
            int dtRowsCount = (int)array[2];
            backgroundWorkerTable.ReportProgress((iP * 100) / dtRowsCount);

            TimeSpan interval = new TimeSpan();
            int intervalMinutes = -1;

            using (SqlConnection sqlConnection = new SqlConnection(FCSO.connectionString))
            {
                sqlConnection.Open();// open connection

                using (SqlCommand readIntervalsChild = new SqlCommand("SELECT * FROM [intervals] WHERE [start_time] >= @our_start_time AND [start_time] <= @our_end_time AND [floor] = @floor AND [zone] = @zone ORDER BY [start_time]", sqlConnection))// get all lines from [intervals] where [start_time] is between ↑start_time and ↑end_time AND floor = ↑floor, zone = ↑zone
                {// get all users who also have been there
                    readIntervalsChild.Parameters.AddWithValue("our_start_time", lineParent["start_time"]);
                    readIntervalsChild.Parameters.AddWithValue("our_end_time", lineParent["end_time"]);
                    readIntervalsChild.Parameters.AddWithValue("floor", lineParent["floor"]);
                    readIntervalsChild.Parameters.AddWithValue("zone", lineParent["zone"]);
                    SqlDataAdapter daChild = new SqlDataAdapter(readIntervalsChild);
                    DataTable dtChild = new DataTable();
                    daChild.Fill(dtChild);// Put all data to DataTable format



                    if (dtChild.Rows.Count == 0)
                        Console.Out.WriteAsync("\n\n+------------------+\nCannot SELECT * FROM [intervals]\nour_start_time: " + lineParent["start_time"] + "\nour_end_time: " + lineParent["end_time"] + "\nfloor: " + lineParent["floor"] + "\nzone: " + lineParent["zone"] + "");


                    foreach (DataRow lineChild in dtChild.Rows)// read [intervals]Child line by line
                    {// for each user who has been there
                        using (SqlCommand selectGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE ([user_source_id] = @user_source_id AND [user_target_id] = @user_target_id) OR ([user_source_id] = @user_target_id AND [user_target_id] = @user_source_id) AND [date] = @date AND [zone] = @zone AND [floor] = @floor ORDER BY [date];", sqlConnection))// check if such line exists in [GraphByDate] table
                        {
                            selectGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// ↑user_id
                            selectGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id
                            selectGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);// date should be unique
                            selectGraphByDate.Parameters.AddWithValue("zone", lineParent["zone"]);// zone = ↑zone
                            selectGraphByDate.Parameters.AddWithValue("floor", lineParent["floor"]);// floor = ↑floor
                            int lineExist = 0;
                            if (selectGraphByDate.ExecuteScalar() != null)// else there are no rows with such arguments
                                lineExist = (int)selectGraphByDate.ExecuteScalar();// Get number of rows (check if such line exists in [GraphByDate] table)

                            if (lineExist > 0)// line already exists, then UPDATE
                            {
                                using (SqlCommand updateGraphByDate = new SqlCommand("UPDATE [GraphByDate] SET [duration] = @duration, [times] = @times WHERE ([user_source_id] = @user_source_id AND [user_target_id] = @user_target_id) OR ([user_source_id] = @user_target_id AND [user_target_id] = @user_source_id) AND [date] = @date AND [zone] = @zone AND [floor] = @floor;", sqlConnection))// update
                                {
                                    updateGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// user_id
                                    updateGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id
                                    updateGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);// date should be unique
                                    updateGraphByDate.Parameters.AddWithValue("zone", lineParent["zone"]);// zone = ↑zone
                                    updateGraphByDate.Parameters.AddWithValue("floor", lineParent["floor"]);// floor = ↑floor

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

                                    try
                                    {
                                        updateGraphByDate.ExecuteNonQuery();// execute UPDATE
                                    }
                                    catch (Exception e)
                                    {
                                        Console.Out.WriteAsync("\n\n+------------------+\nCannot UPDATE [GraphByDate]\nuser_source_id: " + lineParent[3] + "\nuser_target_id: " + lineChild[3] + "\ndate: " + Convert.ToDateTime(lineParent["start_time"]).Date + "\nzone: " + lineParent["zone"] + "\nfloor: " + lineParent["floor"] + "");
                                        continue;
                                    }
                                }// UPDATE INTO [GraphByDate]
                            }// if
                            else// there is no such lines, then INSERT
                            {
                                using (SqlCommand insertGraphByDate = new SqlCommand("INSERT INTO [GraphByDate] (date, user_source_id, user_target_id, duration, times, zone, floor) VALUES (@date, @user_source_id, @user_target_id, @duration, @times, @zone, @floor);", sqlConnection))// get all lines from [intervals] where [start_time] is between ↑ start_time and ↑ end_time
                                {
                                    insertGraphByDate.Parameters.AddWithValue("date", Convert.ToDateTime(lineParent["start_time"]).Date);
                                    insertGraphByDate.Parameters.AddWithValue("user_source_id", lineParent[3]);// user_id
                                    insertGraphByDate.Parameters.AddWithValue("user_target_id", lineChild[3]);// user_id
                                    insertGraphByDate.Parameters.AddWithValue("zone", lineParent["zone"]);// zone = ↑zone
                                    insertGraphByDate.Parameters.AddWithValue("floor", lineParent["floor"]);// floor = ↑floor

                                    interval = Convert.ToDateTime(lineChild["end_time"]) - Convert.ToDateTime(lineParent["start_time"]);
                                    intervalMinutes = interval.Minutes;// time difference (duration) in minutes

                                    insertGraphByDate.Parameters.AddWithValue("duration", intervalMinutes);
                                    insertGraphByDate.Parameters.AddWithValue("times", 1);

                                    try
                                    {
                                        insertGraphByDate.ExecuteNonQuery();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.Out.WriteAsync("\n\n+------------------+\nCannot INSERT INTO [GraphByDate]\nuser_source_id: " + lineParent[3] + "\nuser_target_id: " + lineChild[3] + "\ndate: " + Convert.ToDateTime(lineParent["start_time"]).Date + "\nzone: " + lineParent["zone"] + "\nfloor: " + lineParent["floor"] + "");
                                        continue;
                                    }

                                }// INSERT INTO [GraphByDate]
                            }// else
                        }// SELECT * FROM [GraphByDate]                                
                    }// foreach
                }// SELECT * FROM [intervals] WHERE [start_time]

                sqlConnection.Close();// close connection
            }// end of connection

            //Console.Out.WriteLineAsync("Graph Thread left: " + _numerOfThreadsNotYetCompleted.ToString());
            if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                _waitThreads.Set();// Signal that work is finished
        }// fillInGraph

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
                    Console.Out.WriteAsync("\n\n+------------------+\n\nCannot TRUNCATE TABLE [intervals]\nException: " + ex.Message);
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
                              orderby line.timestamp
                              select line;

            int iP = 1;// counter for Progress Change and ID
            _waitThreads = new ManualResetEvent(false);
            _numerOfThreadsNotYetCompleted = timeGapList.Count() - 1;
            foreach (var line in timeGapList)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(fillInIntervals), new object[] { line, timeGapList, iP, timeGapList.Count() });
                //Console.Out.WriteLineAsync("And left: " + Convert.ToString(timeGapList.Count() - iP));
                iP++;
            }// end of foreach
            _waitThreads.WaitOne();// Wait until the task is complete

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
                        Console.Out.WriteAsync("\n\n+------------------+\n\nCannot TRUNCATE TABLE [GraphByDate]\nException: " + ex.Message);
                        //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }// if cannot delete table. Most likely table doesn't exist
                }

                using (SqlCommand readIntervalsParent = new SqlCommand("SELECT * FROM [intervals] ORDER BY [start_time]", sqlConnection))// First, read all table [intervals]
                {
                    SqlDataAdapter da = new SqlDataAdapter(readIntervalsParent);
                    DataTable dt = new DataTable();
                    da.Fill(dt);// Put all data to DataTable formay

                    _waitThreads = new ManualResetEvent(false);
                    _numerOfThreadsNotYetCompleted = dt.Rows.Count;
                    iP = 1;// counter for Progress Change and ID
                    foreach (DataRow lineParent in dt.Rows)// read [intervals] line by line
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(fillInGraph), new object[] { lineParent, iP, dt.Rows.Count });
                        iP++;
                    }// foreach
                    _waitThreads.WaitOne();// Wait until the task is complete
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

                using (SqlCommand readGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE [user_source_id] = @user_source_id;", sqlConnection))// First, read whole table [GraphByDate]
                {
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





                    //}//foreach
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
            labelWorkinProgress.Visible = true;// Work in progress...
            if (Convert.ToDateTime(comboBoxTill.SelectedItem.ToString()).Subtract(Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString())) <= TimeSpan.Zero)
            {
                MessageBox.Show("Please make sure that your chosen left date earlier than the right one\nDebug info: Line 208, method: buttonFind_Click", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;// 
            }
            FCSO.timeFrom = Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString());
            FCSO.timeTill = Convert.ToDateTime(comboBoxTill.SelectedItem.ToString());

            buttonFind.Enabled = false;// prevent double launch of backgroundWorkerTable
            progressBar1.Visible = true;
            backgroundWorkerTable.RunWorkerAsync(FCSO);
            Console.Out.WriteLineAsync("backgroundWorkerTable - started");
            progressBarSteps.PerformStep();
        }// end of buttonFind_Click

        private void backgroundWorkerTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonFind.Enabled = true;// prevent double launch of backgroundWorkerTable
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            labelWorkinProgress.Visible = false;// Work in progress...
            Console.Out.WriteLineAsync("backgroundWorkerTable - finished");
        }// end of backgroundWorkerTable_RunWorkerCompleted

        private void backgroundWorkerTable_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;// Change the value of the ProgressBar to the BackgroundWorker progress
        }// end of backgroundWorkerTable_ProgressChanged

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void buttonFind_MouseEnter(object sender, EventArgs e)
        {
        }

        private void buttonFind_MouseLeave(object sender, EventArgs e)
        {
        }

        private void linkLabelGenerateGraphs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            updateUserList();
            if (FCSO.userList == null)
            {
                MessageBox.Show("Cannot identify users \nFCSO.userList == null");
                return;
            }
            foreach (KeyValuePair<User, int> userPair in FCSO.userList)// add all users to ThreadPool to save files
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(saveToFileByUser), userPair.Key);
            }// foreach
        }// linkLabelGenerateGraphs_LinkClicked

        private void saveToFileByUser(object userObj)
        {
            if (userObj != null)
            {
                User user = (User)userObj;// associate user

                using (SqlConnection sqlConnection = new SqlConnection(FCSO.connectionString))
                {
                    sqlConnection.Open();// open connection

                    using (SqlCommand readGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE [user_source_id] = @user_source_id OR [user_target_id] = @user_source_id;", sqlConnection))// First, read whole table [GraphByDate]
                    {
                        readGraphByDate.Parameters.AddWithValue("user_source_id", user.id);// 

                        SqlDataAdapter da = new SqlDataAdapter(readGraphByDate);
                        DataTable dt = new DataTable();
                        da.Fill(dt);// Put all data to DataTable formay

                        string path = "users/" + user.id + "_user_" + user.lastName + "_" + user.firstName + ".txt";
                        try
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                            // Create the file.
                            using (FileStream fs = File.Create(path))
                            {
                                Byte[] info = new UTF8Encoding(true).GetBytes("date (whole day); user1; user2; average duration (sec); times; floor-zone\n");
                                // Add some information to the file.
                                fs.Write(info, 0, info.Length);

                                int average_duration = 0;
                                foreach (DataRow line in dt.Rows)// read [GraphByDate] line by line
                                {
                                    average_duration = (Convert.ToInt32(line["duration"]) * 60) / Convert.ToInt32(line["times"]);
                                    info = new UTF8Encoding(true).GetBytes(line["date"] + ";" + line["user_source_id"] + ";" + line["user_target_id"] + ";" + average_duration.ToString() + ";" + line["times"] + ";" + line["floor"] + "-" + line["zone"] + "\n");
                                    // Add some information to the file.
                                    fs.Write(info, 0, info.Length);

                                }//foreach
                            }// using
                        }// try

                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }// SELECT * FROM [GraphByDate]

                    sqlConnection.Close();// close connection
                }// end of connection
            }// if
        }// getUserList

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            /*openFileDialogChooseFile.Filter = "CSV files|*.csv";
            openFileDialogChooseFile.Title = "Select a Log file";*/
            DialogResult result = openFileDialogChooseFile.ShowDialog(); // Show the dialog.
        }

        private void groupBoxSettings_Enter(object sender, EventArgs e)
        {

        }

        private void comboBoxTill_SelectedIndexChanged(object sender, EventArgs e)
        {
            progressBarSteps.PerformStep();/// Perform the increment on the ProgressBar
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void labelBar1_Click(object sender, EventArgs e)
        {

        }

        private void labelHorizontalBar1_Click(object sender, EventArgs e)
        {

        }

        private void progressBarSteps_Click(object sender, EventArgs e)
        {

        }

        private void labelTo_Click(object sender, EventArgs e)
        {

        }

        private void labelFrom_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxFrom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void labelWorkinProgress_Click(object sender, EventArgs e)
        {

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void progressBar3_Click(object sender, EventArgs e)
        {

        }
    }// end of class MainForm : Form
}
