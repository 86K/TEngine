using Newtonsoft.Json;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// 项目配置。
    /// </summary>
    public class Config
    {
        public string remoteIp;
        public int remotePort;
    }
    
    /// <summary>
    /// 项目的全局管理器。
    /// </summary>
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [HideInInspector]
        public Config Config;
        
        public override void Initialize()
        {
            // 加载项目配置
            LoadConfig();
           
            // 初始化Fantasy管理器
            FantasyManager.Instance.Initialize();
        }

        private void LoadConfig()
        {
            Log.Info($"加载项目配置");

            var configTxt = GameModule.Resource.LoadAsset<Text>("Config.json");
            if (configTxt == null)
            {
                Log.Info($"AssetRaw/Configs/Config.json不存在，请检查");
            }
            else
            {
                Config = JsonConvert.DeserializeObject<Config>(configTxt.text);
            }
        }
    }
}