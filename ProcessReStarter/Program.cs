using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessReStarter
{
    /// <summary>
    /// 程式人生重開器
    /// 說明：
    /// config內註記要關閉的程式名稱，名稱為工作管理員內的名稱
    /// 並註記要重開的程式位置(絕對位置)
    /// </summary>
    class Program
    {
        public static string ConfigName = "Config.txt";
        private static string _Path = string.Empty;
        static void Main(string[] args)
        {
            InitConfig();

            #region Killer

            //取得config 要關掉的程式名稱用 ','區分開
            string strKillProcessNames = GetConfig("KillProcessNames");

            //判斷config檔案設定
            if (strKillProcessNames == "")
            {
                Console.WriteLine("請先設定Config檔案，KillProcessNames");

                string configpath = Path.Combine(Environment.CurrentDirectory, ConfigName);
                Process.Start(configpath);
                Console.ReadKey();
                return;
            }
            //要關掉的程式名稱用 ','區分開
            List<string> lstProcessNames = strKillProcessNames.Split(',').ToList();
            var Allprocess = Process.GetProcesses();
            foreach (var processName in lstProcessNames)
            {
                foreach (Process process in Allprocess)
                {
                    if (process.MainWindowTitle == processName)
                    {
                        process.Kill();
                    }
                }
            }
            #endregion


            #region Restarter
            string strreStartProcessNames = GetConfig("RestartLocation");
            //要開啟的程式名稱用 ','區分開 (請用絕對位置)
            List<string> lstStartProcessPath = strreStartProcessNames.Split(',').ToList();
            foreach (string  RestartProcess in lstStartProcessPath)
            {
                Process.Start(RestartProcess);
            }
            #endregion
        }


        #region 記事本參數
        public static void InitConfig()
        {
            _Path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(Path.Combine(_Path, ConfigName)))
            {
                using (StreamWriter w = File.AppendText(Path.Combine(_Path, ConfigName)))
                {

                    string Defaultstring = '{' + Environment.NewLine
                         + "\"KillProcessNames\":\"\"," + Environment.NewLine
                         + "\"RestartLocation\":\"\"" + Environment.NewLine
                         + "}" + Environment.NewLine;
                    w.WriteAsync(Defaultstring);

                }
            }
        }

        public static string GetConfig(string Key)
        {
            var jobject = JObject.Parse(File.ReadAllText(Path.Combine(_Path, ConfigName)));
            return jobject[Key].ToString();
        }
        /// <summary>
        /// 修改Config值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="NewValue"></param>
        /// <returns></returns>
        public static bool SetConfig(string Key, string NewValue)
        {
            var jobject = JObject.Parse(File.ReadAllText(Path.Combine(_Path, ConfigName)));
            jobject[Key] = NewValue;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jobject, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(ConfigName, output);
            return true;
        }
        #endregion
    }
}
