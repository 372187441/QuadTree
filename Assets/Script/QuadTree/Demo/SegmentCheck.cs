using QuadTreeSpace;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    public class SegmentCheck : MonoBehaviour
    {
        public Vector2 rayStart { get { return new Vector2(transform.position.x, transform.position.z); } }
        public Vector2 rayEnd;

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
            quadTree.rayCast(rayStart, rayEnd, ref list);

            foreach (var item in list)
            {
                ((RectangleNode)item.bindNode).Highlight(true);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(rayStart.x, 0, rayStart.y), new Vector3(rayEnd.x, 0, rayEnd.y));
        }
#endif
    }
}
