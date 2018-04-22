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
using Microsoft.SqlServer.Server;
using System.Diagnostics;// Stop Watch

namespace PACS_Analyzer
{


    public partial class MainForm : Form
    {
        /*
         * Global Variables
         */
        FormCoreShare _formCoreGlobalObject = new FormCoreShare();// class for storing information between Core and Form
        private static ManualResetEvent _waitThreads;// to wait untill all threads are done
        private static int _numerOfThreadsNotYetCompleted;// we don't use WaitHandle.WaitAll since we've got more than 64 threads
        private static string _vectorsFolder = "users";
        private string _vectorsSavedMessage = "Vectors have been saved to the folder /" + _vectorsFolder + "/";
        private static string _anomaliesFolder = "anomalies";
        private string _anomaliesSavedMessage = "Anomalies have been saved to the folder /" + _anomaliesFolder + "/";
        private string _anomaliesFound = "Anomalies were found in";
        private string _secondsLabel = " ms";
        private string _workInProgress = "Work in progress...";
        private string _infoText = "[1] Click the button (Browse the log file...)\nPick the file Example.csv from the folder with a project\n[2] Choose time gap with two drop-down date lists\n[3] Click (Find anomalies) to start the main algorithm\n[4] You may watch the progress below (it may take time to analyze big files)";
        private Stopwatch _stopWatch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();

            _formCoreGlobalObject.filePath = "default.csv";
            _formCoreGlobalObject.connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Navalny\Documents\нир\App\PACS Analyzer\PACS Analyzer\DatabaseMain.mdf;Integrated Security=True;MultipleActiveResultSets=True";

            backgroundWorkerTable.WorkerReportsProgress = true;

            progressBarSteps.Minimum = 0;
            progressBarSteps.Maximum = 3;// Number of steps on the form
            progressBarSteps.Value = 0;// Set the initial value of the ProgressBar
            progressBarSteps.Step = 1;// Set the Step value
            richTextBoxInfo.Text = _infoText;
        }

