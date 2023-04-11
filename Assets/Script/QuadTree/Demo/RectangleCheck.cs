using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    public class RectangleCheck : MonoBehaviour
    {
        public float width;
        public float height;

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
            quadTree.searchRectArea(GetColliderRect(), GetAngel(), ref list);

            foreach (var item in list)
            {
                ((RectangleNode)item.bindNode).Highlight(true);
            }
        }

        public Rect GetColliderRect()
        {
            return new Rect(transform.position.x - width / 2, transform.position.z - height / 2, width, height);
        }

        public float GetAngel()
        {
            return -transform.rotation.eulerAngles.y;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            UnityEditor.Handles.color = Color.red;
            drawObbRect(GetColliderRect(), GetAngel());
        }

        void drawObbRect(Rect rect, float theta)
        {
            var v = MathLib.getObbRectCorner(rect, theta);

            Vector3 lu = new Vector3(v[0].x, 0, v[0].y);
            Vector3 lb = new Vector3(v[1].x, 0, v[1].y);
            Vector3 rb = new Vector3(v[2].x, 0, v[2].y);
            Vector3 ru = new Vector3(v[3].x, 0, v[3].y);

            Gizmos.DrawLine(lu, lb);
            Gizmos.DrawLine(lb, rb);
            Gizmos.DrawLine(rb, ru);
            Gizmos.DrawLine(ru, lu);
        }
#endif
    }
}
