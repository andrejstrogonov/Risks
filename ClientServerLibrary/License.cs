using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Windows;

namespace ClientServerLibrary
{ public static class License
    {
        public static string LicenseFilePath = @"license_key.txt";
        public static string LicenseKey { get; set; }
        public static int ExitCode { get; set; }

        public static DateTime FinishDate { get; set; }
        public static long PeriodLeft { get; set; }

        public static DateTime Start { get; set; }

        public static bool ReadKeyFromFile()
        {
            if (File.Exists(LicenseFilePath))
            {
                FileStream fileStream = new FileStream(LicenseFilePath, FileMode.Open);
                StreamReader streamReader = new StreamReader(fileStream);
                if (!streamReader.EndOfStream)
                {
                    LicenseKey = streamReader.ReadLine();

                }
                streamReader.Close();
                fileStream.Close();
                if (LicenseKey is null || LicenseKey.Length != 15)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void SaveNewKey(string key)
        {
            LicenseKey = key;
            FileStream fileStream = new FileStream(LicenseFilePath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine(LicenseKey);
            streamWriter.Close();
            fileStream.Close();
            //SaveCodeWord(key_start, key);
        }

        public static string GetVideoID()
        {
            try
            {
                ManagementObjectSearcher searcher =
                  new ManagementObjectSearcher("root\\CIMV2",
                  "SELECT * FROM Win32_VideoController");
                List<string> results = new List<string>();
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var result = queryObj["PNPDeviceID"];
                    results.Add(result.ToString());
                }


                string myID = results.Last();
                var arr = myID.Split('\\', '&');
                myID = arr[arr.Length - 3];
                return myID;
            }
            catch (ManagementException e)
            {
                return "";
            }
        }



        public static bool ActivateLicense(string key)
        {
                var result = GetKeyVarification(key);
                if (result is null || !result.IsKeyExists)
                {
                    return false;
                }
                if (result.PeriodLeft <= 0)
                    return false;

            /*if (result.IsUsingNow)
            {
                if (IsLastWasSuccess(key)==false)
                {
                    StopUsing(key);
                }
                result = GetKeyVarification(key);
                if (result is null || !result.IsKeyExists)
                {
                    return false;
                }
                if (result.PeriodLeft <= 0)
                    return false;
            }*/

            PeriodLeft = result.PeriodLeft;
                FinishDate = result.DateOfFinish;
            if((DateTime.Now-Start).TotalMinutes>31)
                Start = DateTime.Now;

                SaveNewKey(key);
            
                return true;
            
        }

        public static bool StopUsing()
        {
            string key = LicenseKey;
            var finish = DateTime.Now;
            var ll = finish - Start;
            double longness = ll.TotalMinutes / (60 * 24);         
            key = key + GetVideoID();
            var HttpClient = new HttpClient();
            HttpClient.BaseAddress = new Uri("http://www.simplexplan.somee.com/publish/");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            bool result = false;
            HttpResponseMessage response = HttpClient.GetAsync($"api/Users/activate/{key}/{longness}").Result;
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsAsync<bool>().Result;
            }
            return result;
        }
        public static AuthentificationAswer GetKeyVarification(string key)
        {
            
            //MessageBox.Show(myIP);
            key = key + GetVideoID();
            var HttpClient = new HttpClient(); 
            HttpClient.BaseAddress = new Uri("http://www.simplexplan.somee.com/publish/");
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            AuthentificationAswer result = null;
            HttpResponseMessage response = HttpClient.GetAsync($"api/Users/activate/{key}").Result;            
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsAsync<AuthentificationAswer>().Result;
            }
            return result;
        }
    }
}
