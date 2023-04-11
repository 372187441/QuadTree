using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    public class SectorCheck : MonoBehaviour
    {
        // 扇形边长
        public float arcRadius;
        // 扇形扫掠半角
        public float arcTheta;
        // 扇形朝向角度
        public float arcDegree { get { return -transform.eulerAngles.y; } }
        // 扇形原点
        public Vector2 arcCenter { get { return new Vector2(transform.position.x, transform.position.z); } }

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
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + Time.deltaTime * 20, 0);
            Vector2 t = new Vector2(
                arcCenter.magnitude * Mathf.Cos(arcDegree * Mathf.Deg2Rad),
                arcCenter.magnitude * Mathf.Sin(arcDegree * Mathf.Deg2Rad)
            );

            list.Clear();
            quadTree.searchSectorArea(arcCenter, t.normalized, (arcTheta * Mathf.Deg2Rad) / 2, arcRadius, ref list);

            foreach (var item in list)
            {
                ((RectangleNode)item.bindNode).Highlight(true);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var c = new Vector3(arcCenter.x, 0, arcCenter.y);
            var from = new Vector3(arcCenter.x + arcRadius * Mathf.Cos((arcDegree + arcTheta / 2) * Mathf.Deg2Rad), 0,
                arcCenter.y + arcRadius * Mathf.Sin((arcDegree + arcTheta / 2) * Mathf.Deg2Rad));
            var to = new Vector3(arcCenter.x + arcRadius * Mathf.Cos((arcDegree - arcTheta / 2) * Mathf.Deg2Rad), 0,
                arcCenter.y + arcRadius * Mathf.Sin((arcDegree - arcTheta / 2) * Mathf.Deg2Rad));
            var t = new Vector2(arcCenter.x + arcRadius * Mathf.Cos(arcDegree * Mathf.Deg2Rad),
                arcCenter.y + arcRadius * Mathf.Sin(arcDegree * Mathf.Deg2Rad));
            var ct = new Vector3(t.x, 0, t.y);

            Gizmos.color = Color.red;
            UnityEditor.Handles.DrawWireArc(c, Vector3.up, from - c, arcTheta, arcRadius);
            UnityEditor.Handles.DrawLine(c, from);
            UnityEditor.Handles.DrawLine(c, to);
            UnityEditor.Handles.DrawLine(c, ct);
        }
#endif
    }
}
