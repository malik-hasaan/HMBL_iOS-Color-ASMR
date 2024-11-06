using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using ScratchCardAsset.Core;
using ScratchCardAsset.Core.Data;

namespace ScratchCardAsset
{
    public class ScratchCard : MonoBehaviour
    {
        public enum Quality
        {
            Low = 4,
            Medium = 2,
            High = 1
        }

        public enum ScratchMode
        {
            Erase,
            Restore
        }

        public Camera MainCamera;
        public Transform Surface;
        public Quality RenderTextureQuality = Quality.High;
        public Material Eraser;
        public Material Progress;
        public Material ScratchSurface;
        public RenderTexture RenderTexture;
        public Vector2 BrushScale = Vector2.one;
        public GameObject ToolTip;
        public bool InputEnabled = true;
        public bool IsScratching;

        public bool useChangingScale, useGivenBrushScale;

        [SerializeField]
        private ScratchMode _mode = ScratchMode.Erase;

        public ScratchMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                if (Eraser != null)
                {
                    var blendOp = _mode == ScratchMode.Erase ? (int)BlendOp.Add : (int)BlendOp.ReverseSubtract;
                    Eraser.SetInt(BlendOpShaderParam, blendOp);
                }
            }
        }

        public bool IsScratched
        {
            get
            {
                if (cardRenderer != null)
                {
                    return cardRenderer.IsScratched;
                }

                return false;
            }
            private set { cardRenderer.IsScratched = value; }
        }

        private ScratchCardRenderer cardRenderer;
        public ScratchCardInput cardInput;
        private Triangle triangle;
        private SpriteRenderer surfaceSpriteRenderer;
        private MeshFilter surfaceMeshFilter;
        private Renderer surfaceRenderer;
        private RectTransform surfaceRectTransform;
        private Vector2 boundsSize;
        private Vector2 imageSize;
        private bool isCanvasOverlay;
        private bool isFirstFrame = true;
        private int lastFrameId;

        private const string BlendOpShaderParam = "_BlendOpValue";
        private const string SourceTexProperty = "_SourceTex";

        // Custom fields
        private Vector2 lastPos, currentPos;
        private Vector2 firstScale, maxScale = new Vector2(1.5f, 1.5f);
        private bool firstTime = true;
        private float minDistance = 0.3f;

        #region MonoBehaviour Methods

        void Start()
        {
            Init();
            FillInstantly();
            firstScale = BrushScale;
        }
        public void SetStart()
        {
            Init();
            FillInstantly();
            firstScale = BrushScale;
        }

        void OnDestroy()
        {
            try
            {
                if (RenderTexture != null && RenderTexture.IsCreated())
                {
                    RenderTexture.Release();
                    Destroy(RenderTexture);
                }

                cardRenderer.Release();
            }
            catch { }
        }



        void Update()
        {
            if (lastFrameId == Time.frameCount)
                return;


            if (isFirstFrame)
            {
                if (cardRenderer == null)
                {
                    cardRenderer = new ScratchCardRenderer(this);
                    cardRenderer.SetImageSize(imageSize);
                    cardRenderer.CreateRenderTexture();
                    return;
                }

                cardRenderer.FillRenderTextureWithColor(Color.clear);
                isFirstFrame = false;
            }

            cardInput.Update();
            if (IsScratching)
            {
                currentPos = Input.mousePosition;
                //CardRendererHelper.instance.meshCreator.currentPos = currentPos;
                cardInput.Scratch();

            }
            else
            {
                cardRenderer.IsScratched = false;
            }

            lastFrameId = Time.frameCount;
        }

        #endregion

        #region Initializaion

        private void Init()
        {
            GetScratchBounds();
            InitVariables();
            InitTriangle();
        }

        /// <summary>
        /// Saves scratch card renderer component bounds
        /// </summary>
        private void GetScratchBounds()
        {
            surfaceRenderer = Surface.GetComponent<Renderer>();
            if (surfaceRenderer != null)
            {
                var sourceTexture = surfaceRenderer.sharedMaterial.mainTexture;
                imageSize = new Vector2(sourceTexture.width, sourceTexture.height);
                surfaceMeshFilter = Surface.GetComponent<MeshFilter>();
                if (surfaceMeshFilter != null)
                {
                    boundsSize = surfaceMeshFilter != null && !MainCamera.orthographic
                        ? surfaceMeshFilter.sharedMesh.bounds.size
                        : surfaceRenderer.bounds.size;
                }
                else
                {
                    surfaceSpriteRenderer = Surface.GetComponent<SpriteRenderer>();
                    boundsSize = surfaceSpriteRenderer != null && !MainCamera.orthographic
                        ? surfaceSpriteRenderer.sprite.bounds.size
                        : surfaceRenderer.bounds.size;
                }
            }
            else
            {
                surfaceRectTransform = Surface.GetComponent<RectTransform>();
                if (surfaceRectTransform != null)
                {
                    var canvas = Surface.GetComponentInParent<Canvas>();
                    if (canvas != null)
                    {
                        isCanvasOverlay = canvas.renderMode == RenderMode.ScreenSpaceOverlay;
                    }
                    var rect = surfaceRectTransform.rect;
                    imageSize = new Vector2(rect.width, rect.height);
                    boundsSize = MainCamera.orthographic || isCanvasOverlay
                        ? Vector2.Scale(rect.size, surfaceRectTransform.lossyScale)
                        : imageSize;
                }
                else
                {
                    Debug.LogError("Can't find Renderer or RectTransform Component!");
                }
            }
        }

        private void InitVariables()
        {
            cardInput = new ScratchCardInput(this);
            cardInput.OnScratchStart -= OnScratchStart;
            cardInput.OnScratchStart += OnScratchStart;
            cardInput.OnScratchHole -= OnScratchHole;
            cardInput.OnScratchHole += OnScratchHole;
            cardInput.OnScratchLine -= OnScratchLine;
            cardInput.OnScratchLine += OnScratchLine;
            cardInput.OnScratch -= GetScratchPosition;
            cardInput.OnScratch += GetScratchPosition;
            if (cardRenderer != null)
            {
                cardRenderer.Release();
            }

            cardRenderer = new ScratchCardRenderer(this);
            cardRenderer.SetImageSize(imageSize);
            cardRenderer.CreateRenderTexture();
        }

        private void InitTriangle()
        {
            //bottom left
            var position0 = new Vector3(-boundsSize.x / 2f, -boundsSize.y / 2f, 0);
            var uv0 = Vector2.zero;
            //upper left
            var position1 = new Vector3(-boundsSize.x / 2f, boundsSize.y / 2f, 0);
            var uv1 = Vector2.up;
            //upper right
            var position2 = new Vector3(boundsSize.x / 2f, boundsSize.y / 2f, 0);
            var uv2 = Vector2.one;
            triangle = new Triangle(position0, position1, position2, uv0, uv1, uv2);
        }

        #endregion

        void OnScratchStart()
        {
            cardRenderer.IsScratched = false;
        }

        void OnScratchHole(Vector2 position)
        {
            if (LevelManager.instance.isPaint == false)
            {
                cardRenderer.DefaultScratchHole1(position);
            }
            else
            {
                StartCoroutine(cardRenderer.Filling(position));
            }
        }

        void OnScratchLine(Vector2 start, Vector2 end)
        {

            if (LevelManager.instance.isPaint == false)
            {
                cardRenderer.ScratchLine1(start, end);
            }
            else
            {
                StartCoroutine(cardRenderer.FillingLine(start, end));
            }

        }

        Vector2 GetScratchPosition(Vector2 position)
        {
            var scratchPosition = Vector2.zero;

            if (MainCamera.orthographic || isCanvasOverlay)
            {
                var clickPosition = ToolTip.transform.position;
                var lossyScale = Surface.lossyScale;
                var clickLocalPosition = Vector2.Scale(Surface.InverseTransformPoint(clickPosition), lossyScale) + boundsSize / 2f;
                var pixelsPerInch = new Vector2(imageSize.x / boundsSize.x / lossyScale.x, imageSize.y / boundsSize.y / lossyScale.y);
                scratchPosition = Vector2.Scale(Vector2.Scale(clickLocalPosition, lossyScale), pixelsPerInch);
            }

            else
            {
                var plane = new Plane(Surface.forward, Surface.position);
                var ray = MainCamera.ScreenPointToRay(ToolTip.transform.position);
                float enter;

                if (plane.Raycast(ray, out enter))
                {
                    var point = ray.GetPoint(enter);
                    var pointLocal = Surface.InverseTransformPoint(point);
                    var uv = triangle.GetUV(pointLocal);
                    scratchPosition = new Vector2(uv.x * imageSize.x, uv.y * imageSize.y);
                }
            }

            return scratchPosition;
        }

        public ScratchCardInput GetCardInput()
        {
            return cardInput;
        }

        public void StopColorCoroutine()
        {
            StopCoroutine(cardRenderer.FillingLine(Vector2.zero, Vector2.zero));

            StopCoroutine(cardRenderer.Filling(Vector2.zero));
        }

        #region Public Methods

        /// <summary>
        /// Fills RenderTexture with white color (100% scratched surface)
        /// </summary>
        public void FillInstantly()
        {
            cardRenderer.FillRenderTextureWithColor(Color.white);
            IsScratched = true;
        }

        /// <summary>
        /// Fills RenderTexture with clear color (0% scratched surface)
        /// </summary>
        public void ClearInstantly()
        {
            cardRenderer.FillRenderTextureWithColor(Color.clear);
            IsScratched = true;
        }

        public void Clearcurrent()
        {
            cardRenderer.FillRenderTextureWithColor(Color.white);
            IsScratched = true;
        }

        /// <summary>
        /// Clears scratched surface in next frame
        /// </summary>
        public void Clear()
        {
            isFirstFrame = true;
            IsScratched = true;
        }

        /// <summary>
        /// Recreates RenderTexture and clears it in next frame
        /// </summary>
        public void ResetRenderTexture()
        {
            cardRenderer.CreateRenderTexture();
            isFirstFrame = true;
            IsScratched = true;
        }

        /// <summary>
        /// Scratches hole
        /// </summary>
        /// <param name="position"></param>
        public void ScratchHole(Vector2 position)
        {
        
            if (LevelManager.instance.isPaint == false)
            {
           
                StartCoroutine(cardRenderer.Filling(position));
            }
            else
            {
                cardRenderer.ScratchHole(position);
            }
      
            IsScratched = true;
        }


        /// <summary>
        /// Scratches line
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        public void ScratchLine(Vector2 startPosition, Vector2 endPosition)
        {
       
            if (LevelManager.instance.isPaint == false)
            {
                StartCoroutine(cardRenderer.FillingLine(startPosition, endPosition));
            }
            else
            {
                cardRenderer.ScratchLine1(startPosition, endPosition);
            }
           
            IsScratched = true;
        }


        /// <summary>
        /// Returns scratch texture
        /// </summary>
        /// <returns></returns>
        public Texture2D GetScratchTexture()
        {
            var previousRenderTexture = RenderTexture.active;
            var texture2D = new Texture2D(RenderTexture.width, RenderTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = RenderTexture;
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0, false);
            texture2D.Apply();
            RenderTexture.active = previousRenderTexture;
            return texture2D;
        }

        /// <summary>
        /// Set new ScratchTexture
        /// </summary>
        /// <param name="texture"></param>
        public void SetScratchTexture(Texture2D texture)
        {
            Init();
            ClearInstantly();
            Graphics.Blit(texture, RenderTexture);
            if (Progress.HasProperty(SourceTexProperty))
            {
                Progress.SetTexture(SourceTexProperty, texture);
            }
            ScratchSurface.mainTexture = texture;
            IsScratched = true;
        }

        public IEnumerator ChangingHoleScale(Vector2 position)
        {
            float difference = 0.18f;
            Vector2 newScale = firstScale;
            while (newScale != maxScale)
            {
                newScale = Vector2.Lerp(newScale, maxScale, difference + difference);
                cardRenderer.ScratchHole(position, newScale);
                IsScratched = true;
                yield return new WaitForSeconds(0.001f);
            }
            yield return null;
        }

        public IEnumerator ChangingLineScale(Vector2 startPosition, Vector2 endPosition)
        {
            float difference = 0.05f;
            Vector2 newScale = firstScale;
            while (newScale != maxScale)
            {
                newScale = Vector2.Lerp(newScale, maxScale, difference + difference);
                cardRenderer.ModifiedScratchLine(startPosition, endPosition, newScale);
                IsScratched = true;
                yield return new WaitForSeconds(0.01f);
            }
            yield return null;
        }

        public ScratchCardRenderer GetCardrenderer()
        {
            return cardRenderer;
        }

        #endregion
    }
}