using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    public class CircleCheck : MonoBehaviour
    {
        public float radius;
        public Vector2 center { get { return new Vector2(transform.position.x, transform.position.z); } }

        private QuadTree quadTree;

        void Start()
        {

        }

        public void Init(QuadTree quadTree)
        {
            this.quadTree = quadTree;
        }

        HashSet<QuadTreeData> list = new HashSet<QuadTreeData>();
        public void ManualUpdate()
        {
            list.Clear();
            quadTree.searchCircleArea(center, radius, ref list);

            foreach (var item in list)
            {
                ((RectangleNode)item.bindNode).Highlight(true);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, radius);
        }
#endif
    }
}