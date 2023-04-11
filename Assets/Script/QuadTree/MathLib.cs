using System;
using UnityEngine;

namespace QuadTreeSpace
{
    public class MathLib
    {
        /// <summary>
        /// 取得指定点离指定直线（两点式）最近的点
        /// </summary>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector2 closestPointOnLine(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
        {
            float a1 = linePoint2.y - linePoint1.y;
            float b1 = linePoint1.x - linePoint2.x;
            float c1 = a1 * linePoint1.x + b1 * linePoint1.y;
            float c2 = -b1 * point.x + a1 * point.y;
            float det = a1 * a1 + b1 * b1;
            float cx = 0, cy = 0;
            if (det != 0)
            {
                cx = (a1 * c1 - b1 * c2) / det;
                cy = (a1 * c2 + b1 * c1) / det;
            }
            else
            {
                cx = point.x;
                cy = point.y;
            }
            return new Vector2(cx, cy);
        }

        /// <summary>
        /// 点是否在线段上
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool isPointOnSegment(Vector2 p1, Vector2 p2, Vector2 p)
        {
            if (Mathf.Approximately((p1.x - p.x) * (p2.y - p.y), (p2.x - p.x) * (p1.y - p.y)))
            {
                return false;
            }
            if ((p.x > p1.x && p.x > p2.x) || (p.x < p1.x && p.x < p2.x))
            {
                return false;
            }
            if ((p.y > p1.y && p.y > p2.y) || (p.y < p1.y && p.y < p2.y))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 圆和线段是否相交
        /// </summary>
        /// <param name="ptStart"></param>
        /// <param name="ptEnd"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool isSegmentIntersectCircle(Vector2 ptStart, Vector2 ptEnd, Vector2 center, float radius)
        {
            float dis = (ptEnd - ptStart).magnitude;
            Vector2 d = (ptEnd - ptStart) / dis;
            Vector2 e = center - ptStart;
            float a = Vector2.Dot(d, e);
            float a2 = a * a;
            float e2 = e.sqrMagnitude;
            float r2 = radius * radius;
            if ((r2 - e2 + a2) < 0)
            {
                return false;
            }
            else
            {
                Vector2 p1 = Vector2.zero, p2 = Vector2.zero;
                float f = Mathf.Sqrt(r2 - e2 + a2);
                float t = a - f;
                if (t > 0 && t < dis)
                {
                    p1 = ptStart + t * d;

                }
                t = a + f;
                if (t > 0 && t < dis)
                {
                    p2 = ptStart + t * d;
                }
                if (p1 != p2)
                {       //p1, p2为直线与圆的交点。(注：当线段两点均在圆内时，p1和p2会有问题，应做特殊处理，但本函数仍然有效)
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// 两条线段是否相交
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="cc"></param>
        /// <param name="dd"></param>
        /// <returns></returns>
        public static bool isSegmentIntersectSegment(Vector2 aa, Vector2 bb, Vector2 cc, Vector2 dd)
        {
            if (Mathf.Max(aa.x, bb.x) < Mathf.Min(cc.x, dd.x))
            {
                return false;
            }
            if (Mathf.Max(aa.y, bb.y) < Mathf.Min(cc.y, dd.y))
            {
                return false;
            }
            if (Mathf.Max(cc.x, dd.x) < Mathf.Min(aa.x, bb.x))
            {
                return false;
            }
            if (Mathf.Max(cc.y, dd.y) < Mathf.Min(aa.y, bb.y))
            {
                return false;
            }
            if (mult(cc, bb, aa) * mult(bb, dd, aa) < 0)
            {
                return false;
            }
            if (mult(aa, dd, cc) * mult(dd, bb, cc) < 0)
            {
                return false;
            }
            return true;
        }

        static double mult(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }

        /// <summary>
        /// 线段和矩形是否相交
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool isSegmentIntersectRect(Vector2 aa, Vector2 bb, Rect rect)
        {
            float maxX = Mathf.Max(aa.x, bb.x), minX = Mathf.Min(aa.x, bb.x);
            float maxY = Mathf.Max(aa.y, bb.y), minY = Mathf.Min(aa.y, bb.y);
            if (minX >= rect.x && maxX <= rect.max.x && minY >= rect.y && maxY <= rect.max.y)
            {
                return true;
            }
            else
            {
                if (isSegmentIntersectSegment(aa, bb, rect.min, new Vector2(rect.min.x, rect.max.y))) return true;
                if (isSegmentIntersectSegment(aa, bb, new Vector2(rect.min.x, rect.max.y), rect.max)) return true;
                if (isSegmentIntersectSegment(aa, bb, rect.max, new Vector2(rect.max.x, rect.min.y))) return true;
                if (isSegmentIntersectSegment(aa, bb, new Vector2(rect.max.x, rect.min.y), rect.min)) return true;
            }
            return false;
        }

        /// <summary>
        /// 线段和旋转矩形是否相关
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="rect"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static bool isSegmentIntersectObbRect(Vector2 aa, Vector2 bb, Rect rect, float theta)
        {
            float cosTheta = Mathf.Cos(-theta * Mathf.Deg2Rad);
            float sinTheta = Mathf.Sin(-theta * Mathf.Deg2Rad);

            aa = rotatePos(aa, rect.center, cosTheta, sinTheta);
            bb = rotatePos(bb, rect.center, cosTheta, sinTheta);

            float maxX = Mathf.Max(aa.x, bb.x), minX = Mathf.Min(aa.x, bb.x);
            float maxY = Mathf.Max(aa.y, bb.y), minY = Mathf.Min(aa.y, bb.y);
            if (minX >= rect.x && maxX <= rect.max.x && minY >= rect.y && maxY <= rect.max.y)
            {
                return true;
            }
            else
            {
                if (isSegmentIntersectSegment(aa, bb, rect.min, new Vector2(rect.min.x, rect.max.y))) return true;
                if (isSegmentIntersectSegment(aa, bb, new Vector2(rect.min.x, rect.max.y), rect.max)) return true;
                if (isSegmentIntersectSegment(aa, bb, rect.max, new Vector2(rect.max.x, rect.min.y))) return true;
                if (isSegmentIntersectSegment(aa, bb, new Vector2(rect.max.x, rect.min.y), rect.min)) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断矩形和圆是否相交
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool isRectIntersectCircle(Rect rect, Vector2 p, float r)
        {
            float cx = Mathf.Abs(p.x - rect.center.x);
            float cy = Mathf.Abs(p.y - rect.center.y);

            if (cx > rect.width / 2 + r) return false;
            if (cy > rect.height / 2 + r) return false;

            if (cx <= rect.width / 2) return true;
            if (cy <= rect.height / 2) return true;

            float d = (cx - rect.width / 2) * (cx - rect.width / 2) + (cy - rect.height / 2) * (cy - rect.height / 2);
            return d < r * r;
        }

        /// <summary>
        /// 判断矩形是否包含圆（圆完全在矩形内部）
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool isRectContainsCircle(Rect rect, Vector2 p, float r)
        {
            if (!rect.Contains(new Vector2(p.x - r, p.y))) return false;
            if (!rect.Contains(new Vector2(p.x, p.y - r))) return false;
            if (!rect.Contains(new Vector2(p.x + r, p.y))) return false;
            if (!rect.Contains(new Vector2(p.x, p.y + r))) return false;
            return true;
        }

        /// <summary>
        /// 判断矩形是否包含旋转矩形（圆完全在矩形内部）
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="obbRect"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool isRectContainsOBBRect(Rect rect, Rect obbRect, float angle)
        {
            var corners1 = getObbRectCorner(obbRect, angle);
            for (int i = 0; i < 4; i++)
            {
                if (!rect.Contains(corners1[i])) return false;
            }
            return true;
        }

        /// <summary>
        /// 以某个点为锚点将P旋转指定角度
        /// </summary>
        /// <param name="p"></param>
        /// <param name="pivot"></param>
        /// <param name="cosTheta"></param>
        /// <param name="sinTheta"></param>
        /// <returns></returns>
        public static Vector2 rotatePos(Vector2 p, Vector2 pivot, float cosTheta, float sinTheta)
        {
            float x = cosTheta * (p.x - pivot.x) - sinTheta * (p.y - pivot.y) + pivot.x;
            float y = sinTheta * (p.x - pivot.x) + cosTheta * (p.y - pivot.y) + pivot.y;
            return new Vector2(x, y);
        }

        /// <summary>
        /// 判断扇形是否包含点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="angle"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static bool isPointInCircularSector(Vector2 point, Vector2 center, float radius, float angle, float degree)
        {
            Vector2 d = point - center;
            if (d.sqrMagnitude >= radius * radius) return false;

            var start = new Vector2(Mathf.Cos((degree - angle / 2) * Mathf.Deg2Rad), Mathf.Sin((degree - angle / 2) * Mathf.Deg2Rad));
            var end = new Vector2(Mathf.Cos((degree + angle / 2) * Mathf.Deg2Rad), Mathf.Sin((degree + angle / 2) * Mathf.Deg2Rad));
            return !areClockwise(start, d) && areClockwise(end, d);
        }

        private static bool areClockwise(Vector2 v1, Vector2 v2)
        {
            return -v1.x * v2.y + v1.y * v2.x > 0;
        }

        /// <summary>
        /// 计算线段与点的最短平方距离
        /// </summary>
        /// <param name="x0">线段起点</param>
        /// <param name="u">线段方向至末端点</param>
        /// <param name="x">任意点</param>
        /// <returns></returns>
        public static float SegmentPointSqrDistance(Vector2 x0, Vector2 u, Vector2 x)
        {
            float t = Vector2.Dot(x - x0, u) / u.sqrMagnitude;
            return (x - (x0 + Mathf.Clamp(t, 0, 1) * u)).sqrMagnitude;
        }

        /// <summary>
        /// 扇形与圆盘相交测试
        /// </summary>
        /// <param name="a">扇形圆心</param>
        /// <param name="u">扇形方向（单位矢量）</param>
        /// <param name="theta">扇形扫掠半角</param>
        /// <param name="l">扇形边长</param>
        /// <param name="c">圆盘圆心</param>
        /// <param name="r">圆盘半径</param>
        /// <returns></returns>
        public static bool IsSectorDiskIntersect(
            Vector2 a, Vector2 u, float theta, float l,
            Vector2 c, float r)
        {
            // 1. 如果扇形圆心和圆盘圆心的方向能分离，两形状不相交
            Vector2 d = c - a;
            float rsum = l + r;
            if (d.sqrMagnitude > rsum * rsum)
                return false;

            // 2. 计算出扇形局部空间的 p
            float px = Vector2.Dot(d, u);
            float py = Mathf.Abs(Vector2.Dot(d, new Vector2(-u.y, u.x)));

            // 3. 如果 p_x > ||p|| cos theta，两形状相交
            if (px > d.magnitude * Mathf.Cos(theta))
                return true;

            // 4. 求左边线段与圆盘是否相交
            Vector2 q = l * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Vector2 p = new Vector2(px, py);
            return SegmentPointSqrDistance(Vector2.zero, q, p) <= r * r;
        }

        /// <summary>
        /// 扇形与矩形相交测试
        /// </summary>
        /// <param name="a">扇形圆心</param>
        /// <param name="u">扇形方向（单位矢量）</param>
        /// <param name="theta">扇形扫掠半角</param>
        /// <param name="l">扇形边长</param>
        /// <param name="c">圆盘圆心</param>
        /// <param name="r">矩形中心</param>
        /// <param name="w">矩形宽度</param>
        /// <param name="h">矩形高度</param>
        public static bool IsSectorRectIntersect(
                   Vector2 a, Vector2 u, float theta, float l,
                          Vector2 r, float w, float h)
        {
            // 1. 如果扇形圆心和矩形圆心的方向能分离，两形状不相交
            Vector2 d = r - a;
            float rsum = l + Mathf.Sqrt(w * w + h * h) / 2;
            if (d.sqrMagnitude > rsum * rsum)
                return false;
            // 2. 计算出扇形局部空间的 p
            float px = Vector2.Dot(d, u);
            float py = Mathf.Abs(Vector2.Dot(d, new Vector2(-u.y, u.x)));
            // 3. 如果 p_x > ||p|| cos theta，两形状相交
            if (px > d.magnitude * Mathf.Cos(theta))
                return true;
            // 4. 求左边线段与矩形是否相交
            Vector2 q = l * new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Vector2 p = new Vector2(px, py);
            return SegmentPointSqrDistance(Vector2.zero, q, p) <= w * w / 4 + h * h / 4;
        }

        /// <summary>
        /// 将OBB包围盒转成AABB包围盒
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Rect convertObb2AABB(Rect rect, float angle)
        {
            var points = getObbRectCorner(rect, angle);
            float minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = -1, maxY = -1;
            for (int i = 0; i < 4; i++)
            {
                Vector2 p = points[i];
                minX = p.x < minX ? p.x : minX;
                maxX = p.x > maxX ? p.x : maxX;
                minY = p.y < minY ? p.y : minY;
                maxY = p.y > maxY ? p.y : maxY;
            }
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// 判断旋转矩形是否包含点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="theta"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool isOBBRectContainsPoint(Rect rect, float theta, Vector2 point)
        {
            float cosTheta = Mathf.Cos(-theta * Mathf.Deg2Rad);
            float sinTheta = Mathf.Sin(-theta * Mathf.Deg2Rad);

            Vector2 relativePoint = rotatePos(point, rect.center, cosTheta, sinTheta);
            return rect.Contains(relativePoint);
        }

        /// <summary>
        /// 判断旋转矩形是否与圆相交
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="theta"></param>
        /// <param name="point"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool isOBBRectIntersectCircle(Rect rect, float theta, Vector2 point, float radius)
        {
            float cosTheta = Mathf.Cos(-theta * Mathf.Deg2Rad);
            float sinTheta = Mathf.Sin(-theta * Mathf.Deg2Rad);

            Vector2 relativePoint = rotatePos(point, rect.center, cosTheta, sinTheta);
            return isRectIntersectCircle(rect, relativePoint, radius);
        }

        /// <summary>
        /// 取得旋转矩形的四个顶点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static Corner getObbRectCorner(Rect rect, float theta)
        {
            float cosTheta = Mathf.Cos(theta * Mathf.Deg2Rad);
            float sinTheta = Mathf.Sin(theta * Mathf.Deg2Rad);

            Vector2 X = new Vector2(cosTheta, sinTheta);
            Vector2 Y = new Vector2(-sinTheta, cosTheta);
            X = X * rect.width / 2;
            Y = Y * rect.height / 2;

            var corner = new Corner();
            corner[0] = rect.center - X - Y;
            corner[1] = rect.center + X - Y;
            corner[2] = rect.center + X + Y;
            corner[3] = rect.center - X + Y;
            return corner;
        }

        /// <summary>
        /// 旋转矩形之间是否有碰撞
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="theta1"></param>
        /// <param name="rect2"></param>
        /// <param name="theta2"></param>
        /// <returns></returns>
        public static bool isOBBRectIntersectObbRect(Rect rect1, float theta1, Rect rect2, float theta2)
        {
            var corners1 = getObbRectCorner(rect1, theta1);
            var corners2 = getObbRectCorner(rect2, theta2);

            return isOBBRectIntersectObbRect(corners1, corners2);
        }

        /// <summary>
        /// 旋转矩形之间是否有碰撞
        /// </summary>
        /// <param name="corners1"></param>
        /// <param name="corners2"></param>
        /// <returns></returns>
        public static bool isOBBRectIntersectObbRect(Corner corners1, Corner corners2)
        {
            Axis axis1 = new Axis();
            axis1[0] = corners1[1] - corners1[0];
            axis1[1] = corners1[3] - corners1[0];
            var origin1 = new Origin();
            for (int i = 0; i < 2; i++)
            {
                axis1[i] /= axis1[i].sqrMagnitude;
                origin1[i] = Vector2.Dot(corners1[0], axis1[i]);
            }

            Axis axis2 = new Axis();
            axis2[0] = corners2[1] - corners2[0];
            axis2[1] = corners2[3] - corners2[0];
            var origin2 = new Origin();
            for (int i = 0; i < 2; i++)
            {
                axis2[i] /= axis2[i].sqrMagnitude;
                origin2[i] = Vector2.Dot(corners2[0], axis2[i]);
            }

            return overlapsWay(axis1, origin1, corners2) && overlapsWay(axis2, origin2, corners1);
        }

        private static bool overlapsWay(Axis axis1, Origin origin1, Corner corners2)
        {
            for (int i = 0; i < 2; i++)
            {
                float t = Vector2.Dot(corners2[0], axis1[i]);
                float tMin = t;
                float tMax = t;

                for (int j = 1; j < 4; j++)
                {
                    t = Vector2.Dot(corners2[j], axis1[i]);
                    if (t < tMin)
                    {
                        tMin = t;
                    }
                    else if (t > tMax)
                    {
                        tMax = t;
                    }
                }

                if (tMin > 1 + origin1[i] || tMax < origin1[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 计算两条线段的交点。若无交点则返回Vector2.zero
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Vector2 isSegmentIntersectSegment2(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            // 三角形abc 面积的2倍  
            float area_abc = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);

            // 三角形abd 面积的2倍  
            float area_abd = (a.x - d.x) * (b.y - d.y) - (a.y - d.y) * (b.x - d.x);

            // 面积符号相同则两点在线段同侧,不相交 (对点在线段上的情况,本例当作不相交处理);  
            if (area_abc * area_abd >= 0)
            {
                return Vector2.zero;
            }

            // 三角形cda 面积的2倍  
            float area_cda = (c.x - a.x) * (d.y - a.y) - (c.y - a.y) * (d.x - a.x);
            // 三角形cdb 面积的2倍  
            // 注意: 这里有一个小优化.不需要再用公式计算面积,而是通过已知的三个面积加减得出.  
            float area_cdb = area_cda + area_abc - area_abd;
            if (area_cda * area_cdb >= 0)
            {
                return Vector2.zero;
            }

            //计算交点坐标  
            float t = area_cda / (area_abd - area_abc);
            float dx = t * (b.x - a.x),
                dy = t * (b.y - a.y);
            return new Vector2(a.x + dx, a.y + dy);
        }

        /// <summary>
        /// 计算矩形与线段相交最近的点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="angle"></param>
        /// <param name="rayStart"></param>
        /// <param name="rayEnd"></param>
        /// <returns></returns>
        public static Vector2 RaycastObbRect(Rect rect, float angle, Vector2 rayStart, Vector2 rayEnd)
        {
            var c = getObbRectCorner(rect, angle);
            Vector2 intersectionPoint = Vector2.zero;
            float minSqrDis = Int32.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                int ip = i < 3 ? i + 1 : 0;
                Vector2 p = isSegmentIntersectSegment2(c[i], c[ip], rayStart, rayEnd);
                float dis = (rayStart - p).sqrMagnitude;
                if (p != Vector2.zero && minSqrDis > dis)
                {
                    minSqrDis = dis;
                    intersectionPoint = p;
                }
            }
            return intersectionPoint;
        }
    }
}

