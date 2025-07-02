using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 统计控制器
    /// </summary>
    public class StatControl : MonoBehaviour
    {
        /// <summary>
        /// 控制接口
        /// </summary>
        public static StatControl Instance;

        /// <summary>
        /// 启动次数
        /// </summary>
        private int start_cnt;

        /// <summary>
        /// 游戏时长
        /// </summary>
        private float play_time;

        /// <summary>
        /// 启动次数
        /// </summary>
        public int StartCnt { get => this.start_cnt; }

        /// <summary>
        /// 游戏时长
        /// </summary>
        public float PlayTime { get => this.play_time; }

        /// <summary>
        /// 唤醒
        /// </summary>
        void Awake()
        {
            Instance = this;
            this.start_cnt = 1;
            this.play_time = 0;
            string key = "Fywork.StartCnt";
            if (PlayerPrefs.HasKey(key))
            {
                this.start_cnt = PlayerPrefs.GetInt(key) + 1;
            }
            PlayerPrefs.SetInt(key, this.start_cnt);
            PlayerPrefs.Save();
            key = "Fywork.PlayTime";
            if (PlayerPrefs.HasKey(key))
            {
                this.play_time = PlayerPrefs.GetFloat(key);
            }
        }

        /// <summary>
        /// 更新处理
        /// </summary>
        void Update()
        {
            this.play_time += Time.deltaTime;
        }

        /// <summary>
        /// 销毁处理
        /// </summary>
        void OnDestroy()
        {
            string key = "Fywork.PlayTime";
            PlayerPrefs.SetFloat(key, this.play_time);
            PlayerPrefs.Save();
        }
    }
}