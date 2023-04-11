using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    /// <summary>
    /// 四叉树节点
    /// </summary>
    public class QuadTreeData
    {
        /// <summary> 碰撞矩阵 </summary>
        public Rect rect;
        /// <summary> 归属区域 </summary>
        public List<QuadTree> quadTrees = new List<QuadTree>();
        /// <summary> 绑定节点 </summary>
        public Object bindNode;
    }
}
