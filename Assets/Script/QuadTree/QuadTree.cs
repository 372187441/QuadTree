using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    /// <summary>
    /// �Ĳ����ڵ�
    /// </summary>
    public class QuadTree
    {
        /// <summary> ������洢�����ﵽ���ٵ�ʱ��ʼ���� </summary>
        public readonly static int maxSum = 4;
        /// <summary> ������ </summary>
        public readonly static int maxDeep = 4;

        /// <summary> ���ڵ� </summary>
        public QuadTree father;
        /// <summary> �ĸ������� </summary>
        public QuadTree[] childs;
        /// <summary> ��ǰ����洢�Ľڵ� </summary>
        public List<QuadTreeData> objs;
        /// <summary> ��ǰ�ýڵ��ڸ��ڵ��λ�� </summary>
        public int index;
        /// <summary> ��ǰ�ýڵ����ĵ���� </summary>
        public int deep;
        /// <summary> �Լ���λ����Ϣ </summary>
        public Rect rect;
        /// <summary> ������Χ�Ľڵ㣬�洢�ڹ������� </summary>
        public static HashSet<QuadTreeData> outRangeObjs = new HashSet<QuadTreeData>();

        public QuadTree(Rect rect, int index = -1, int deep = 1, QuadTree father = null)
        {
            this.rect = rect;
            this.index = index;
            this.deep = deep;
            this.father = father;
            objs = new List<QuadTreeData>();
        }

        /// <summary>
        /// ��ӽڵ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Add(QuadTreeData obj)
        {
            if (obj.rect.xMin > rect.xMax || obj.rect.xMax < rect.xMin || obj.rect.yMin > rect.yMax || obj.rect.yMax < rect.yMin)
            {
                // û���κν���
                if (father == null)
                {
                    // �����Ĳ����ķ�Χ������ר��λ��
                    obj.quadTrees.Add(this);
                    outRangeObjs.Add(obj);
                }
                return false;
            }

            //����ӽڵ㲻���� ˵����û�зָ�
            if (childs == null)
            {
                obj.quadTrees.Add(this);
                objs.Add(obj);
                // ������ֵ����ʼ�ָ�
                if (deep < maxDeep && objs.Count > maxSum)
                {
                    Split();
                }
                return true;
            }
            else
            {
                var objCenter = obj.rect.center;
                var curCenter = rect.center;
                // ����ڷֽ�����, �������ϲ�������ֵ
                if (objs.Count < maxSum && 
                    (Math.Abs(objCenter.x - curCenter.x) < obj.rect.width / 2 || Math.Abs(objCenter.y - curCenter.y) < obj.rect.height / 2))
                {
                    if (objs.Count < maxSum)
                    {
                        obj.quadTrees.Add(this);
                        objs.Add(obj);
                        return true;
                    }
                }                

                // ���Է����ӽڵ���
                for (int i = 0; i < 4; i++)
                {
                    childs[i].Add(obj);
                }
            }

            return true;
        }

        /// <summary>
        /// ɾ���ڵ�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool Remove(QuadTreeData obj)
        {
            bool result = false;
            for (int i = obj.quadTrees.Count - 1; i >= 0 ; --i)
            {
                result |= obj.quadTrees[i].objs.Remove(obj);

                if (obj.quadTrees[i].father == null)
                {
                    result |= outRangeObjs.Remove(obj);
                }
            }
            obj.quadTrees.Clear();

            return result;
        }

        /// <summary>
        /// ���½ڵ�λ��
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public static void Update(QuadTreeData obj, float posX, float posY)
        {
            if (obj.rect.x - posX < 0.000001f && obj.rect.x - posX > -0.000001f &&
                obj.rect.y - posY < 0.000001f && obj.rect.y - posY > -0.000001f)
            {
                // �仯���󣬲�����
                return;
            }

            obj.rect.x = posX;
            obj.rect.y = posY;

            // TODO ���Ż�
            var root = obj.quadTrees[0];
            while (root.father != null)
            {
                root = root.father;
            }
            Remove(obj);
            root.Add(obj);
        }

        /// <summary>
        /// ɸѡ�����ܺ��߶η�����ײ�Ľڵ�
        /// </summary>
        /// <param name="rayStart">�߶����</param>
        /// <param name="rayEnd">�߶��յ�</param>
        /// <param name="ret">�������Ľ���������뵽ret��</param>
        public void rayCast(Vector2 rayStart, Vector2 rayEnd, ref HashSet<QuadTreeData> ret)
        {
            if (!MathLib.isSegmentIntersectRect(rayStart, rayEnd, rect))
            {
                return;
            }

            for (int i = objs.Count - 1; i >= 0; --i)
            {
                if (MathLib.isSegmentIntersectRect(rayStart, rayEnd, objs[i].rect))
                {
                    ret.Add(objs[i]);
                }
            }

            if (childs == null)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                childs[i].rayCast(rayStart, rayEnd, ref ret);
            }
        }

        /// <summary>
        /// ɸѡ�����ܺ�Բ������ײ�Ľڵ�
        /// </summary>
        /// <param name="center">Բ��</param>
        /// <param name="radius">�뾶</param>
        /// <param name="ret">�������Ľ���������뵽ret��</param>
        public void searchCircleArea(Vector2 center, float radius, ref HashSet<QuadTreeData> ret)
        {
            if (!MathLib.isRectIntersectCircle(rect, center, radius))
            {
                return;
            }

            for (int i = objs.Count - 1; i >= 0; --i)
            {
                if (MathLib.isRectIntersectCircle(objs[i].rect, center, radius))
                {
                    ret.Add(objs[i]);
                }
            }

            if (childs == null)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                childs[i].searchCircleArea(center, radius, ref ret);
            }
        }

        /// <summary>
        /// ɸѡ�����ܺ���ת���η�����ײ�Ľڵ�
        /// </summary>
        /// <param name="rect">����</param>
        /// <param name="angle">��ת�Ƕ�</param>
        /// <param name="ret">�������Ľ���������뵽ret��</param>
        public void searchRectArea(Rect rect, float angle, ref HashSet<QuadTreeData> ret)
        {
            if (!MathLib.isOBBRectIntersectObbRect(rect, angle, this.rect, 0))
            {
                return;
            }

            for (int i = objs.Count - 1; i >= 0; --i)
            {
                if (MathLib.isOBBRectIntersectObbRect(rect, angle, objs[i].rect, 0))
                {
                    ret.Add(objs[i]);
                }
            }

            if (childs == null)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                childs[i].searchRectArea(rect, angle, ref ret);
            }
        }

        /// <summary>
        /// �������������ڵ������ӽ��(�Ὣ�ڵ㿴����Բ���˻�����)
        /// </summary>
        /// <param name="center">���ε�Բ��</param>
        /// <param name="direction">���η��򣨵�λʸ����</param>
        /// <param name="theta">����ɨ�Ӱ��</param>
        /// <param name="radius">���α߳�</param>
        /// <param name="ret">�������Ľ���������뵽ret��</param>
        public void searchSectorArea(Vector2 center, Vector2 direction, float theta, float radius, ref HashSet<QuadTreeData> ret)
        {
            if (!MathLib.isRectIntersectCircle(rect, center, radius))
            {
                return;
            }

            for (int i = 0; i < objs.Count; i++)
            {
                if (MathLib.IsSectorRectIntersect(center, direction, theta, radius, objs[i].rect.center, objs[i].rect.width, objs[i].rect.height))
                {
                    ret.Add(objs[i]);
                }
            }

            if (childs == null)
            {
                return;
            }

            for (int i = 0; i < 4; i++)
            {
                childs[i].searchSectorArea(center, direction, theta, radius, ref ret);
            }
        }

        private void Split()
        {
            float x = rect.x;
            float y = rect.y;
            float widthHalf = (rect.width / 2);
            float heightHalf = (rect.height / 2);
            int nextDeep = deep + 1;

            childs = new QuadTree[4];
            //��һ����
            childs[0] = new QuadTree(new Rect(x + widthHalf, y + heightHalf, widthHalf, heightHalf), 0, nextDeep, this);
            //�ڶ�����
            childs[1] = new QuadTree(new Rect(x, y + heightHalf, widthHalf, heightHalf), 1, nextDeep, this);
            //��������
            childs[2] = new QuadTree(new Rect(x, y, widthHalf, heightHalf), 2, nextDeep, this);
            //��������
            childs[3] = new QuadTree(new Rect(x + widthHalf, y, widthHalf, heightHalf), 3, nextDeep, this);

            // ���´���ǰ�ڵ�
            var temp = new List<QuadTreeData>(objs);
            objs.Clear();
            for (int i = temp.Count - 1; i >= 0 ; --i)
            {
                Remove(temp[i]);
                Add(temp[i]);
            }
        }
    }
}
