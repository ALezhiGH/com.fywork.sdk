using System.Collections;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 泛鱼登录处理器
    /// </summary>
    public class FywLogHandler : LogHandler
    {
        /// <summary>
        /// 保存key
        /// </summary>
        private string save = "";

        /// <summary>
        /// 初始化处理
        /// </summary>
        /// <param name="args"></param>
        protected override void Init(Hashtable args)
        {
            this.save = args["save"].ToString();
            this.Back();
        }

        /// <summary>
        /// 调用处理
        /// </summary>
        protected override void Call()
        {
            string user_id;
            if (!PlayerPrefs.HasKey(this.save))
            {
                user_id = SystemInfo.deviceUniqueIdentifier;
                PlayerPrefs.SetString(this.save, user_id);
                PlayerPrefs.Save();
            }
            else
            {
                user_id = PlayerPrefs.GetString(this.save);
            }
            string open_id = user_id + "_" + Application.identifier;
            string union_id = user_id + "_" + Application.companyName;
            this.LogData.Set(string.Empty, string.Empty, string.Empty, user_id, open_id, union_id);
            this.Back();
        }
    }
}