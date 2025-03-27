using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fusion
{
    public static class NetworkHelper
    {
        /// <summary>
        /// 检查网络是否可用
        /// </summary>
        /// <returns></returns>
        public static bool CheckNetworkAvailable()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface netInterface in networkInterfaces)
            {
                if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    if (netInterface.OperationalStatus == OperationalStatus.Up)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        // /// <summary>
        // /// 通过网络获取当前的DateTime
        // /// 需要导入SimpleJSON
        // /// </summary>
        // /// <returns></returns>
        // public static async UniTask<DateTime> GetDateTimeByNetwork()
        // {
        //     DateTime dateTime = DateTime.Now;
        //     try
        //     {
        //         using HttpClient client = new HttpClient();
        //         HttpResponseMessage response = await client.GetAsync("https://worldtimeapi.org/api/ip");
        //
        //         if (response.IsSuccessStatusCode)
        //         {
        //             string result = await response.Content.ReadAsStringAsync();
        //
        //             // 获取到的数据格式：
        //             //{
        //             //"abbreviation":"CST",
        //             //"client_ip":"59.174.171.185",
        //             //"datetime":"2023-06-27T15:05:00.430899+08:00",
        //             //"day_of_week":2,
        //             //"day_of_year":178,
        //             //"dst":false,
        //             //"dst_from":null,
        //             //"dst_offset":0,
        //             //"dst_until":null,
        //             //"raw_offset":28800,
        //             //"timezone":"Asia/Shanghai",
        //             //"unixtime":1687849500,
        //             //"utc_datetime":"2023-06-27T07:05:00.430899+00:00",
        //             //"utc_offset":"+08:00",
        //             //"week_number":26
        //             //}
        //             
        //             var rootNode = SimpleJSON.JSON.Parse(result);
        //             if (rootNode != null)
        //             {
        //                 string time = rootNode["datetime"];
        //                 if (!string.IsNullOrEmpty(time))
        //                 {
        //                     string t = time.Substring(0, 19).Replace("T", " ");
        //                     string[] tt = t.Split(" ");
        //                     var ymd = tt[0].Split("-");
        //                     int.TryParse(ymd[0], out int year);
        //                     int.TryParse(ymd[1], out int month);
        //                     int.TryParse(ymd[2], out int day);
        //                     var hms = tt[1].Split(':');
        //                     int.TryParse(hms[0], out int hour);
        //                     int.TryParse(hms[1], out int min);
        //                     int.TryParse(hms[2], out int sec);
        //                     dateTime = new DateTime(year, month, day, hour, min, sec);
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.Log($"Get datetime by network failed：\n{e}");
        //     }
        //
        //     return dateTime;
        // }
    }
}