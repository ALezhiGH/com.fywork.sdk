using UnityEngine;

namespace SDK
{
    /// <summary>
    /// 日志工具
    /// </summary>
    public class LogTool
    {
        /// <summary>
        /// 常规消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Norm(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// 常规消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Norm(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            LogTool.Norm(msg);
        }

        /// <summary>
        /// 警告消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(string msg)
        {
            Debug.LogWarning(msg);
        }

        /// <summary>
        /// 警告消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Warn(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            LogTool.Warn(msg);
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string msg)
        {
            Debug.LogError(msg);
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Error(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            LogTool.Error(msg);
        }
    }
}