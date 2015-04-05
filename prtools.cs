using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using MSExcel = Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Xml;
using System.Threading;

namespace PRTools
{
    public partial class prtools : Form
    {
        public prtools()
        {
            InitializeComponent();
            this.typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.sortVar.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dasdFileLoadVar.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dasdTopOutputVar.DropDownStyle = ComboBoxStyle.DropDownList;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         * 更新进度指示器
         */
        Thread uithread = null;
        Thread workthread = null;
        private void UpdateProgressThread()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                this.Invoke(new Action<int>(this.UpdateProgress), i);
            }
        }

        /*
         * 进度指示器
         */
        private void UpdateProgress(int v)
        {
            this.progressBar1.Value = v;
        }

        /*
         * 执行分析功能方法，通过Case控制分析类型和需要分析的内容
         */
        public void fileProcess_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = fileTextBox.Text;//文件路径，通过对话框选择得到
                string caseVar = "";
                fileText.Text = "";

                if (typeComboBox.Text == null || typeComboBox.Text == "")
                {
                    MessageBox.Show("请先选择分析类型"); //如果没有选择分析类型，提示必须先选择分析类型
                }
                else if (fileTextBox.Text == null || fileTextBox.Text == "")
                {
                    MessageBox.Show("请先选择监控结果文件");//如果没有选择监控结果文件，提示必须先选择监控结果文件
                }
                else
                {
                    caseVar = typeComboBox.Text.Substring(2, 3);//Case控制结果变量对分析类型 1.CPU 使用分析 2.DASD使用分析 3.TTRN交易分析 4.TRNR笔数分析
                }
                switch (caseVar)
                {
                    case "CPU":
                        {
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            workthread = new Thread(new ThreadStart(this.RegexCpuRate));
                            workthread.Start();
                            //tempStr = RegexCpuRate(fileName);//生成CPU 使用分析数据
                        }
                        break;
                    case "DAS":
                        {
                            //FileInfo fi = new FileInfo(fileName);
                            //if (fi.Length >= 3145728 && dasdFileLoadVar.Text == "否")
                            //{
                            //    MessageBox.Show("DASD分析只支持3M以下文件");
                            //}
                            //else
                            //{
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.RegexDASDRate));
                            workthread.Start();