        private void openFileDialogChooseFile_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                _formCoreGlobalObject.filePath = openFileDialogChooseFile.FileName;
                _formCoreGlobalObject.fileName = openFileDialogChooseFile.SafeFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot open the file error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            progressBarSteps.Visible = true;
            _waitThreads = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(new WaitCallback(updateUserList));// start gathering user information in a Thread
            _waitThreads.WaitOne();// Wait until the task is complete
            progressBarSteps.PerformStep();
        }// openFileDialogChooseFile

        private void updateUserList(Object x = null)
        {
            var engine = new FileHelperEngine<CSVReader>();
            CSVReader[] result = new CSVReader[0];
            try
            {
                result = engine.ReadFile(_formCoreGlobalObject.filePath);// read the file
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot find the file:\n" + _formCoreGlobalObject.filePath + "\n\n" + e.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _waitThreads.Set();// Signal that work is finished
                return;
            }
            _formCoreGlobalObject.fileLinesNumber = result.Length;
            _formCoreGlobalObject.userList = new Dictionary<User, int>();

            var uniqueUsers = result.GroupBy(p => p.empid)// find unique Users
                            .Select(g => g.First())
                            .ToList();

            int iForUser = 1;
            User temp;
            foreach (var line in uniqueUsers)
            {// make a new User obj and add to FormCoreShare obj
                temp = new User(iForUser, line.empid, line.a, line.firstname, line.lastname, line.department);
                _formCoreGlobalObject.userList.Add(temp, iForUser);
                iForUser++;
            }
            _waitThreads.Set();// Signal that work is finished
        }// Look through all file and update User list

        private bool updateDateGap()
        {
            var engine = new FileHelperEngine<CSVReader>();
            CSVReader[] result = new CSVReader[0];
            try
            {
                result = engine.ReadFile(_formCoreGlobalObject.filePath);// read the file
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot find the file:\n" + _formCoreGlobalObject.filePath + "\n\n" + e.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            _formCoreGlobalObject.fileLinesNumber = result.Length;
            _formCoreGlobalObject.comboBoxTimes = new List<DateTime>();

            for (int i = 0; i < _formCoreGlobalObject.fileLinesNumber - 2; i = i + Convert.ToInt32(_formCoreGlobalObject.fileLinesNumber / 20))
            {
                _formCoreGlobalObject.comboBoxTimes.Add(result[i].timestamp);
            }
            _formCoreGlobalObject.comboBoxTimes.Add(result[_formCoreGlobalObject.fileLinesNumber - 2].timestamp);// add dates to MainForm

            return true;
        }// updateDateGap (from openFileDialogChooseFile_FileOk)

        private void fillInIntervals(object x)
        {
            // parse object x
            object[] array = x as object[];
            CSVReader line = array[0] as CSVReader;
            IOrderedEnumerable<CSVReader> timeGapList = array[1] as IOrderedEnumerable<CSVReader>;

            TimeSpan interval = new TimeSpan();
            int intervalMinutes = -1;
            int timeGapListCount = timeGapList.Count();// length of timeGapList

            // next timeStamp for this user. FINISH time
            var nextTimeLine = (from nextTime in timeGapList
                                where nextTime.empid == line.empid && nextTime.timestamp > line.timestamp
                                orderby nextTime.timestamp
                                select nextTime).FirstOrDefault();
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
                intervalMinutes = interval.Hours * 60 + interval.Minutes;// time difference (duration) in minutes
            }// try
            catch (Exception e)// there is no FINISH time
            {
                Console.Out.WriteAsync("\n\n+------------------+\n\nNo finish time\nException: " + e.Message);
            }

            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
            {
                sqlConnection.Open();// open connection

                SqlCommand insertIntervals = new SqlCommand("INSERT INTO [intervals] (start_time, end_time, user_id, floor, zone, duration) VALUES (@start_time, @end_time, @user_id, @floor, @zone, @duration)", sqlConnection);
                insertIntervals.Parameters.AddWithValue("start_time", line.timestamp);
                insertIntervals.Parameters.AddWithValue("end_time", nextTimeLine.timestamp);
                insertIntervals.Parameters.AddWithValue("user_id", _formCoreGlobalObject.userList.First(g => g.Key.empID == line.empid).Key.id);// find UserId from FormCoreShare obj
                insertIntervals.Parameters.AddWithValue("duration", intervalMinutes);
                insertIntervals.Parameters.AddWithValue("floor", line.floor);
                insertIntervals.Parameters.AddWithValue("zone", line.zone);
                try
                {
                    insertIntervals.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.Out.WriteAsync("\n\n+------------------+\n\nthe end of list for this user\nException: " + e.Message);//the end of list for this user
                }// catch

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
            backgroundWorkerTable.ReportProgress((iP * 100) / dtRowsCount, new object[] { 2 });

            TimeSpan interval = new TimeSpan();
            int intervalMinutes = -1;

            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
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
                                        continue;// skip this lineChild
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
                                        continue;// skip this lineChild
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
            /*
            * fill table INTERVALS
            **/
            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
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
                }// if cannot delete table. Most likely table doesn't exist
                sqlConnection.Close();// close connection
            }// end of connection

            var engine = new FileHelperEngine<CSVReader>();
            var result = engine.ReadFile(_formCoreGlobalObject.filePath);// read the file
            _formCoreGlobalObject.fileLinesNumber = result.Length;

            // List of lines with User(client) filter
            var timeGapList = from line in result
                              where line.timestamp >= _formCoreGlobalObject.timeFrom && line.timestamp <= _formCoreGlobalObject.timeTill
                              orderby line.timestamp
                              select line;

            int iP = 1;// counter for Progress Change and ID
            _waitThreads = new ManualResetEvent(false);
            _numerOfThreadsNotYetCompleted = timeGapList.Count() - 1;
            foreach (var line in timeGapList)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(fillInIntervals), new object[] { line, timeGapList });
                backgroundWorkerTable.ReportProgress((iP * 100) / timeGapList.Count(), new object[] { 1 });
                iP++;
            }// end of foreach
            _waitThreads.WaitOne();// Wait until the task is complete

            /*
            * fill table GRAPHBYDAY
            **/


            /*
            SqlDataAdapter da;
            DataTable dt;
            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
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
                    }// if cannot delete table. Most likely table doesn't exist
                }// TRUNCATE TABLE [GraphByDate]

                using (SqlCommand readIntervalsParent = new SqlCommand("SELECT * FROM [intervals] ORDER BY [start_time]", sqlConnection))// First, read all table [intervals]
                {
                    da = new SqlDataAdapter(readIntervalsParent);
                }// SELECT * FROM [intervals]

                dt = new DataTable();
                da.Fill(dt);// Put all data to DataTable format
                sqlConnection.Close();// close connection
            }// end of connection
            
            _waitThreads = new ManualResetEvent(false);
            _numerOfThreadsNotYetCompleted = dt.Rows.Count;
            iP = 1;// counter for Progress Change and ID
            foreach (DataRow lineParent in dt.Rows)// read [intervals] line by line
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(fillInGraph), new object[] { lineParent, iP, dt.Rows.Count });
                iP++;
            }// foreach
            _waitThreads.WaitOne();// Wait until the task is complete

            */

            backgroundWorkerTable.ReportProgress(100, new object[] { 2 });
            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
            {
                sqlConnection.Open();// open connection

                FileInfo file = new FileInfo("GraphByDateFillIn.sql");
                string script = file.OpenText().ReadToEnd();

                using (SqlCommand GraphByDateFillIn = new SqlCommand(script, sqlConnection))//Empty table intervals
                {
                    try
                    {
                        GraphByDateFillIn.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteAsync("\n\n+------------------+\n\nCannot GraphByDateFillIn\nException: " + ex.Message);
                    }// if cannot delete table. Most likely table doesn't exist
                }// TRUNCATE TABLE [GraphByDate]

                sqlConnection.Close();// close connection
            }// end of connection

            /*
            * Anomalies search
            **/


            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
            {
                sqlConnection.Open();// open connection

                FileInfo file = new FileInfo("Anomalies.sql");
                string script = file.OpenText().ReadToEnd();

                using (SqlCommand Anomalies = new SqlCommand(script, sqlConnection))//Empty table intervals
                {
                    try
                    {
                        Anomalies.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.Out.WriteAsync("\n\n+------------------+\n\nCannot Anomalies\nException: " + ex.Message);
                    }// if cannot delete table. Most likely table doesn't exist
                }// 

                sqlConnection.Close();// close connection
            }// end of connection



            //using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
            //{
            //sqlConnection.Open();// open connection
            //using (SqlCommand readGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE [user_source_id] = @user_source_id;", sqlConnection))// First, read whole table [GraphByDate]
            //{
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
            //}// SELECT * FROM [GraphByDate]
            //sqlConnection.Close();// close connection
            //}// end of connection
        }// end of backgroundWorkerTable_DoWork

        private void buttonFind_Click(object sender, EventArgs e)
        {
            _stopWatch.Reset();
            _stopWatch.Start();// start time

            if (Convert.ToDateTime(comboBoxTill.SelectedItem.ToString()).Subtract(Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString())) <= TimeSpan.Zero)
            {
                MessageBox.Show("Please make sure that your \n`" + labelFrom.Text + "`\nis earlier than\n`" + labelTo.Text + "`", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // get dates from date pickers on the form
            _formCoreGlobalObject.timeFrom = Convert.ToDateTime(comboBoxFrom.SelectedItem.ToString());
            _formCoreGlobalObject.timeTill = Convert.ToDateTime(comboBoxTill.SelectedItem.ToString());

            // prevent double launch of backgroundWorkerTable
            buttonFind.Enabled = false;
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            progressBar2.Visible = true;
            progressBar2.Value = 0;
            progressBar3.Visible = true;
            progressBar3.Value = 0;

            this.Cursor = Cursors.WaitCursor;// Set wait cursor
            richTextBoxInfo.Visible = true;// Work in progress...
            richTextBoxInfo.Text = _workInProgress;
            // run threads
            backgroundWorkerTable.RunWorkerAsync(_formCoreGlobalObject);
            Console.Out.WriteLineAsync("backgroundWorkerTable - started");

            progressBarSteps.PerformStep();
        }// Find Anomalies Button Clicked

        private void backgroundWorkerTable_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _stopWatch.Stop();// stop time
            buttonFind.Enabled = true;// prevent double launch of backgroundWorkerTable
            progressBar1.Visible = false;
            progressBar2.Visible = false;
            progressBar3.Visible = false;
            richTextBoxInfo.Text = _anomaliesFound + " " + _stopWatch.Elapsed.Milliseconds + _secondsLabel;// Work in progress...
            this.Cursor = Cursors.Default;// UNset wait cursor

            Console.Out.WriteLineAsync("backgroundWorkerTable - finished");
        }// end of backgroundWorkerTable_RunWorkerCompleted

        private void backgroundWorkerTable_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object[] array = e.UserState as object[];
            if (array == null)
                return;
            if ((int)array[0] == 1)
                progressBar1.Value = e.ProgressPercentage;
            else
                progressBar2.Value = e.ProgressPercentage;
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
            _stopWatch.Reset();
            _stopWatch.Start();// start time

            this.Cursor = Cursors.WaitCursor;// Set wait cursor
            updateUserList();
            if (_formCoreGlobalObject.userList == null)
            {
                MessageBox.Show("Cannot identify users \nFCSO.userList == null");
                return;
            }
            int iP = 0;
            progressBar3.Visible = true;
            foreach (KeyValuePair<User, int> userPair in _formCoreGlobalObject.userList)// add all users to ThreadPool to save files
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(saveToFileByUser), userPair.Key);
                progressBar3.Value = (iP * 100) / _formCoreGlobalObject.userList.Count();
                iP++;
            }// foreach
            progressBar3.Visible = false;

            _stopWatch.Stop();// stop time
            richTextBoxInfo.Text = _vectorsSavedMessage + " " + _stopWatch.Elapsed.Milliseconds + _secondsLabel;
            richTextBoxInfo.Visible = true;
            this.Cursor = Cursors.Default;// UNset wait cursor
        }// linkLabelGenerateGraphs_LinkClicked

        private void saveToFileByUser(object userObj)
        {
            if (userObj != null)
            {
                User user = (User)userObj;// associate user

                using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
                {
                    sqlConnection.Open();// open connection

                    using (SqlCommand readGraphByDate = new SqlCommand("SELECT * FROM [GraphByDate] WHERE [user_source_id] = @user_source_id OR [user_target_id] = @user_source_id;", sqlConnection))// First, read whole table [GraphByDate]
                    {
                        readGraphByDate.Parameters.AddWithValue("user_source_id", user.id);// 

                        SqlDataAdapter da = new SqlDataAdapter(readGraphByDate);
                        DataTable dt = new DataTable();
                        da.Fill(dt);// Put all data to DataTable formay

                        string dirPath = "users";
                        System.IO.Directory.CreateDirectory(dirPath);
                        string path = dirPath + '/' + user.id + "_user_" + user.lastName + "_" + user.firstName + ".txt";
                        try
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                            // Create the file.
                            using (FileStream fs = File.Create(path))
                            {
                                Byte[] info = new UTF8Encoding(true).GetBytes("date (whole day); user1; user2; average duration (sec); times; floor;zone\n");
                                // Add some information to the file.
                                fs.Write(info, 0, info.Length);

                                int average_duration = 0;
                                foreach (DataRow line in dt.Rows)// read [GraphByDate] line by line
                                {
                                    average_duration = (Convert.ToInt32(line["duration"]) * 60) / Convert.ToInt32(line["times"]);
                                    info = new UTF8Encoding(true).GetBytes(
                                        line["date"]
                                        + ";"
                                        + _formCoreGlobalObject.userList.FirstOrDefault(x => x.Value == Convert.ToInt32(line["user_source_id"])).Key.empID
                                        + ";"
                                        + _formCoreGlobalObject.userList.FirstOrDefault(x => x.Value == Convert.ToInt32(line["user_target_id"])).Key.empID
                                        + ";"
                                        + average_duration.ToString()
                                        + ";"
                                        + line["times"]
                                        + ";"
                                        + line["floor"]
                                        + ";"
                                        + line["zone"]
                                        + "\n");
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
            }// if userObj==null
        }// getUserList

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            openFileDialogChooseFile.Filter = "CSV files|*.csv";
            openFileDialogChooseFile.Title = "Select a Log file";
            DialogResult result = openFileDialogChooseFile.ShowDialog(); // Show the dialog.

            // DataSources for DropDown Lists
            if (updateDateGap() == false)
                return;

            // From
            BindingSource forComboBoxFrom = new BindingSource();
            forComboBoxFrom.DataSource = _formCoreGlobalObject.comboBoxTimes;
            comboBoxFrom.DataSource = forComboBoxFrom;
            comboBoxFrom.Enabled = true;
            // Till
            BindingSource forComboBoxTill = new BindingSource();
            forComboBoxTill.DataSource = _formCoreGlobalObject.comboBoxTimes;
            comboBoxTill.DataSource = forComboBoxTill;
            comboBoxTill.Enabled = true;
            // Make rest of the form visible
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            labelFrom.Visible = true;
            labelTo.Visible = true;
            comboBoxFrom.Visible = true;
            comboBoxTill.Visible = true;
            linkLabelGenerateGraphs.Visible = true;
            linkLabelSaveAnomalies.Visible = true;
            labelDelimeter.Visible = true;

            progressBarSteps.PerformStep();
        }// Browse the log file Button Click

        private void groupBoxSettings_Enter(object sender, EventArgs e)
        {

        }

        private void comboBoxTill_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonFind.Enabled = true;
            buttonFind.Visible = true;
        }// We clicked to second date picker

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

        private void linkLabelSaveAnomalies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _stopWatch.Reset();
            _stopWatch.Start();// start time

            this.Cursor = Cursors.WaitCursor;// Set wait cursor
            updateUserList();
            if (_formCoreGlobalObject.userList == null)
            {
                MessageBox.Show("Cannot identify users \nFCSO.userList == null");
                return;
            }
            int iP = 0;
            progressBar3.Visible = true;

            using (SqlConnection sqlConnection = new SqlConnection(_formCoreGlobalObject.connectionString))
            {
                sqlConnection.Open();// open connection

                using (SqlCommand readAnomalies = new SqlCommand("SELECT * FROM [AnomaliesTEMP];", sqlConnection))// First, read whole table [AnomaliesTEMP]
                {
                    SqlDataAdapter da = new SqlDataAdapter(readAnomalies);
                    DataTable dt = new DataTable();
                    da.Fill(dt);// Put all data to DataTable formay

                    System.IO.Directory.CreateDirectory(_anomaliesFolder);
                    string path = _anomaliesFolder + '/' + "anomalies" + ".txt";
                    try
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                        // Create the file.
                        using (FileStream fs = File.Create(path))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes("date (whole day); user1; user2; odd duration (sec); average duration (sec); odd times; average times (sec); floor;zone\n");
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);

                            int odd_duration = 0;
                            int average_duration = 0;
                            foreach (DataRow line in dt.Rows)// read [AnomaliesTEMP] line by line
                            {
                                odd_duration = (Convert.ToInt32(line["duration"]) * 60) / Convert.ToInt32(line["times"]);
                                //average_duration = (Convert.ToInt32(line["duration_AVG"]) * 60) / Convert.ToInt32(line["times"]);
                                info = new UTF8Encoding(true).GetBytes(
                                    line["date"]
                                    + ";"
                                    + _formCoreGlobalObject.userList.FirstOrDefault(x => x.Value == Convert.ToInt32(line["user_source_id"])).Key.empID
                                    + ";"
                                    + _formCoreGlobalObject.userList.FirstOrDefault(x => x.Value == Convert.ToInt32(line["user_target_id"])).Key.empID
                                    + ";"
                                    + odd_duration.ToString()
                                    + ";"
                                    + "0"
                                    + ";"
                                    + line["times"]
                                    + ";"
                                    + line["times_AVG"]
                                    + ";"
                                    + line["floor"]
                                    + ";"
                                    + line["zone"]
                                    + "\n");
                                // Add some information to the file.
                                fs.Write(info, 0, info.Length);

                            }//foreach
                        }// using
                    }// try

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }// SELECT * FROM [AnomaliesTEMP]

                sqlConnection.Close();// close connection
            }// end of connection

            progressBar3.Visible = false;

            _stopWatch.Stop();// stop time
            richTextBoxInfo.Text = _anomaliesSavedMessage + " " + _stopWatch.Elapsed.Milliseconds + _secondsLabel;
            richTextBoxInfo.Visible = true;
            this.Cursor = Cursors.Default;// UNset wait cursor

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(richTextBoxInfo.Text, labelInfoText.Text);
        }
    }// end of class MainForm : Form
}
