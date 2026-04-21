using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 项目配置。
    /// </summary>
    public class Config
    {
        public string remoteIp;
        public int remotePort;
        
        // 其他配置字段...
    }
    
    /// <summary>
    /// 项目的全局管理器。
    /// </summary>
    public class GameManager : SingletonBehaviour<GameManager>
    {
        // 配置数据给一些必要的默认值
        public static Config Config = new()
        {
            remoteIp = "127.0.0.1",
            remotePort = 8081,
        };
        
        public override void Initialize()
        {
            base.Initialize();

            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await LoadConfigAsync();

            // NOTE：再这里对所有的全局管理器进行初始化，不需要手动挂载到main中。
            // 初始化Fantasy管理器
            FantasyManager.Instance.Initialize();
            // 初始化鼠标管理器
            MouseManager.Instance.Initialize();
        }

        private async UniTask LoadConfigAsync()
        {
            var folderPath = Application.streamingAssetsPath + "/Configs";
            if (!Directory.Exists(folderPath))
            {
                Log.Info($"配置文件夹不存在：{folderPath}");
                return;
            }
            
            var filePath = folderPath + "/Config.json";
            if (!File.Exists(filePath))
            {
                Log.Info($"配置文件不存在：{filePath}");
                return;
            }
            
            var configText = await UniTask.RunOnThreadPool(() => File.ReadAllText(filePath));
            var config = JsonConvert.DeserializeObject<Config>(configText);
            if (config != null)
            {
                Config = config;
            }
        }
    }
}
