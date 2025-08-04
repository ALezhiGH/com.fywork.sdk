using System;
using System.Collections;
using UnityEngine;
#if FY_UMENG
using Umeng;
#endif

namespace SDK
{
	/// <summary>
	/// 友盟统计处理器
	/// </summary>
	public class UmengStatHandler : StatHandler
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="args"></param>
		protected override void Init(Hashtable args)
		{
			if (!Application.isEditor)
			{
				try
				{
#if FY_UMENG
					this.key = args["key"].ToString();
					GA.StartWithAppKeyAndChannelId(this.key, this.chnl_ctrl.Chnl.ToString());
#endif
					this.Back();
				}
				catch (Exception e)
				{
					Debug.LogError(e.StackTrace);
					Debug.LogError(e.Message);
				}
			}
			else
			{
				this.Back();
			}
		}

		/// <summary>
		/// 增加统计
		/// </summary>
		/// <param name="key"></param>
		public override void AppendStat(string key)
		{
			if (!Application.isEditor)
			{
#if FY_UMENG
				GA.Event(key);
#endif
			}
		}

		/// <summary>
		/// 增加统计
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public override void AppendStat(string key, int value)
		{
			if (!Application.isEditor)
			{
#if FY_UMENG
				GA.Event(key, value.ToString());
#endif
			}
		}

		/// <summary>
		/// 增加统计
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public override void AppendStat(string key, string value)
		{
			if (!Application.isEditor)
			{
#if FY_UMENG
				GA.Event(key, value);
#endif
			}
		}

		/// <summary>
		/// 打开面板
		/// </summary>
		/// <param name="panel_name"></param>
		public override void OpenPanel(string panel_name)
		{
			if (!Application.isEditor)
			{
#if FY_UMENG
				GA.PageBegin(panel_name);
#endif
			}
		}

		/// <summary>
		/// 关闭面板
		/// </summary>
		/// <param name="panel_name"></param>
		public override void ClosePanel(string panel_name)
		{
			if (!Application.isEditor)
			{
#if FY_UMENG
				GA.PageEnd(panel_name);
#endif
			}
		}
	}
}
