## Download
[Last Release](https://github.com/AlexeyVolkov/PACS-Analyzer/releases/download/v0.8/PACS.Analyzer.April.2018.zip)

[Source code](https://github.com/AlexeyVolkov/PACS-Analyzer/releases/tag/v0.8)

# PACS Analyzer
##### _Physical Access Control System Analazyer_

The system for detecting anomalies in the behavior of employees based on Physical Access Control Systems data

![Preview of PACS Analyzer main window](https://pp.userapi.com/c830309/v830309716/e364c/Os5_AhB9cpc.jpg "PACS Analyzer main window")

## Structure
```
PACS Analyzer
│   Anomalies.sql           <-- SQL commands to find anomalies from [GraphByDate] table
│   DatabaseMain.mdf        <-- DataBase file containing all the tables
│   Example.csv             <-- Example PACS log file (contains a few users and few weeks of observation
│   FileHelpers.dll         <-- Third-party library for parsing CVS files
│   GraphByDateFillIn.sql   <-- SQL commands to fill in [GraphByDate] table from [Intervals] table
│   PACS Analyzer.exe       <-- Main executable file. The Program.
│
└───anomalies                     <-- Folder appears after Anomalies were found
│   │   anomalies.txt             <-- Contains a table of odd behavior of employees
└───users                         <-- Folder appears after Vectors were saved
    │   1_user_Dedos_Lidelse.txt  <-- Contains a list of employee`s meetings
    │   2_user_Onda_Marin.txt
    │   ...
```
## The Idea
###### The parts of code below highlight the main points
You run the program and select the log file to search for anomalies.
```cs
openFileDialogChooseFile.Filter = "CSV files|*.csv";

...

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
```
You can configure the desired time period and run the main algorithm.
```cs
backgroundWorkerTable.RunWorkerAsync(_formCoreGlobalObject);
```
First, it fills in the [Intervals] table
```cs
SqlCommand emptyUsers = new SqlCommand("TRUNCATE TABLE [intervals];", sqlConnection);

...

// List of lines with User(client) filter
var timeGapList = from line in result
                  where line.timestamp >= _formCoreGlobalObject.timeFrom && line.timestamp <= _formCoreGlobalObject.timeTill
                  orderby line.timestamp
                  select line;

...

foreach (var line in timeGapList)
{
  ThreadPool.QueueUserWorkItem(new WaitCallback(fillInIntervals), new object[] { line, timeGapList });
}

...

_waitThreads.WaitOne();// Wait until the task is complete
```
###### Since the SQL code is almost the core of the program, here are just links without description
Second, it runs [GraphByDateFillIn.sql](https://github.com/AlexeyVolkov/PACS-Analyzer/blob/master/PACS%20Analyzer/bin/Debug/GraphByDateFillIn.sql)

Absolutely the same happens next with [Anomalies.sql](https://github.com/AlexeyVolkov/PACS-Analyzer/blob/master/PACS%20Analyzer/bin/Debug/Anomalies.sql)

Profit!

Now all the data stores in [AnomaliesTEMP] table for Anomalies and in [Intervals] for Vectors.

To save Vectors click relevant button.
```cs
_stopWatch.Start();// start time
this.Cursor = Cursors.WaitCursor;// Set wait cursor
foreach (KeyValuePair<User, int> userPair in _formCoreGlobalObject.userList)// add all users to ThreadPool to save files
  {
    ThreadPool.QueueUserWorkItem(new WaitCallback(saveToFileByUser), userPair.Key);
  }
_stopWatch.Stop();// stop time
 richTextBoxInfo.Text = _vectorsSavedMessage + " " + _stopWatch.Elapsed.Milliseconds + _secondsLabel;
this.Cursor = Cursors.Default;// UNset wait cursor
```
To save Anomalies click relevant button.
```cs
using (SqlCommand readAnomalies = new SqlCommand("SELECT * FROM [AnomaliesTEMP];", sqlConnection))// First, read whole table [AnomaliesTEMP]
{
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

fs.Write(info, 0, info.Length);
}
```

## User Instructions
![Preview of PACS Analyzer main window with numbers](https://pp.userapi.com/c846221/v846221535/c22a/RkMIEmOZ02M.jpg "PACS Analyzer main window with numbers")
* [1] Click the button (_Browse the log file..._)
* Pick the file Example.csv from the folder with a project
* [2] Choose time gap with two drop-down date lists
* [3] Click (_Find anomalies_) to start the main algorithm
* [4] You may watch the progress below (it may take time to analyze big files)
##### Result
* Two buttons to save Anomalies and Vectors

## Third-party

* [Visual Studio Community](https://www.visualstudio.com/vs/community/)
* [CSV Parser](https://www.filehelpers.net/)

## Useful Links
* [User List](https://github.com/uic-evl/EventEvent2016/blob/master/data/csv/Employee%20List.csv)
* [Как думать на SQL? - Хабр](https://habrahabr.ru/post/305926/)
* [Prettify SQL](http://www.dpriver.com/pp/sqlformat.htm)
