using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fyw
{
    /// <summary>
    /// 支持Unity超链接富文本语法
    /// </summary>
    public class HyperLink : Text, IPointerClickHandler
    {
        /// <summary>
        /// 超链接信息类
        /// </summary>
        private class LinkInfo
        {
            /// <summary>
            /// 起始序号
            /// </summary>
            public int start;

            /// <summary>
            /// 结束序号
            /// </summary>
            public int end;

            /// <summary>
            /// 内容
            /// </summary>
            public string name;

            /// <summary>
            /// 包围框
            /// </summary>
            public List<Rect> rects = new List<Rect>();
        }

        /// <summary>
        /// 超链接正则
        /// </summary>
        private static Regex href_regex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);

        /// <summary>
        /// 颜色正则
        /// </summary>
        private static Regex color_regex = new Regex(@"<color=([^>\n\s]+)>(.*?)(</color>)", RegexOptions.Singleline);

        /// <summary>
        /// 超链接信息列表
        /// </summary>
        private List<LinkInfo> infos = new List<LinkInfo>();

        /// <summary>
        /// 链接点击事件
        /// </summary>
        public UnityEvent<string> OnLinkClick = new UnityEvent<string>();

        /// <summary>
        /// 渲染处理
        /// </summary>
        /// <param name="toFill"></param>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            this.InitInfo();
            this.InitRect(toFill);
        }

        /// <summary>
        /// 点击处理
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out point);
            foreach (LinkInfo info in infos)
            {
                var rects = info.rects;
                for (var i = 0; i < rects.Count; ++i)
                {
                    if (rects[i].Contains(point))
                    {
                        this.OnLinkClick.Invoke(info.name);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化连接信息
        /// </summary>
        private void InitInfo()
        {
            infos.Clear();
            int index = 0;
            StringBuilder sb = new StringBuilder();
            foreach (Match match in href_regex.Matches(this.text))
            {
                sb.Append(this.text.Substring(index, match.Index - index));

                //空格和回车没有顶点渲染，所以要去掉
                sb = sb.Replace(" ", "");
                sb = sb.Replace("\n", "");
                int start = sb.Length;

                //第一个是连接url,第二个是连接文本，跳转用url，计算index用文本
                Group url_group = match.Groups[1];
                Group title_group = match.Groups[2];

                //如果有Color语法嵌套，则还要继续扒，知道把最终文本扒出来
                Match color_match = color_regex.Match(title_group.Value);
                if (color_match.Groups.Count > 3)
                {
                    title_group = color_match.Groups[2];
                }
                sb.Append(title_group.Value);
                LinkInfo info = new LinkInfo
                {
                    start = start,
                    end = (start + title_group.Value.Length),
                    name = url_group.Value
                };
                index = match.Index + match.Length;
                infos.Add(info);
            }
            sb.Append(this.text.Substring(index, this.text.Length - index));
        }

        /// <summary>
        /// 初始化连接包围框
        /// </summary>
        /// <param name="toFill"></param>
        private void InitRect(VertexHelper toFill)
        {
            //处理超链接包围框
            UIVertex vert = new UIVertex();
            foreach (var info in infos)
            {
                info.rects.Clear();

                //一个字符是四个顶点，所以Index要乘以4
                int start_vertex = info.start * 4;
                int end_vertex = info.end * 4;
                if (start_vertex >= toFill.currentVertCount)
                {
                    continue;
                }

                //将超链接里面的文本顶点索引坐标加入到包围框
                toFill.PopulateUIVertex(ref vert, start_vertex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (int i = start_vertex; i < end_vertex; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }
                    toFill.PopulateUIVertex(ref vert, i);
                    pos = vert.position;
                    if (pos.x < bounds.min.x)//换行重新添加包围框
                    {
                        info.rects.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(pos);//扩展包围框
                    }
                }
                info.rects.Add(new Rect(bounds.min, bounds.size));
            }
        }
    }
}