                            //tempStr = RegexDASDRate(fileName);//生成DASD使用分析数据
                            //}
                        }
                        break;
                    case "TTR":
                        {
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.RegexTTRNRate));
                            workthread.Start();
                            //tempStr = RegexTTRNRate(fileName);//生成TTRN交易分析数据
                        }
                        break;
                    case "TRN":
                        {
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.RegexTRNRRate));
                            workthread.Start();
                            //tempStr = RegexTRNRRate(fileName);//生成TRNR笔数分析数据
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*
         * 生成CPU 使用分析数据
         */
        public void RegexCpuRate()
        {
            string tempPr = null, fileLine = null, lparName = null, timePoint = "", dataSortStr = "", fileName = fileTextBox.Text;
            int i = 0;
            try
            {
                StreamReader sr = File.OpenText(fileName);
                bool varTemp = false;
                this.label5.Text = "当前分析行数";
                while ((fileLine = sr.ReadLine()) != null)
                {
                    Regex regCPUActivity = new Regex("C P U  A C T I V I T Y");//匹配CPU ACTIVITY的数据
                    Match matCPUActivity = regCPUActivity.Match(fileLine);
                    i++;
                    this.textBoxRows.Text = Convert.ToString(i);
                    if (matCPUActivity.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                        varTemp = false;
                    }
                    Regex regSystemID = new Regex("SYSTEM ID");//匹配SYSTEM ID的数据
                    Match matSystemID = regSystemID.Match(fileLine);
                    if (matSystemID.Success && !varTemp)
                    {
                        int lineNum = Convert.ToInt16(matSystemID.Index.ToString());
                        lparName = fileLine.Substring(lineNum + 9, 10).Trim();//取得监控数据头LparName
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regRMFTime = new Regex("RMF       TIME");//匹配RMF TIME的数据
                    Match matRMFTime = regRMFTime.Match(fileLine);
                    if (matRMFTime.Success && !varTemp)
                    {
                        timePoint = fileLine.Substring(70, 8);
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regNumType = new Regex(" NUM  TYPE");//匹配 NUM TYPE的数据
                    Match matNumType = regNumType.Match(fileLine);
                    if (matNumType.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regCPMatch = new Regex(" CP     100.00");//匹配 CP 100.00的数据
                    Match matCPMatch = regCPMatch.Match(fileLine);
                    if (matCPMatch.Success)
                    {
                        tempPr = tempPr + lparName + "  " + fileLine + "\r\n";//拼接上LparName，用于数据比对
                    }
                    Regex regTotalAverage = new Regex(" TOTAL/AVERAGE");//匹配 TOTAL/AVERAGE的数据
                    Match matTotalAverage = regTotalAverage.Match(fileLine);
                    if (matTotalAverage.Success)
                    {
                        tempPr = tempPr + lparName + "  " + timePoint + fileLine + "\r\n";
                        varTemp = true;
                    }
                }
                this.fileText.Text = tempPr;//窗口显示结果数据
                dataSortStr = RegexCPUDataSort(tempPr);
                this.fileText.Text = dataSortStr;
                if (this.fileText.Text != null && this.fileText.Text != "")
                {
                    MessageBox.Show("CPU执行分析已完成");
                }
                this.EndPublicPropertiesGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            //return dataSortStr;//返回结果数据
        }

        /*
         * 对结果数据按Lpar先后顺序排列数据
         */
        public string RegexCPUDataSort(string inTempstr)
        {
            string tempStr = "", lparName = "", tempLparName = "", lpArrStr = "", lineTempStr = "";
            bool lparNameVar = true;
            int i = 0;
            try
            {
                this.label5.Text = "当前排序行数";
                foreach (string linestr in fileText.Lines)
                {
                    Regex regTotalAverage = new Regex(" TOTAL/AVERAGE");
                    Match matTotalAverage = regTotalAverage.Match(linestr);
                    if (matTotalAverage.Success)
                    {
                        lparName = linestr.Substring(0, 6).Trim();
                        if (tempLparName != "" && tempLparName != lparName)
                        {
                            tempLparName = lparName;
                            if (!lpArrStr.Contains(lparName))
                            {
                                lpArrStr = lpArrStr + "," + lparName;
                            }
                        }
                        else if (tempLparName != lparName)
                        {
                            if (lparNameVar)
                            {
                                lpArrStr = lpArrStr + "," + lparName;
                                lparNameVar = false;
                            }
                            tempLparName = lparName;
                        }
                        tempStr = tempStr + linestr + "\r\n";
                    }
                    Regex regCPMatch = new Regex("CP     100.00");
                    Match matCPMatch = regCPMatch.Match(linestr);
                    if (matCPMatch.Success)
                    {
                        tempStr = tempStr + linestr + "\r\n";
                    }
                }
                fileText.Text = tempStr;
                string[] lparNameArray = LparNameArray(lpArrStr.Substring(1));
                for (int a = 0; a < lparNameArray.Length; a++)
                {
                    foreach (string linestragain in fileText.Lines)
                    {
                        Regex regLparName = new Regex(lparNameArray[a]);
                        Match matLparName = regLparName.Match(linestragain);
                        if (matLparName.Success)
                        {
                            i++;
                            this.textBoxRows.Text = Convert.ToString(i);
                            lineTempStr = lineTempStr + linestragain + "\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return lineTempStr;
        }

        /*
         * 生成DASD 使用分析数据
         */
        public void RegexDASDRate()
        {
            String tempPr = null, fileLine = null, lparName = null, timePoint = "", dataSortStr = "", fileName = fileTextBox.Text;
            int i = 0;
            /*
            fileProcess fp = new fileProcess();
            StringBuilder tmpstr = fp.GetFileContent(fileName);
            fileText.Text = tmpstr.ToString();
            string content = string.Empty;
            StreamReader srt = new StreamReader(fileName) ;
            content = srt.ReadToEnd();//一次性读入内存              
            MemoryStream ms = new MemoryStream(Encoding.GetEncoding("GB2312").GetBytes(content));//放入内存流，以便逐行读取
            long line = 0;
            StreamReader srnew = new StreamReader(ms);
            while (srnew.Peek() > -1)
                {
                    this.fileText.AppendText(srnew.ReadLine() + "\r\n");
                    Application.DoEvents();
                }
             */
            try
            {
                StreamReader sr = File.OpenText(fileName);
                bool varTemp = false;
                Thread.Sleep(1000);
                if (this.dasdFileLoadVar.Text == "是")
                {
                    TruncateTable("prdasd");
                }

                //string pattern = @"\b(?<ServiceGroup>\S+)\b(?<DevNum>\S+)\b";
                ////Regex rex = new Regex(pattern, RegexOptions.IgnoreCase); 
                //MatchCollection mc = Regex.Matches(sr.ReadToEnd(),pattern);
                //foreach (Match match in mc)
                //{
                //    GroupCollection gc = match.Groups;
                //    string tp = string.Format(gc["ServiceGroup"].Value, gc["DevNum"].Value); 
                //}
                while ((fileLine = sr.ReadLine()) != null)
                {
                    Regex regDADA = new Regex("D I R E C T   A C C E S S   D E V I C E   A C T I V I T Y");//匹配DIRECT ACCESS DEVICE ACTIVITY的数据
                    Match matDADA = regDADA.Match(fileLine);
                    this.label5.Text = "当前分析行数";
                    i++;
                    this.textBoxRows.Text = Convert.ToString(i);
                    if (matDADA.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                        varTemp = false;
                    }
                    Regex regSystemID = new Regex("SYSTEM ID");//匹配SYSTEM ID的数据
                    Match matSystemID = regSystemID.Match(fileLine);
                    if (matSystemID.Success && !varTemp)
                    {
                        int lineNum = Convert.ToInt16(matSystemID.Index.ToString());
                        lparName = fileLine.Substring(lineNum + 9, 10).Trim();
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regRMFTime = new Regex("RMF       TIME");//匹配RMF TIME的数据
                    Match matRMFTime = regRMFTime.Match(fileLine);
                    if (matRMFTime.Success && !varTemp)
                    {
                        int lineNum = Convert.ToInt16(matRMFTime.Index.ToString());
                        timePoint = fileLine.Substring(lineNum + 15, 8);
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regStorageDev = new Regex("  STORAGE  DEV");//匹配STORAGE  DEV的数据
                    Match matStorageDev = regStorageDev.Match(fileLine);
                    if (matStorageDev.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regDeviceType = new Regex(deviceType.Text);//匹配33909磁盘类型数据
                    Match matDeviceType = regDeviceType.Match(fileLine);
                    if (matDeviceType.Success)
                    {
                        tempPr = tempPr + lparName + "  " + timePoint + " " + fileLine + "\r\n";
                        if (this.dasdFileLoadVar.Text == "是")
                        {
                            string storageGroup = fileLine.Substring(1, 8).Trim();
                            string vomuleSerial = fileLine.Substring(25, 6).Trim();
                            double deviceActivityRate = Math.Round(Convert.ToDouble(fileLine.Substring(41, 8).Trim()), 3);
                            double avgRespTime = Math.Round(Convert.ToDouble(fileLine.Substring(50, 5).Trim()), 3);
                            //bool insertMdb = InsertMDB("prtool.accdb", "prdasd", lparName, timePoint, storageGroup, vomuleSerial, deviceActivityRate, avgRespTime);
                            InsertOracle("prdasd", lparName, timePoint, storageGroup, vomuleSerial, deviceActivityRate, avgRespTime);
                        }
                    }
                }
                if (this.dasdFileLoadVar.Text == "否")
                {
                    fileText.Text = tempPr;//窗口显示结果数据
                }
                else
                {
                    MessageBox.Show("DASD数据已Load到数据库");
                }
                if (this.sortVar.Text == "是")
                {
                    dataSortStr = RegexDASDDataSort(tempPr);
                    fileText.Text = dataSortStr;
                }
                if (this.fileText.Text != null && this.fileText.Text != "")
                {
                    MessageBox.Show("DASD执行分析已完成");
                }
                this.EndPublicPropertiesGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            //return tempPr;//返回结果数据
        }

        /*
         * Dasd数据插入Access方法
         */
        public static bool InsertMDB(string mdbPath, string tableName, string lparName, string timePoint, string serviceGroup, string volumeSerial,
                                             string deviceActivityRate, string avgRespTime)
        {
            try
            {
                //1、建立连接    
                string strConn
                    = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + "";
                OleDbConnection odcConnection = new OleDbConnection(strConn);
                //2、打开连接    
                odcConnection.Open();

                string sql = @"insert into " + tableName + "(lparName,timePoint,serviceGroup,volumeSerial,deviceActivityRate,avgRespTime) values" +
                                 "('" + lparName + "','" + timePoint + "','" + serviceGroup + "','" + volumeSerial + "'," + deviceActivityRate + "," + avgRespTime + ")";
                OleDbCommand comm = new OleDbCommand(sql, odcConnection);
                comm.ExecuteNonQuery();
                odcConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /*
         * Dasd数据插入Oracle方法
         */
        public static void InsertOracle(string tableName, string lparName, string timePoint, string serviceGroup, string volumeSerial,
                                     double deviceActivityRate, double avgRespTime)
        {
            try
            {
                //读取app.config连接字符串
                XmlDocument doc = new XmlDocument();
                doc.Load("app.config");
                XmlNode root = doc.SelectSingleNode("//configuration");
                XmlNode node = root.SelectSingleNode("//appSettings/add[@name='ConnectionString']");
                XmlElement el = node as XmlElement;
                string connStr = el.GetAttribute("connectionString");
                OracleConnection conn = new OracleConnection(connStr);
                conn.Open();
                //执行SQL
                OracleCommand insertSql = conn.CreateCommand();
                insertSql.CommandText = "insert into " + tableName + "(lparName,timePoint,serviceGroup,volumeSerial,deviceActivityRate,avgRespTime) values" +
                                 "('" + lparName + "','" + timePoint + "','" + serviceGroup + "','" + volumeSerial + "'," + deviceActivityRate + "," + avgRespTime + ")";
                insertSql.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*
         * Truncate某个表方法
         */
        public static void TruncateTable(string tableName)
        {
            try
            {
                //读取app.config连接字符串
                XmlDocument doc = new XmlDocument();
                doc.Load("app.config");
                XmlNode root = doc.SelectSingleNode("//configuration");
                XmlNode node = root.SelectSingleNode("//appSettings/add[@name='ConnectionString']");
                XmlElement el = node as XmlElement;
                string connStr = el.GetAttribute("connectionString");
                OracleConnection conn = new OracleConnection(connStr);
                conn.Open();
                //执行SQL
                OracleCommand insertSql = conn.CreateCommand();
                insertSql.CommandText = "truncate table " + tableName + "";
                insertSql.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PrepareDataTable()
        {
            try
            {
                string tableName = "prdasd", fileName = "C:\\DASDAnalysis.xlsx", tempValue = "";
                int i = 0;
                MSExcel.ApplicationClass excelApp = new MSExcel.ApplicationClass();
                excelApp.Visible = false;
                object objMissing = System.Reflection.Missing.Value;
                MSExcel.Workbook excelDoc = (MSExcel.Workbook)excelApp.Workbooks.Add(1);

                //读取app.config连接字符串
                XmlDocument doc = new XmlDocument();
                doc.Load("app.config");
                XmlNode root = doc.SelectSingleNode("//configuration");
                XmlNode node = root.SelectSingleNode("//appSettings/add[@name='ConnectionString']");
                XmlElement el = node as XmlElement;
                string connStr = el.GetAttribute("connectionString");
                OracleConnection conn = new OracleConnection(connStr);
                conn.Open();
                //执行SQL
                OracleCommand lparNameSql = conn.CreateCommand();
                lparNameSql.CommandText = "select lparname from  " + tableName + " group by lparname order by lparname desc";
                OracleDataReader lparNameReader = lparNameSql.ExecuteReader();
                while (lparNameReader.Read())
                {
                    string lparName = Convert.ToString(lparNameReader.GetOracleString(0));
                    MSExcel.Worksheet excelSheet = (MSExcel.Worksheet)excelDoc.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = lparName;
                    excelSheet.Cells[1, 1] = "LPAR Name";
                    excelSheet.Cells[1, 2] = "Time";
                    excelSheet.Cells[1, 3] = "Storage Group";
                    excelSheet.Cells[1, 4] = "Volume Serial";
                    excelSheet.Cells[1, 5] = "Device Activity Rate";
                    excelSheet.Cells[1, 6] = "AVG Resp Time";
                    if (this.dasdTopOutputVar.Text == "否")
                    {
                        OracleCommand dataSql = conn.CreateCommand();
                        dataSql.CommandText = "select * from " + tableName + " where lparname = " + "'" + lparName + "' order by lparname,timepoint";
                        OracleDataReader dataReader = dataSql.ExecuteReader();
                        OracleCommand dataCount = conn.CreateCommand();
                        dataCount.CommandText = "select count(*) from " + tableName + " where lparname = " + "'" + lparName + "'";
                        int rowCount = Convert.ToInt16(dataCount.ExecuteScalar());
                        for (int a = 0; a < rowCount; a++)
                        {
                            i++;
                            this.textBoxRows.Text = Convert.ToString(i);
                            dataReader.Read();
                            for (int b = 0; b < dataReader.FieldCount; b++)
                            {
                                if (dataReader.GetOracleValue(b).ToString().Trim() != "Null")
                                {
                                    tempValue = Convert.ToString(dataReader.GetOracleValue(b)).Trim();
                                }
                                else
                                {
                                    tempValue = "";
                                }
                                excelSheet.Cells[a + 2, b + 1] = tempValue;
                            }
                        }
                        dataReader.Close();
                        excelSheet.UsedRange.EntireColumn.AutoFit();
                    }
                    else if (this.dasdTopOutputVar.Text == "是")
                    {
                        //Top DeviceActivityRate输出
                        OracleCommand dataSql = conn.CreateCommand();
                        dataSql.CommandText = "select * from " + tableName + " where lparname = " + "'" + lparName + "' order by deviceactivityrate desc";
                        OracleDataReader dataReader = dataSql.ExecuteReader();
                        for (int a = 0; a < Convert.ToInt16(daseTopOutputLine.Text); a++)
                        {
                            i++;
                            this.textBoxRows.Text = Convert.ToString(i);
                            dataReader.Read();
                            for (int b = 0; b < dataReader.FieldCount; b++)
                            {
                                if (dataReader.GetOracleValue(b).ToString().Trim() != "Null")
                                {
                                    tempValue = Convert.ToString(dataReader.GetOracleValue(b)).Trim();
                                }
                                else
                                {
                                    tempValue = "";
                                }
                                excelSheet.Cells[a + 2, b + 1] = tempValue;
                            }
                        }
                        dataReader.Close();
                        //Top DeviceActivityRate输出
                        excelSheet.Cells[1, 7] = "LPAR Name";
                        excelSheet.Cells[1, 8] = "Time";
                        excelSheet.Cells[1, 9] = "Storage Group";
                        excelSheet.Cells[1, 10] = "Volume Serial";
                        excelSheet.Cells[1, 11] = "AVG Resp Time";
                        excelSheet.Cells[1, 12] = "Device Activity Rate";
                        OracleCommand dataRespSql = conn.CreateCommand();
                        dataRespSql.CommandText = "select lparname,timepoint,servicegroup,volumeserial,avgresptime,deviceactivityrate from " + tableName +
                                                          " where lparname = " + "'" + lparName + "' order by avgresptime desc";
                        OracleDataReader dataRespReader = dataRespSql.ExecuteReader();
                        for (int a = 0; a < Convert.ToInt16(daseTopOutputLine.Text); a++)
                        {
                            i++;
                            this.textBoxRows.Text = Convert.ToString(i);
                            dataRespReader.Read();
                            for (int b = 0; b < dataRespReader.FieldCount; b++)
                            {
                                if (dataRespReader.GetOracleValue(b).ToString().Trim() != "Null")
                                {
                                    tempValue = Convert.ToString(dataRespReader.GetOracleValue(b)).Trim();
                                }
                                else
                                {
                                    tempValue = "";
                                }
                                excelSheet.Cells[a + 2, b + 7] = tempValue;
                            }
                        }
                        dataRespReader.Close();
                        excelSheet.UsedRange.EntireColumn.AutoFit();
                    }
                }
                lparNameReader.Close();
                conn.Close();

                excelDoc.SaveAs("" + fileName + "", objMissing, objMissing, objMissing,
                objMissing, objMissing, MSExcel.XlSaveAsAccessMode.xlNoChange,
                objMissing, objMissing, objMissing,
                objMissing, objMissing);
                excelDoc = null;
                excelApp.Quit();
                excelApp = null;
                MessageBox.Show("C:\\DASDAnalysis.xlsx CPU分析文件已生成");
                this.EndPublicPropertiesGroup();
                fileText.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
        }

        /*
         * 对结果数据按Lpar先后顺序排列数据
         */
        public string RegexDASDDataSort(string inTempstr)
        {
            string tempStr = "", lparName = "", tempLparName = "", lpArrStr = "", lineTempStr = "";
            bool lparNameVar = true;
            int i = 0;
            try
            {
                this.label5.Text = "当前排序行数";
                foreach (string linestr in fileText.Lines)
                {
                    Regex regDeviceType = new Regex(deviceType.Text);
                    Match matDeviceType = regDeviceType.Match(linestr);
                    if (matDeviceType.Success)
                    {
                        lparName = linestr.Substring(0, 6).Trim();
                        if (tempLparName != "" && tempLparName != lparName)
                        {
                            tempLparName = lparName;
                            if (!lpArrStr.Contains(lparName))
                            {
                                lpArrStr = lpArrStr + "," + lparName;
                            }
                        }
                        else if (tempLparName != lparName)
                        {
                            if (lparNameVar)
                            {
                                lpArrStr = lpArrStr + "," + lparName;
                                lparNameVar = false;
                            }
                            tempLparName = lparName;
                        }
                        tempStr = tempStr + linestr + "\r\n";
                    }
                }
                fileText.Text = tempStr;
                string[] lparNameArray = LparNameArray(lpArrStr.Substring(1));
                for (int a = 0; a < lparNameArray.Length; a++)
                {
                    foreach (string linestragain in fileText.Lines)
                    {
                        Regex regLparName = new Regex(@"^" + lparNameArray[a]);
                        Match matLparName = regLparName.Match(linestragain);
                        if (matLparName.Success)
                        {
                            i++;
                            this.textBoxRows.Text = Convert.ToString(i);
                            lineTempStr = lineTempStr + linestragain + "\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return lineTempStr;
        }

        /*
         * 生成TTRN交易分析数据
         */
        public void RegexTTRNRate()
        {
            String tempPr = null, fileLine = null, fileName = fileTextBox.Text;
            bool varTemp = false;
            int i = 0;
            try
            {
                StreamReader sr = File.OpenText(fileName);
                while ((fileLine = sr.ReadLine()) != null)
                {
                    Regex regAvg = new Regex("                                 Avg");//从匹配Avg开始取后续所有数据
                    Match matAvg = regAvg.Match(fileLine);
                    if (matAvg.Success)
                    {
                        //tempPr = tempPr + fileLine + "\r\n";
                        varTemp = true;
                    }
                    else if (varTemp && fileLine.Trim() != "" && fileLine.Length > 4)//去掉空行数据
                    {
                        i++;
                        this.textBoxRows.Text = Convert.ToString(i);
                        Regex regStartMatch = new Regex(@"^\s[A-Z][A-Z][A-Z]\w");
                        Match matStartMatch = regStartMatch.Match(fileLine);
                        if (matStartMatch.Success)
                        {
                            tempPr = tempPr + fileLine + "\r\n";
                        }
                        Regex regTotal = new Regex(" Total");
                        Match matTotal = regTotal.Match(fileLine);
                        if (matTotal.Success)
                        {
                            tempPr = tempPr + fileLine + "\r\n";
                        }
                        Regex regSpaceAvg = new Regex("                                 Avg");
                        Match matSpaceAvg = regSpaceAvg.Match(fileLine);
                        if (matSpaceAvg.Success)
                        {
                            tempPr = tempPr + fileLine + "\r\n";
                        }
                    }
                }
                fileText.Text = tempPr;//窗口显示结果数据
                if (this.fileText.Text != null && this.fileText.Text != "")
                {
                    MessageBox.Show("交易量执行分析已完成");
                }
                this.EndPublicPropertiesGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            //return tempPr;//返回结果数据
        }

        /*
         * 生成TRNR笔数分析数据
         */
        public void RegexTRNRRate()
        {
            String tempPr = null, fileLine = null, fileName = fileTextBox.Text;
            int i = 0;
            try
            {
                StreamReader sr = File.OpenText(fileName);
                while ((fileLine = sr.ReadLine()) != null)
                {
                    i++;
                    this.textBoxRows.Text = Convert.ToString(i);
                    Regex regTimeOne = new Regex(@"^\s[0-9][0-9]:[0-9][0-9]:[0-9][0-9]");//匹配从空格开头+时间格式的数据
                    Match matTimeOne = regTimeOne.Match(fileLine);
                    if (matTimeOne.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regTimeTwo = new Regex(@"^\s+[0-9]:[0-9][0-9]:[0-9][0-9]");//匹配从空格开头+时间格式的数据
                    Match matTimeTwo = regTimeTwo.Match(fileLine);
                    if (matTimeTwo.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                    Regex regTimeThree = new Regex(@"[0]\s[0-9]:[0-9][0-9]:[0-9][0-9]");//匹配从空格开头+时间格式的数据
                    Match matTimeThree = regTimeThree.Match(fileLine);
                    if (matTimeThree.Success)
                    {
                        tempPr = tempPr + fileLine + "\r\n";
                    }
                }
                fileText.Text = tempPr;//窗口显示结果数据
                if (this.fileText.Text != null && this.fileText.Text != "")
                {
                    MessageBox.Show("Transaction Rate执行分析已完成");
                }
                this.EndPublicPropertiesGroup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            //return tempPr;//返回结果数据
        }

        /*
         * 数据转成逗号分隔形式的方法
         */
        public string dataToCsv(String tempStr)
        {
            string resultStr = "", tempPr = null;
            try
            {
                foreach (string linestr in fileText.Lines)
                {
                    Regex r = new Regex(@"\s+");
                    resultStr = r.Replace(linestr.Trim(), " ");
                    resultStr.Trim();
                    resultStr = resultStr.Replace(" ", ",");
                    tempPr = tempPr + resultStr + "\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tempPr;
        }


        /*
         * 计算字符串在数据内容个数
         */
        public int StrCount(String tempStr, String strVar)
        {
            int count = 0;
            try
            {
                Regex regStrVar = new Regex(strVar);
                MatchCollection matStrVar = regStrVar.Matches(tempStr, 0);
                count = matStrVar.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return count;
        }

        /*
         * 返回指定字符匹配的数据
         */
        public string TempStr(String tempStr, String strVar)
        {
            String tempPr = null;
            try
            {
                foreach (string linestr in fileText.Lines)
                {
                    Regex regStrVar = new Regex(strVar);
                    Match matStrVar = regStrVar.Match(linestr);
                    if (matStrVar.Success)
                    {
                        tempPr = tempPr + linestr + "\r\n";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tempPr;
        }

        /*
         *逗号分隔符数据转成数组方法 
         */
        public string[] LparNameArray(String tempStr)
        {
            string[] inArray = null;
            try
            {
                inArray = tempStr.Split(',');
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return inArray;
        }

        /*
         * Excel列数字转成字母
         */
        public string ToName(int index)
        {
            if (index <= 0)
                throw new Exception("无效的输入数字");

            index--;
            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0)
                    index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);

            return String.Join(string.Empty, chars.ToArray());
        }

        /*
         *处理需要生成的CPU分析数据，保存数据到Excel工作表 
         */
        public void ResultCPUToExcel()
        {
            try
            {
                string tempLparName = "", lparName = "", busyStr = "", tempText = "", fileName = "C:\\CPUAnalysis.xlsx";
                int row = 2, i = 2, j = 2, k = 0, l = 0, m = 0, n = 0, l1 = 1, p1 = 0, p3 = 2, rowCpu2 = 0, colCpu2 = 0, rowPoint = 0;
                MSExcel.ApplicationClass excelApp = new MSExcel.ApplicationClass();
                excelApp.Visible = false;
                object objMissing = System.Reflection.Missing.Value;
                MSExcel.Workbook excelDoc = (MSExcel.Workbook)excelApp.Workbooks.Add(1);

                //CPU使用走势（根据单位时间内不同LPAR的平均CPU使用率数据，绘制折线图体现各LPAR的CPU使用走势。）
                ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = "CPU使用走势";
                MSExcel.Worksheet excelSheet = (MSExcel.Worksheet)excelDoc.Worksheets[1];

                //⒈ 需根据原始数据提取多个lpar的CPU使用率数据，以lpar为单位绘制 LPAR busy/MVS busy 比值结果的折线图。
                MSExcel.Worksheet excelSheet1 = (MSExcel.Worksheet)excelDoc.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                excelSheet.Cells[1, 1] = "根据单位时间内不同LPAR的平均CPU使用率数据，绘制折线图体现各LPAR的CPU使用走势。";
                excelSheet.Cells[row, 1] = "";

                excelSheet1.Cells[1, 1] = "根据原始数据提取多个lpar的CPU使用率数据，以LPAR为单位绘制 LPAR busy/MVS busy 比值结果的折线图。";
                ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = "LPAR Busy";
                excelSheet1.Cells[2, 2] = "LPAR Busy";
                excelSheet1.Cells[2, 3] = "MVS Busy";
                excelSheet1.Cells[2, 4] = "LPAR Busy/MVS Busy";

                //分段分析匹配数据
                tempText = fileText.Text;
                busyStr = TempStr(tempText, " TOTAL/AVERAGE");
                fileText.Text = busyStr;
                foreach (string linestr in fileText.Lines)
                {
                    rowPoint++;
                    textBoxRows.Text = Convert.ToString(rowPoint);
                    Regex regTotalAverageOne = new Regex(" TOTAL/AVERAGE");
                    Match matTotalAverageOne = regTotalAverageOne.Match(linestr);
                    if (matTotalAverageOne.Success)
                    {
                        lparName = linestr.Substring(0, 6).Trim();
                        if (tempLparName != "" && tempLparName != lparName)
                        {
                            i++;
                            colCpu2 = i;
                            tempLparName = lparName;
                            excelSheet.Cells[2, i] = tempLparName;
                        }
                        else
                        {
                            excelSheet.Cells[2, i] = lparName;
                            tempLparName = lparName;
                        }
                    }
                    Regex regTotalAverageTwo = new Regex(" TOTAL/AVERAGE");
                    Match matTotalAverageTwo = regTotalAverageTwo.Match(linestr);
                    if (matTotalAverageTwo.Success)
                    {
                        string timeName = linestr.Substring(6, 8).Trim();
                        if (timeName != null)
                        {
                            j++;
                            rowCpu2 = j;
                            excelSheet.Cells[j, 1] = timeName;
                        }
                    }
                    Regex regTotalAverageThree = new Regex(" TOTAL/AVERAGE");
                    Match matTotalAverageThree = regTotalAverageThree.Match(linestr);
                    if (matTotalAverageThree.Success)
                    {
                        int lineNum = Convert.ToInt16(matTotalAverageThree.Index.ToString());
                        string lparBusy = linestr.Substring(lineNum + 23, 8).Trim();
                        if (lparBusy != null)
                        {
                            excelSheet.Cells[j, i] = lparBusy;
                            k++;
                        }
                        if (Convert.ToInt16(StrCount(fileText.Text, lparName)) == k)
                        {
                            j = 2;
                            k = 0;
                        }
                    }
                }

                //写入LPAR Busy/MVS Busy
                fileText.Text = tempText;
                busyStr = TempStr(tempText, "CP     100.00");
                fileText.Text = busyStr;
                //int lpNameCount = StrCount(busyStr, lparName);
                foreach (string lprate in fileText.Lines)
                {
                    Regex regCPMatch = new Regex("CP     100.00");
                    Match matCPMatch = regCPMatch.Match(lprate);
                    if (matCPMatch.Success)
                    {
                        rowPoint++;
                        textBoxRows.Text = Convert.ToString(rowPoint);
                        int lineNum = Convert.ToInt16(matCPMatch.Index.ToString());
                        //lparCount = StrCount(busyStr, lparName);
                        string lparLineName = lprate.Substring(0, 6).Trim();
                        double lparBusy = Convert.ToDouble(lprate.Substring(lineNum + 15, 10).Trim());
                        double mvsBusy = Convert.ToDouble(lprate.Substring(lineNum + 25, 10).Trim());
                        if (!lprate.Contains(tempLparName))
                        {
                            excelSheet1.Cells[2, p1 = p1 + 2] = "LPAR Busy";
                            excelSheet1.Cells[2, p1 = p1 + 1] = "MVS Busy";
                            excelSheet1.Cells[2, p1 = p1 + 1] = "LPAR Busy/MVS Busy";
                            m = p1;
                            l = p1;
                            p3 = 2;
                            l1 = p1 - 3;
                        }
                        if (lprate.Contains(lparLineName))
                        {
                            p3++;
                            excelSheet1.Cells[p3, l1] = lparLineName;
                            excelSheet1.Cells[p3, l1 + 1] = lparBusy;
                            excelSheet1.Cells[p3, l1 + 2] = mvsBusy;
                            excelSheet1.Cells[p3, l1 + 3] = Math.Round(Convert.ToDouble(lparBusy) / Convert.ToDouble(mvsBusy), 2);
                            n = p3;
                        }
                        tempLparName = lparLineName;
                    }
                }

                for (int o = 1; o <= m / 4; o++)
                {
                    l++;
                    for (int p = 2; p <= n; p++)
                    {
                        if (p == 2)
                        {
                            excelSheet1.Cells[p, l] = excelSheet1.Cells[p + 1, o * 4 - 3];
                        }
                        else
                        {
                            excelSheet1.Cells[p, l] = excelSheet1.Cells[p, o * 4];
                        }
                    }
                }

                CreateChart(excelDoc, excelSheet1, ToName(m + 1) + 2, ToName(m + m / 4) + n, "CPU1");
                CreateChart(excelDoc, excelSheet, "A2", ToName(colCpu2) + Convert.ToString(rowCpu2), "CPU2");

                excelDoc.SaveAs("" + fileName + "", objMissing, objMissing, objMissing,
                objMissing, objMissing, MSExcel.XlSaveAsAccessMode.xlNoChange,
                objMissing, objMissing, objMissing,
                objMissing, objMissing);
                excelDoc = null;
                excelApp.Quit();
                excelApp = null;
                MessageBox.Show("C:\\CPUAnalysis.xlsx CPU分析文件已生成");
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
        }

        /*
         *处理需要生成的DASD分析数据，保存数据到Excel工作表 
         */
        public void ResultDASDToExcel()
        {
            try
            {
                string tempLparName = "", lparName = "", fileName = "C:\\DASDAnalysis.xlsx";
                int l1 = 1, p1 = 0, p3 = 2, rowPoint = 0;
                MSExcel.ApplicationClass excelApp = new MSExcel.ApplicationClass();
                excelApp.Visible = false;
                object objMissing = System.Reflection.Missing.Value;
                MSExcel.Workbook excelDoc = (MSExcel.Workbook)excelApp.Workbooks.Add(1);

                //用表格记录测试期间主机DASD设备响应的相关信息。
                ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = "DASD响应时间";
                MSExcel.Worksheet excelSheet = (MSExcel.Worksheet)excelDoc.Worksheets[1];

                excelSheet.Cells[1, 1] = "用表格记录测试期间主机DASD设备响应的相关信息。";
                excelSheet.Cells[2, 1] = "LPAR Name";
                excelSheet.Cells[2, 2] = "Time";
                excelSheet.Cells[2, 3] = "Storage Group";
                excelSheet.Cells[2, 4] = "Volume Serial";
                excelSheet.Cells[2, 5] = "Device Activity Rate";
                excelSheet.Cells[2, 6] = "AVG Resp Time";

                foreach (string linestr in fileText.Lines)
                {
                    //写入DASD相关信息到Excel
                    Regex regDeviceType = new Regex(deviceType.Text);
                    Match matDeviceType = regDeviceType.Match(linestr);
                    if (matDeviceType.Success)
                    {
                        rowPoint++;
                        textBoxRows.Text = Convert.ToString(rowPoint);
                        int lineNum = Convert.ToInt16(matDeviceType.Index.ToString());
                        string lparLineName = linestr.Substring(0, 6).Trim();
                        string timePoint = linestr.Substring(6, 8);
                        string storageGroup = linestr.Substring(16, 8).Trim();
                        string vomuleSerial = linestr.Substring(lineNum + 9, 6).Trim();
                        string deviceActivityRate = linestr.Substring(lineNum + 26, 8).Trim();
                        string avgRespTime = linestr.Substring(lineNum + 34, 5).Trim();
                        lparName = lparLineName;
                        if (lparName != tempLparName)
                        {
                            excelSheet.Cells[2, p1 = p1 + 1] = "LPAR Name";
                            excelSheet.Cells[2, p1 = p1 + 1] = "Time";
                            excelSheet.Cells[2, p1 = p1 + 1] = "Storage Group";
                            excelSheet.Cells[2, p1 = p1 + 1] = "Volume Serial";
                            excelSheet.Cells[2, p1 = p1 + 1] = "Device Activity Rate";
                            excelSheet.Cells[2, p1 = p1 + 1] = "AVG Resp Time";
                            p3 = 2;
                            l1 = p1 - 5;
                        }
                        if (lparName == lparLineName)
                        {
                            p3++;
                            excelSheet.Cells[p3, l1] = lparLineName;
                            excelSheet.Cells[p3, l1 + 1] = timePoint;
                            excelSheet.Cells[p3, l1 + 2] = storageGroup;
                            excelSheet.Cells[p3, l1 + 3] = vomuleSerial;
                            excelSheet.Cells[p3, l1 + 4] = deviceActivityRate;
                            excelSheet.Cells[p3, l1 + 5] = avgRespTime;
                        }
                        tempLparName = lparLineName;
                    }
                }

                //CreateChart(excelDoc, excelSheet, 0, "CPU1");

                excelDoc.SaveAs("" + fileName + "", objMissing, objMissing, objMissing,
                objMissing, objMissing, MSExcel.XlSaveAsAccessMode.xlNoChange,
                objMissing, objMissing, objMissing,
                objMissing, objMissing);
                excelDoc = null;
                excelApp.Quit();
                excelApp = null;
                MessageBox.Show("C:\\DASDAnalysis.xlsx DASD响应时间分析文件已生成");
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
        }

        /*
         *处理需要生成的TTRN分析数据，保存数据到Excel工作表 
         */
        public void ResultTTRNToExcel()
        {
            try
            {
                string trnChnnSum = "", trnChnnName = "", tempTrnChnnName = "", fileName = "C:\\TTRNAnalysis.xlsx"; ;
                int j = 2, tempChnnSum = 0, k = 2, colTtrn2 = 0, colTtrn1 = 0, rowPoint = 0;
                MSExcel.ApplicationClass excelApp = new MSExcel.ApplicationClass();
                excelApp.Visible = false;
                object objMissing = System.Reflection.Missing.Value;
                MSExcel.Workbook excelDoc = (MSExcel.Workbook)excelApp.Workbooks.Add(1);

                //根据交易名称和笔数建立柱状分析图。能够看出每种和每组交易量与总交易量的关系。
                ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = "总交易笔数与分类交易笔数";
                MSExcel.Worksheet excelSheet = (MSExcel.Worksheet)excelDoc.Worksheets[1];

                excelSheet.Cells[1, 1] = "根据交易名称和笔数建立柱状分析图。能够看出每种和每组交易量与总交易量的关系。";
                excelSheet.Cells[2, 1] = "交易名称";
                excelSheet.Cells[3, 1] = "总交易量（笔数）";

                excelSheet.Cells[4, 1] = "交易名称";
                excelSheet.Cells[5, 1] = "分类交易量（笔数）";

                foreach (string linestr in fileText.Lines)
                {
                    Regex reg = new Regex("           ");
                    Match mat = reg.Match(linestr);
                    if (mat.Success)
                    {
                        rowPoint++;
                        textBoxRows.Text = Convert.ToString(rowPoint);
                        int lineNum = Convert.ToInt16(mat.Index.ToString());
                        trnChnnName = linestr.Substring(1, 6).Trim();
                        if ("Total" != trnChnnName && trnChnnName.Trim() != "")
                        {
                            j++;
                            colTtrn1 = j;
                            trnChnnSum = linestr.Substring(18, 9).Trim();
                            excelSheet.Cells[2, j] = trnChnnName;
                            excelSheet.Cells[3, j] = trnChnnSum;
                            if (j == 3)
                            {
                                tempTrnChnnName = trnChnnName;
                            }
                            if (trnChnnName.Length > 0 && trnChnnName.Substring(0, 1) == tempTrnChnnName.Substring(0, 1))
                            {
                                tempChnnSum = tempChnnSum + Convert.ToInt32(trnChnnSum);
                            }
                            else
                            {
                                k++;
                                excelSheet.Cells[4, k] = tempTrnChnnName.Substring(0, 1) + "*";
                                excelSheet.Cells[5, k] = tempChnnSum;
                                tempChnnSum = 0;
                                tempChnnSum = tempChnnSum + Convert.ToInt32(trnChnnSum);
                            }
                            tempTrnChnnName = trnChnnName;
                        }
                        else if ("Total" == trnChnnName)
                        {
                            k++;
                            colTtrn2 = k;
                            excelSheet.Cells[4, k] = tempTrnChnnName.Substring(0, 1) + "*";
                            excelSheet.Cells[5, k] = tempChnnSum;
                            tempChnnSum = 0;
                            tempChnnSum = tempChnnSum + Convert.ToInt32(trnChnnSum);

                            trnChnnSum = linestr.Substring(18, 9).Trim();
                            excelSheet.Cells[2, 2] = trnChnnName;
                            excelSheet.Cells[3, 2] = Convert.ToInt32(trnChnnSum);
                            excelSheet.Cells[4, 2] = trnChnnName;
                            excelSheet.Cells[5, 2] = Convert.ToInt32(trnChnnSum);
                        }
                    }
                }

                CreateChart(excelDoc, excelSheet, "B2", Convert.ToString(ToName(colTtrn1)) + "3", "TTRN1");

                CreateChart(excelDoc, excelSheet, "B4", Convert.ToString(ToName(colTtrn2)) + "5", "TTRN2");

                excelDoc.SaveAs("" + fileName + "", objMissing, objMissing, objMissing,
                objMissing, objMissing, MSExcel.XlSaveAsAccessMode.xlNoChange,
                objMissing, objMissing, objMissing,
                objMissing, objMissing);
                excelDoc = null;
                excelApp.Quit();
                excelApp = null;
                MessageBox.Show("C:\\TTRNAnalysis.xlsx 交易量分析文件已生成");
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
        }


        /*
         *处理需要生成的TRNR分析数据，保存数据到Excel工作表 
         */
        public void ResultTRNRToExcel()
        {
            try
            {
                string shiftTime = "", trnr = "", fileName = "C:\\TRNRAnalysis.xlsx"; ;
                int row = 1, i = 0, j = 2, m = 2, n = 0, rowTrnr1 = 0, rowTrnr2 = 0, avgsum = 0, rowPoint = 0;
                MSExcel.ApplicationClass excelApp = new MSExcel.ApplicationClass();
                excelApp.Visible = false;
                object objMissing = System.Reflection.Missing.Value;
                MSExcel.Workbook excelDoc = (MSExcel.Workbook)excelApp.Workbooks.Add(1);

                //根据系统的transaction rate（每时间交易量）记录，绘制出折现图表示没时间交易量的区间。
                ((MSExcel.Worksheet)excelDoc.Worksheets[1]).Name = "Transaction Rate";
                MSExcel.Worksheet excelSheet = (MSExcel.Worksheet)excelDoc.Worksheets[1];

                excelSheet.Cells[1, 1] = "根据系统的transaction rate（每时间交易量）记录，绘制出折现图表示没有时间交易量的区间及交易随着时间处理情况。";
                excelSheet.Cells[2, 1] = "交易时间";
                excelSheet.Cells[2, 2] = "Transaction Rate";
                excelSheet.Cells[2, 3] = "交易时间";
                excelSheet.Cells[2, 4] = "Transaction Rate";

                foreach (string linestr in fileText.Lines)
                {
                    i++;
                    if (i > row)
                    {
                        if (linestr.Length > 0)
                        {
                            rowPoint++;
                            textBoxRows.Text = Convert.ToString(rowPoint);
                            shiftTime = linestr.Substring(1, 8).Trim();
                            trnr = linestr.Substring(70, 9).Trim();
                            if (trnr.Length > 0)
                            {
                                j++;
                                n++;
                                rowTrnr1 = j;
                                excelSheet.Cells[j, 1] = shiftTime;
                                excelSheet.Cells[j, 2] = Convert.ToInt32(trnr);
                                avgsum = avgsum + Convert.ToInt32(trnr);
                            }
                            if ((trnr.Length > 0 && n % Convert.ToInt32(trNum.Text) == 0))
                            {
                                m++;
                                rowTrnr2 = m;
                                excelSheet.Cells[m, 3] = shiftTime;
                                excelSheet.Cells[m, 4] = Math.Round(Convert.ToDouble(avgsum / Convert.ToInt32(trNum.Text)), 0);
                                avgsum = 0;
                            }
                        }
                    }
                }

                CreateChart(excelDoc, excelSheet, "A2", "B" + Convert.ToString(rowTrnr1), "TRNR1");
                CreateChart(excelDoc, excelSheet, "C2", "D" + Convert.ToString(rowTrnr2), "TRNR2");

                excelDoc.SaveAs("" + fileName + "", objMissing, objMissing, objMissing,
                objMissing, objMissing, MSExcel.XlSaveAsAccessMode.xlNoChange,
                objMissing, objMissing, objMissing,
                objMissing, objMissing);
                excelDoc = null;
                excelApp.Quit();
                excelApp = null;
                MessageBox.Show("C:\\TRNRAnalysis.xlsx Transaction Rate分析文件已生成");
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
                fileText.Text = "";
            }
        }

        /*
         *通过分析类型控制生成Excel结果文件 
         */
        private void createExcel_Click(object sender, EventArgs e)
        {
            try
            {
                String caseVar = "";
                if ((fileText.Text == null || fileText.Text == "") && dasdFileLoadVar.Text != "是")
                {
                    MessageBox.Show("请先执行分析");
                }
                else if (typeComboBox.Text == null || typeComboBox.Text == "")
                {
                    MessageBox.Show("请先选择分析类型");
                }
                else
                {
                    caseVar = typeComboBox.Text.Substring(2, 3);
                }
                switch (caseVar)
                {
                    case "CPU":
                        {
                            string path = "C:\\CPUAnalysis.xlsx";
                            if (File.Exists(path))
                            {
                                MessageBox.Show("C:\\CPUAnalysis.xlsx文件已存在，将删除此文件");
                                File.Delete(path);
                            }
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.ResultCPUToExcel));
                            workthread.Start();
                            //ResultCPUToExcel(path, "CPU使用走势");
                        }
                        break;
                    case "DAS":
                        {
                            string path = "C:\\DASDAnalysis.xlsx";
                            if (File.Exists(path))
                            {
                                MessageBox.Show("C:\\DASDAnalysis.xlsx文件已存在，将删除此文件");
                                File.Delete(path);
                            }
                            if (dasdFileLoadVar.Text == "否")
                            {
                                this.StartPublicPropertiesGroup();
                                uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                                uithread.Start();

                                Thread workthread = new Thread(new ThreadStart(this.ResultDASDToExcel));
                                workthread.Start();
                            }
                            else
                            {
                                this.StartPublicPropertiesGroup();
                                uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                                uithread.Start();

                                Thread workthread = new Thread(new ThreadStart(this.PrepareDataTable));
                                workthread.Start();
                            }
                            //ResultDASDToExcel(path, "DASD响应时间");
                        }
                        break;
                    case "TTR":
                        {
                            string path = "C:\\TTRNAnalysis.xlsx";
                            if (File.Exists(path))
                            {
                                MessageBox.Show("C:\\TTRNAnalysis.xlsx文件已存在，将删除此文件");
                                File.Delete(path);
                            }
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.ResultTTRNToExcel));
                            workthread.Start();
                            //ResultTTRNToExcel(path, "总交易笔数与分类交易笔数");
                        }
                        break;
                    case "TRN":
                        {
                            string path = "C:\\TRNRAnalysis.xlsx";
                            if (File.Exists(path))
                            {
                                MessageBox.Show("C:\\TRNRAnalysis.xlsx文件已存在，将删除此文件");
                                File.Delete(path);
                            }
                            this.StartPublicPropertiesGroup();
                            uithread = new Thread(new ThreadStart(this.UpdateProgressThread));
                            uithread.Start();

                            Thread workthread = new Thread(new ThreadStart(this.ResultTRNRToExcel));
                            workthread.Start();
                            //ResultTRNRToExcel(path, "Transaction Rate");
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /*
         *结果数据生成Excel图表方法
         */
        public void CreateChart(MSExcel.Workbook mBook, MSExcel.Worksheet mSheet, string row, string col, string caseVar)
        {
            try
            {
                MSExcel.Range oResizeRange;
                MSExcel.Series oSeries;
                //设置数据取值范围
                switch (caseVar)
                {
                    case "CPU1":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlLineMarkers;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlColumns);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(10, Type.Missing);
                        mSheet.Shapes.Item(1).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(40, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(1).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(1).Width = 600;   //调图表的宽度
                        mSheet.Shapes.Item(1).Height = 280;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 600;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "CPU2":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlLineMarkers;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlColumns);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(2, Type.Missing);
                        mSheet.Shapes.Item(1).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(12, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(1).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(1).Width = 600;   //调图表的宽度
                        mSheet.Shapes.Item(1).Height = 280;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 600;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "TTRN1":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlColumnClustered;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlRows);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(7, Type.Missing);
                        mSheet.Shapes.Item(1).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(2, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(1).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(1).Width = 650;   //调图表的宽度
                        mSheet.Shapes.Item(1).Height = 300;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 650;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "TTRN2":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlColumnClustered;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlRows);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(32, Type.Missing);
                        mSheet.Shapes.Item(2).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(2, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(2).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(2).Width = 500;   //调图表的宽度
                        mSheet.Shapes.Item(2).Height = 250;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 500;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "TRNR1":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlLineMarkers;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlColumns);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(4, Type.Missing);
                        mSheet.Shapes.Item(1).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(6, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(1).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(1).Width = 700;   //调图表的宽度
                        mSheet.Shapes.Item(1).Height = 300;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 700;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "TRNR2":
                        mBook.Charts.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                        mBook.ActiveChart.ChartType = MSExcel.XlChartType.xlLineMarkers;//设置图形
                        //以下是给图表放在指定位置
                        mBook.ActiveChart.Location(MSExcel.XlChartLocation.xlLocationAsObject, mSheet.Name);
                        mBook.ActiveChart.SetSourceData(mSheet.get_Range(row, col), MSExcel.XlRowCol.xlColumns);
                        oResizeRange = (MSExcel.Range)mSheet.Rows.get_Item(29, Type.Missing);
                        mSheet.Shapes.Item(2).Top = (float)(double)oResizeRange.Top;  //调图表的位置上边距
                        oResizeRange = (MSExcel.Range)mSheet.Columns.get_Item(6, Type.Missing);  //调图表的位置左边距
                        mSheet.Shapes.Item(2).Left = (float)(double)oResizeRange.Left;
                        //mBook.ActiveChart.Location(Excel.XlChartLocation.xlLocationAutomatic, title);
                        mSheet.Shapes.Item(2).Width = 700;   //调图表的宽度
                        mSheet.Shapes.Item(2).Height = 300;  //调图表的高度
                        // mBook.ActiveChart.PlotArea.Interior.ColorIndex = 19;  //设置绘图区的背景色 
                        mBook.ActiveChart.PlotArea.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置绘图区边框线条
                        mBook.ActiveChart.PlotArea.Width = 700;   //设置绘图区宽度
                        //mBook.ActiveChart.ChartArea.Interior.ColorIndex = 10; //设置整个图表的背影颜色
                        //mBook.ActiveChart.ChartArea.Border.ColorIndex = 8;// 设置整个图表的边框颜色
                        mBook.ActiveChart.ChartArea.Border.LineStyle = MSExcel.XlLineStyle.xlContinuous;//设置边框线条
                        mBook.ActiveChart.HasDataTable = false;
                        break;
                    case "DAS":
                        break;
                    default:
                        break;
                }

                //设置图例的位置和格式
                //mBook.ActiveChart.Legend.Top = 20.00; //具体设置图例的上边距
                //mBook.ActiveChart.Legend.Left = 80.00;//具体设置图例的左边距
                mBook.ActiveChart.Legend.Interior.ColorIndex = MSExcel.XlColorIndex.xlColorIndexNone;
                //mBook.ActiveChart.Legend.Width = 150;
                mBook.ActiveChart.Legend.Font.Size = 9;
                //mBook.ActiveChart.Legend.Font.Bold = true;
                mBook.ActiveChart.Legend.Font.Name = "宋体";
                //mBook.ActiveChart.Legend.Position = Excel.XlLegendPosition.xlLegendPositionTop;//设置图例的位置
                mBook.ActiveChart.Legend.Border.LineStyle = MSExcel.XlLineStyle.xlLineStyleNone;//设置图例边框线条

                //设置X轴的显示
                MSExcel.Axis xAxis = (MSExcel.Axis)mBook.ActiveChart.Axes(MSExcel.XlAxisType.xlValue, MSExcel.XlAxisGroup.xlPrimary);
                xAxis.MajorGridlines.Border.LineStyle = MSExcel.XlLineStyle.xlDot;
                //xAxis.MajorGridlines.Border.ColorIndex = 1;//gridLine横向线条的颜色
                xAxis.HasTitle = true;
                switch (caseVar)
                {
                    case "CPU1":
                        xAxis.AxisTitle.Text = "（LPAR Busy/MVS Busy）";
                        break;
                    case "CPU2":
                        xAxis.AxisTitle.Text = "（LPAR Busy%）";
                        break;
                    case "TTRN1":
                        xAxis.AxisTitle.Text = "（交易量）";
                        break;
                    case "TTRN2":
                        xAxis.AxisTitle.Text = "（分组交易量）";
                        break;
                    case "TRNR1":
                        xAxis.AxisTitle.Text = "（Transaction Rate 笔/s）";
                        break;
                    case "TRNR2":
                        xAxis.AxisTitle.Text = "（Transaction Rate 笔/s）";
                        break;
                    case "DAS":
                        break;
                    default:
                        break;
                }
                //xAxis.MinimumScale = 1500;
                //xAxis.MaximumScale = 6000;
                xAxis.TickLabels.Font.Name = "宋体";
                xAxis.TickLabels.Font.Size = 9;

                //设置Y轴的显示
                MSExcel.Axis yAxis = (MSExcel.Axis)mBook.ActiveChart.Axes(MSExcel.XlAxisType.xlCategory, MSExcel.XlAxisGroup.xlPrimary);
                // yAxis.TickLabelSpacing = 30;
                //yAxis.TickLabels.NumberFormat = "M月D日";
                yAxis.TickLabels.Orientation = MSExcel.XlTickLabelOrientation.xlTickLabelOrientationHorizontal;//Y轴显示的方向,是水平还是垂直等
                yAxis.TickLabels.Font.Size = 8;
                yAxis.TickLabels.Font.Name = "宋体";
                yAxis.HasTitle = true;
                switch (caseVar)
                {
                    case "CPU1":
                        yAxis.AxisTitle.Text = "（次数）";
                        break;
                    case "CPU2":
                        yAxis.AxisTitle.Text = "（时间5m）";
                        break;
                    case "TTRN1":
                        yAxis.AxisTitle.Text = "（交易）";
                        break;
                    case "TTRN2":
                        yAxis.AxisTitle.Text = "（分组交易）";
                        break;
                    case "TRNR1":
                        yAxis.AxisTitle.Text = "（时间 s）";
                        break;
                    case "TRNR2":
                        yAxis.AxisTitle.Text = "（时间 s）";
                        break;
                    case "DAS":
                        break;
                    default:
                        break;
                }
                //mBook.ActiveChart.HasTitle = true;
                //mBook.ActiveChart.ChartTitle.Text = "LPAR Busy/MVS Busy";
                //mBook.ActiveChart.ChartTitle.Shadow = false;
                //mBook.ActiveChart.Floor.Interior.ColorIndex = 8;  
                //mBook.ActiveChart.ChartTitle.Border.LineStyle = Excel.XlLineStyle.xlContinuous;
                oSeries = (MSExcel.Series)mBook.ActiveChart.SeriesCollection(1);
                //oSeries.Border.ColorIndex = 9;
                oSeries.Border.Weight = MSExcel.XlBorderWeight.xlThick;
                //oSeries = (MSExcel.Series)mBook.ActiveChart.SeriesCollection(2);
                //oSeries.Border.ColorIndex = 9;
                //oSeries.Border.Weight = MSExcel.XlBorderWeight.xlThick;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.EndPublicPropertiesGroup();
            }
        }

        /*
         * 
         * 监控结果文件选择
         */
        private void fileOpen_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "选择TXT监控结果文件";
                //openFileDialog1.InitialDirectory = @"C:\";
                openFileDialog1.Filter = "TXT文件(*.txt)|*.txt";
                openFileDialog1.Multiselect = false;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FileName = null;
                openFileDialog1.ShowDialog();
                openFileDialog1.ValidateNames = true;
                fileTextBox.Text = openFileDialog1.FileName;
                fileText.Text = "";
            }
            catch
            {
                this.Close();
            }
        }

        /*
         * 监控结果文件选择
         */
        private void fileOpenMenu_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "选择TXT监控结果文件";
                //openFileDialog1.InitialDirectory = @"C:\";
                openFileDialog1.Filter = "TXT文件(*.txt)|*.txt";
                openFileDialog1.Multiselect = false;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FileName = null;
                openFileDialog1.ShowDialog();
                openFileDialog1.ValidateNames = true;
                fileTextBox.Text = openFileDialog1.FileName;
                fileText.Text = "";
            }
            catch
            {
                this.Close();
            }
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.CanSelect == true)
            {
                fileText.Text = "";
                fileTextBox.Text = "";
            }

        }

        private void toolFileOpen_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Title = "选择TXT监控结果文件";
                //openFileDialog1.InitialDirectory = @"C:\";
                openFileDialog1.Filter = "TXT文件(*.txt)|*.txt";
                openFileDialog1.Multiselect = false;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FileName = null;
                openFileDialog1.ShowDialog();
                openFileDialog1.ValidateNames = true;
                fileTextBox.Text = openFileDialog1.FileName;
                fileText.Text = "";
            }
            catch
            {
                this.Close();
            }
        }

        private void dasdFileLoadVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (typeComboBox.CanSelect == true)
            {
                fileText.Text = "";
            }
        }

        private void StartPublicPropertiesGroup()
        {
            this.progressBar1.Visible = true;
            this.typeComboBox.Enabled = false;
            this.fileProcess.Enabled = false;
            this.createExcel.Enabled = false;
            this.fileOpen.Enabled = false;
            this.deviceType.Enabled = false;
            this.trNum.Enabled = false;
            this.sortVar.Enabled = false;
            this.dasdFileLoadVar.Enabled = false;
            this.toolFileOpen.Enabled = false;
            this.fileOpenMenu.Enabled = false;
            this.dasdTopOutputVar.Enabled = false;
            this.daseTopOutputLine.Enabled = false;
        }

        private void EndPublicPropertiesGroup()
        {
            uithread.Abort();
            this.progressBar1.Visible = false;
            this.typeComboBox.Enabled = true;
            this.createExcel.Enabled = true;
            this.fileProcess.Enabled = true;
            this.fileOpen.Enabled = true;
            this.deviceType.Enabled = true;
            this.trNum.Enabled = true;
            this.sortVar.Enabled = true;
            this.dasdFileLoadVar.Enabled = true;
            this.toolFileOpen.Enabled = true;
            this.fileOpenMenu.Enabled = true;
            this.dasdTopOutputVar.Enabled = true;
            this.daseTopOutputLine.Enabled = true;
            this.label5.Text = "当前处理行数";
        }
    }

}
