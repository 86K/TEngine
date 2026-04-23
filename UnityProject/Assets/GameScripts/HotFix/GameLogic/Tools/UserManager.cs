using Fantasy;
using UnityEditor;
using UnityEngine;

namespace GameLogic
{
    /// <summary>
    /// 用户信息。
    /// </summary>
    public sealed class UserInfo
    {
        /// <summary>
        /// 账号。
        /// </summary>
        public string account;
    
        /// <summary>
        /// 密码。
        /// </summary>
        public string password;

        /// <summary>
        /// 用户编号。
        /// </summary>
        public long userId;
    }
    
    public class UserManager : SingletonBehaviour<UserManager>
    {
        private UserInfo _userInfo;
        
        public void RecordUserInfo(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public void SignOut()
        {
            if (_userInfo != null)
            {
                FantasyManager.Instance.Session.C2G_SignoutMessage(_userInfo.userId);
            }
            
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}