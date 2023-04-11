using UnityEngine;

namespace QuadTreeSpace
{
    public class RectangleNode : MonoBehaviour
    {
        public QuadTreeData qadTreeData;
        private QuadTree rootQuadTree;

        public bool move = false;
        private Renderer _renderer;
        private TextMesh _textMesh;

        private float boundMinX;
        private float boundMaxX;
        private float boundMinY;
        private float boundMaxY;

        private float speedX = 10f;
        private float speedY = 10f;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _textMesh = transform.Find("text").GetComponent<TextMesh>();
            RandSpeed();

            if (Random.Range(0, 100) > 50)
            {
                speedX = -speedX;
            }
            if (Random.Range(0, 100) > 50)
            {
                speedY = -speedY;
            }
        }

        private bool isHighlight = false;
        public void Highlight(bool b)
        {
            if (isHighlight == b)
                return;

            isHighlight = b;
            if (b)
                _renderer.material.color = Color.red;
            else
                _renderer.material.color = Color.white;
        }

        public void Init(QuadTree rootQuadTree, float boundMinX, float boundMaxX, float boundMinY, float boundMaxY)
        {
            this.rootQuadTree = rootQuadTree;
            this.boundMinX = boundMinX;
            this.boundMaxX = boundMaxX;
            this.boundMinY = boundMinY;
            this.boundMaxY = boundMaxY;

            UpdateQuadData();
            transform.localScale = new Vector3(qadTreeData.rect.width / 10f, 1, qadTreeData.rect.height / 10f);
            transform.position = new Vector3(qadTreeData.rect.center.x, 0, qadTreeData.rect.center.y);
            _textMesh.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.z, 1);
        }

        private void UpdateQuadData()
        {
            if (qadTreeData == null)
            {
                float width = Random.Range(1f, 20f);
                float height = Random.Range(1f, 20f);
                qadTreeData = new QuadTreeData();
                qadTreeData.bindNode = this;
                qadTreeData.rect = new Rect(
                    Random.Range(boundMinX - 100, boundMaxX - width + 100),
                    Random.Range(boundMinY - 100, boundMaxY - height + 100),
                    width, height);
                rootQuadTree.Add(qadTreeData);
                _textMesh.text = GetPlace(qadTreeData);
            }
        }

        void Update()
        {
            if (!move)
                return;

            float x = qadTreeData.rect.x + Time.deltaTime * speedX;
            float y = qadTreeData.rect.y + Time.deltaTime * speedY;
            if (speedX > 0)
            {
                if (x + qadTreeData.rect.width > boundMaxX)
                {
                    RandSpeed();
                    speedX = -Mathf.Abs(speedX);
                }
            }
            else
            {
                if (x < boundMinX)
                {
                    RandSpeed();
                    speedX = Mathf.Abs(speedX);
                }
            }

            if (speedY > 0)
            {
                if (y + qadTreeData.rect.height > boundMaxY)
                {
                    RandSpeed();
                    speedY = -Mathf.Abs(speedY);
                }
            }
            else
            {
                if (y < boundMinY)
                {
                    RandSpeed();
                    speedY = Mathf.Abs(speedY);
                }
            }

            QuadTree.Update(qadTreeData, x, y);
            //qadTreeData.quadTrees[0].Update(qadTreeData, x, y);
            transform.position = new Vector3(qadTreeData.rect.center.x, 0, qadTreeData.rect.center.y);
            _textMesh.text = GetPlace(qadTreeData);
        }

        private string GetPlace(QuadTreeData _qadTreeData)
        {
            if (_qadTreeData.quadTrees.Count == 0)
                return "";

            string place = "";
            for (int i = 0; i < _qadTreeData.quadTrees.Count; i++)
            {
                place += GetPlace(_qadTreeData.quadTrees[i]);
                place += '\n';
            }
            return place.Substring(0, place.Length - 1);
        }
        private string GetPlace(QuadTree _qadTree)
        {
            if (_qadTree.father == null)
            {
                return "T";
            }else
            {
                return GetPlace(_qadTree.father) + "-" + _qadTree.index.ToString();
            }
        }

        private void RandSpeed()
        {
            speedX = Random.Range(-300, 300);
            speedY = Random.Range(-300, 300);

            if (speedX == 0 && speedY == 0)
                speedX = 1;

            float sqrt = Mathf.Sqrt(speedX * speedX + speedY * speedY);
            speedX = speedX * 30 / sqrt;
            speedY = speedY * 30 / sqrt;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (qadTreeData != null)
            {
                drawRect(qadTreeData.rect);
            }
        }

        void drawRect(Rect rect)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(rect.min.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.min.y));
            Gizmos.DrawLine(new Vector3(rect.max.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.max.y));
            Gizmos.DrawLine(new Vector3(rect.max.x, 0, rect.max.y), new Vector3(rect.min.x, 0, rect.max.y));
            Gizmos.DrawLine(new Vector3(rect.min.x, 0, rect.max.y), new Vector3(rect.min.x, 0, rect.min.y));
        }

        public QuadTreeData GetQuadTreeData()
        {
            return qadTreeData;
        }

        public QuadTree GetRootQuadTree()
        {
            return rootQuadTree;
        }
#endif
    }
}

