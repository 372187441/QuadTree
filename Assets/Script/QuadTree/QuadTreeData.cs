using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    /// <summary>
    /// �Ĳ����ڵ�
    /// </summary>
    public class QuadTreeData
    {
        /// <summary> ��ײ���� </summary>
        public Rect rect;
        /// <summary> �������� </summary>
        public List<QuadTree> quadTrees = new List<QuadTree>();
        /// <summary> �󶨽ڵ� </summary>
        public Object bindNode;
    }
}
