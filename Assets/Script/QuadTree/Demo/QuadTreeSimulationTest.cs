using UnityEngine;
using QuadTreeSpace;

public class QuadTreeSimulationTest : MonoBehaviour {
	// 物体
	public GameObject prefab;
	public int objectNum = 500;
    private QuadTree rootQuadTree;
    private RectangleNode[] rectangleNodes = new RectangleNode[0];
    // 范围
    public Rect rect;
	// 检测
	public CircleCheck circleCheck;
	public RectangleCheck rectangleCheck;
	public SectorCheck sectorCheck;
	public SegmentCheck segmentCheck;


    void Start () {
		rootQuadTree = new QuadTree(rect);

        if (prefab != null)
        {
            rectangleNodes = new RectangleNode[objectNum];
            for (int i = 0; i < objectNum; i++)
            {
                GameObject go = Instantiate(prefab);
                rectangleNodes[i] = go.AddComponent<RectangleNode>();
                rectangleNodes[i].Init(rootQuadTree, rect.xMin, rect.xMax, rect.yMin, rect.yMax);
            }
        }        

        circleCheck?.Init(rootQuadTree);
        rectangleCheck?.Init(rootQuadTree);
        sectorCheck?.Init(rootQuadTree);
        segmentCheck?.Init(rootQuadTree);
    }

	void Update(){
		foreach(var ball in rectangleNodes)
        {
			ball.Highlight(false);
		}
		circleCheck?.ManualUpdate();
        rectangleCheck?.ManualUpdate();
        sectorCheck?.ManualUpdate();
        segmentCheck?.ManualUpdate();
    }
	
	void OnGUI () {
		if(GUI.Button(new Rect(0,0,100,20), "MoveTest")){
			foreach(var ball in rectangleNodes){
				ball.move = !ball.move;
			}
		}		
	}

#if UNITY_EDITOR
	void drawRect(Rect rect){
		Gizmos.DrawLine(new Vector3(rect.min.x,0, rect.min.y), new Vector3(rect.max.x,0, rect.min.y));
        Gizmos.DrawLine(new Vector3(rect.max.x,0, rect.min.y), new Vector3(rect.max.x,0, rect.max.y));
        Gizmos.DrawLine(new Vector3(rect.max.x,0, rect.max.y), new Vector3(rect.min.x,0, rect.max.y));
        Gizmos.DrawLine(new Vector3(rect.min.x,0, rect.max.y), new Vector3(rect.min.x,0, rect.min.y));
	}

	void drawObbRect(Rect rect, float theta){
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

	void drawQuadTree(QuadTree q){
		if(q == null) return;

		drawRect(q.rect);
		if(q.childs != null){
			foreach(QuadTree node in q.childs)
            {
				drawQuadTree(node);
			}
		}
	}

	void OnDrawGizmos()
    {
        drawQuadTree(rootQuadTree);

		// 白色矩形框，和其他形状相交检测
        Gizmos.color = Color.white;
		Rect rect = new Rect(-50, -100, 100, 200);
		float angel = 30;

        if (circleCheck != null)
        {
            if (MathLib.isOBBRectIntersectCircle(rect, angel, circleCheck.center, circleCheck.radius))
            {
                Gizmos.color = Color.red;
            }
            // 圆心在矩形上最近的点
            Vector2 newCenter = MathLib.rotatePos(circleCheck.center, rect.center, Mathf.Cos(-angel * Mathf.Deg2Rad), Mathf.Sin(-angel * Mathf.Deg2Rad));
            Vector2 intersectionPoint = new Vector2(
                Mathf.Max(rect.x, Mathf.Min(newCenter.x, rect.x + rect.width)),
                Mathf.Max(rect.y, Mathf.Min(newCenter.y, rect.y + rect.height)));
            intersectionPoint = MathLib.rotatePos(intersectionPoint, rect.center, Mathf.Cos(angel * Mathf.Deg2Rad), Mathf.Sin(angel * Mathf.Deg2Rad));
            UnityEditor.Handles.DrawSolidDisc(new Vector3(intersectionPoint.x, 0, intersectionPoint.y), Vector3.up, 2);
        }


        if (rectangleCheck != null)
		{
            // 和矩形相交检测
            if (MathLib.isOBBRectIntersectObbRect(rectangleCheck.GetColliderRect(), rectangleCheck.GetAngel(), rect, angel))
            {
                Gizmos.color = Color.green;
            }
        }
        
        if (segmentCheck != null)
        {
            // 和线段相交检测
            if (MathLib.isSegmentIntersectObbRect(segmentCheck.rayStart, segmentCheck.rayEnd, rect, angel))
            {
                Gizmos.color = Color.blue;
                Vector2 po = MathLib.RaycastObbRect(rect, 30, segmentCheck.rayStart, segmentCheck.rayEnd);
                UnityEditor.Handles.DrawSolidDisc(new Vector3(po.x, 0, po.y), Vector3.up, 2);
            }
        }

        drawObbRect(rect, angel);
    }
#endif
}
