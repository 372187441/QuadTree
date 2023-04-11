# QuadTree
## 一个简单的unity四叉树管理器
### 实现了线段、圆、旋转矩形、扇形的粗略检测

## 使用方法
``` C#
var rootQuadTree = new QuadTree(rect);
var qadTreeData = new QuadTreeData();
// 添加
rootQuadTree.Add(qadTreeData);
// 移除
QuadTree.Remove(qadTreeData);
// 更新
QuadTree.Update(qadTreeData, x, y);
// 线段检测
rootQuadTree.rayCast(rayStart, rayEnd, ref list);
// 圆检测
rootQuadTree.searchCircleArea(center, radius, ref list);
// 旋转矩形检测
rootQuadTree.searchRectArea(GetColliderRect(), GetAngel(), ref list);
// 扇形检测
rootQuadTree.searchSectorArea(arcCenter, t.normalized, (arcTheta * Mathf.Deg2Rad) / 2, arcRadius, ref list);
```
