using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTreeSpace
{
    /// <summary>
    /// 四叉树节点
    /// </summary>
    public class QuadTree
    {
        /// <summary> 当自身存储数量达到多少的时候开始划分 </summary>
        public readonly static int maxSum = 4;
        /// <summary> 最大深度 </summary>
        public readonly static int maxDeep = 4;

        /// <summary> 父节点 </summary>
        public QuadTree father;
        /// <summary> 四个子区域 </summary>
        public QuadTree[] childs;
        /// <summary> 当前区域存储的节点 </summary>
        public List<QuadTreeData> objs;
        /// <summary> 当前该节点在父节点的位置 </summary>
        public int index;
        /// <summary> 当前该节点代表的的深度 </summary>
        public int deep;
        /// <summary> 自己的位置信息 </summary>
        public Rect rect;
        /// <summary> 超出范围的节点，存储在公共区域 </summary>
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
        /// 添加节点
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Add(QuadTreeData obj)
        {
            if (obj.rect.xMin > rect.xMax || obj.rect.xMax < rect.xMin || obj.rect.yMin > rect.yMax || obj.rect.yMax < rect.yMin)
            {
                // 没有任何交集
                if (father == null)
                {
                    // 超出四叉树的范围，放入专用位置
                    obj.quadTrees.Add(this);
                    outRangeObjs.Add(obj);
                }
                return false;
            }

            //如果子节点不存在 说明还没有分割
            if (childs == null)
            {
                obj.quadTrees.Add(this);
                objs.Add(obj);
                // 超过阈值，开始分割
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
                // 如果在分界轴上, 且数量上不超过阈值
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

                // 尝试放入子节点下
                for (int i = 0; i < 4; i++)
                {
                    childs[i].Add(obj);
                }
            }

            return true;
        }

        /// <summary>
        /// 删除节点
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
        /// 更新节点位置
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public static void Update(QuadTreeData obj, float posX, float posY)
        {
            if (obj.rect.x - posX < 0.000001f && obj.rect.x - posX > -0.000001f &&
                obj.rect.y - posY < 0.000001f && obj.rect.y - posY > -0.000001f)
            {
                // 变化不大，不处理
                return;
            }

            obj.rect.x = posX;
            obj.rect.y = posY;

            // TODO 待优化
            var root = obj.quadTrees[0];
            while (root.father != null)
            {
                root = root.father;
            }
            Remove(obj);
            root.Add(obj);
        }

        /// <summary>
        /// 筛选出可能和线段发生碰撞的节点
        /// </summary>
        /// <param name="rayStart">线段起点</param>
        /// <param name="rayEnd">线段终点</param>
        /// <param name="ret">搜索到的结果将被插入到ret里</param>
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
        /// 筛选出可能和圆发生碰撞的节点
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="radius">半径</param>
        /// <param name="ret">搜索到的结果将被插入到ret里</param>
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
        /// 筛选出可能和旋转矩形发生碰撞的节点
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="ret">搜索到的结果将被插入到ret里</param>
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
        /// 搜索扇形区域内的所有子结点(会将节点看做成圆形退化处理)
        /// </summary>
        /// <param name="center">扇形的圆心</param>
        /// <param name="direction">扇形方向（单位矢量）</param>
        /// <param name="theta">扇形扫掠半角</param>
        /// <param name="radius">扇形边长</param>
        /// <param name="ret">搜索到的结果将被插入到ret里</param>
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
            //第一象限
            childs[0] = new QuadTree(new Rect(x + widthHalf, y + heightHalf, widthHalf, heightHalf), 0, nextDeep, this);
            //第二象限
            childs[1] = new QuadTree(new Rect(x, y + heightHalf, widthHalf, heightHalf), 1, nextDeep, this);
            //第三象限
            childs[2] = new QuadTree(new Rect(x, y, widthHalf, heightHalf), 2, nextDeep, this);
            //第四象限
            childs[3] = new QuadTree(new Rect(x + widthHalf, y, widthHalf, heightHalf), 3, nextDeep, this);

            // 重新处理当前节点
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
