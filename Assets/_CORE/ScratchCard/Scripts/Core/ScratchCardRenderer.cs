using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset.Tools;
using UnityEngine;
using UnityEngine.Rendering;

namespace ScratchCardAsset.Core
{
    /// <summary>
    /// Draws holes and lines into RenderTexture
    /// </summary>
    public class ScratchCardRenderer
    {
        public bool IsScratched, useChangingHole;

        private ScratchCard scratchCard;
        private Mesh meshHole;
        private Mesh meshLine;
        private CommandBuffer commandBuffer;
        private RenderTargetIdentifier rti;
        private Bounds localBounds;
        private Vector2 imageSize;

        private const string MaskTexProperty = "_MaskTex";
        private const string MainTexProperty = "_MainTex";
        private const string SourceTexProperty = "_SourceTex";

        public ScratchCardRenderer(ScratchCard card)
        {
            scratchCard = card;
            localBounds = new Bounds(Vector2.one / 2f, Vector2.one);
            commandBuffer = new CommandBuffer { name = "ScratchCardRenderer" };
            meshHole = MeshGenerator.GenerateQuad(Vector3.zero, Vector2.zero);
        }

        public Mesh GetMeshHole()
        {
            return meshHole;
        }
        public Mesh GetMeshLine()
        {
            return meshLine;
        }
        public CommandBuffer GetCommandBuffer()
        {
            return commandBuffer;
        }
        public RenderTargetIdentifier GetRenderTarget()
        {
            return rti;
        }
        public Material GetMaterial()
        {
            return scratchCard.Eraser;
        }
        public Rect GetPositionRect(Vector2 position)
        {
            return new Rect(
                (position.x - 0.5f * scratchCard.Eraser.mainTexture.width * (scratchCard.BrushScale.x)) / imageSize.x,
                (position.y - 0.5f * scratchCard.Eraser.mainTexture.height * (scratchCard.BrushScale.y)) / imageSize.y,
                scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x / imageSize.x,
                scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y / imageSize.y);
        }

        public void Release()
        {
            if (commandBuffer != null)
            {
                commandBuffer.Release();
            }
            if (meshHole != null)
            {
                Object.Destroy(meshHole);
            }
            if (meshLine != null)
            {
                Object.Destroy(meshLine);
            }
        }

        /// <summary>
        /// Creates RenderTexture
        /// </summary>
        public void CreateRenderTexture()
        {
            var renderTextureSize = new Vector2(imageSize.x / (float)scratchCard.RenderTextureQuality, imageSize.y / (float)scratchCard.RenderTextureQuality);
            scratchCard.RenderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 0, RenderTextureFormat.ARGB32);
            scratchCard.ScratchSurface.SetTexture(MaskTexProperty, scratchCard.RenderTexture);
            scratchCard.Progress.SetTexture(MainTexProperty, scratchCard.RenderTexture);
            if (scratchCard.Progress.HasProperty(SourceTexProperty))
            {
                scratchCard.Progress.SetTexture(SourceTexProperty, scratchCard.ScratchSurface.mainTexture);
            }
            rti = new RenderTargetIdentifier(scratchCard.RenderTexture);
        }

        bool IsInBounds(Rect rect)
        {
            var upperLeft = new Vector2(rect.min.x, rect.max.y);
            var upperRight = rect.max;
            var bottomLeft = rect.min;
            var bottomRight = new Vector2(rect.max.x, rect.min.y);
            return localBounds.Contains(upperLeft) || localBounds.Contains(upperRight) ||
                   localBounds.Contains(bottomLeft) || localBounds.Contains(bottomRight);
        }

        /// <summary>
        /// Draws quad into RenderTexture
        /// </summary>
        public void ScratchHole(Vector2 position, Vector2 scale)
        {
            ModifiedScratchHole(position);
        }

        public void ScratchHole(Vector2 position)
        {

        }

        public void DefaultScratchHole1(Vector2 position)
        {
            Rect positionRect = new Rect(
                (position.x - 0.5f * scratchCard.Eraser.mainTexture.width * (scratchCard.BrushScale.x)) / imageSize.x,
                (position.y - 0.5f * scratchCard.Eraser.mainTexture.height * (scratchCard.BrushScale.y)) / imageSize.y,
                scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x / imageSize.x,
                scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y / imageSize.y);

            if (IsInBounds(positionRect))
            {
                meshHole.vertices = new[]
          {
             new Vector3(positionRect.xMin, positionRect.yMax, 0),
             new Vector3(positionRect.xMax, positionRect.yMax, 0),
             new Vector3(positionRect.xMax, positionRect.yMin, 0),
             new Vector3(positionRect.xMin, positionRect.yMin, 0)
         };
                GL.LoadOrtho();
                commandBuffer.Clear();
                commandBuffer.SetRenderTarget(rti);
                commandBuffer.DrawMesh(meshHole, Matrix4x4.identity, scratchCard.Eraser);
                Graphics.ExecuteCommandBuffer(commandBuffer);
                IsScratched = true;
            }
        }

        public IEnumerator Filling(Vector2 position)
        {
            yield return null;
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.1f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.2f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.3f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.4f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.5f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.6f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.1f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.2f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.3f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.4f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.5f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.6f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.1f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.2f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.3f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.4f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.5f);
            //yield return new WaitForSeconds(0.075f);
            //ScratchHoleAddition(position, 1.6f);
        }

        public IEnumerator FillingLine(Vector2 startPosition, Vector2 endPosition)
        {
            ScratchLineAddition(startPosition, endPosition, 1f);
            yield return new WaitForSeconds(0.05f);
            ScratchLineAddition(startPosition, endPosition, 1.015f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.03f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.045f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.06f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.075f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.09f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.1f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.115f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.13f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.145f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.16f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.175f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.19f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.2f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.215f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.23f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.245f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.26f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.275f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.29f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.3f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.315f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.33f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.345f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.36f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.375f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.39f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.4f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.415f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.43f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.445f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.46f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.475f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.49f);
            yield return new WaitForSeconds(0.02f);
            ScratchLineAddition(startPosition, endPosition, 1.5f);
        }

        public void ScratchHoleAddition(Vector2 position, float multiplier)
        {
            try
            {
                var positionRect = new Rect(
                    (position.x - 0.5f * scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier) / imageSize.x,
                    (position.y - 0.5f * scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier) / imageSize.y,
                    scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier / imageSize.x,
                    scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier / imageSize.y);

                if (IsInBounds(positionRect))
                {
                    meshHole.vertices = new[]
                    {
                    new Vector3(positionRect.xMin, positionRect.yMax, 0),
                    new Vector3(positionRect.xMax, positionRect.yMax, 0),
                    new Vector3(positionRect.xMax, positionRect.yMin, 0),
                    new Vector3(positionRect.xMin, positionRect.yMin, 0)
                };
                    GL.LoadOrtho();
                    commandBuffer.Clear();
                    commandBuffer.SetRenderTarget(rti);
                    commandBuffer.DrawMesh(meshHole, Matrix4x4.identity, scratchCard.Eraser);
                    Graphics.ExecuteCommandBuffer(commandBuffer);
                    IsScratched = true;
                }
            }

            catch
            {
            }
        }

        public void ScratchLineAddition(Vector2 startPosition, Vector2 endPosition, float multiplier)
        {
            try
            {
                var holesCount = (int)Vector2.Distance(startPosition, endPosition) / (int)scratchCard.RenderTextureQuality;
                var positions = new List<Vector3>();
                var colors = new List<Color>();
                var indices = new List<int>();
                var uv = new List<Vector2>();
                var count = 0;
                for (var i = 0; i < holesCount; i++)
                {
                    var holePosition = startPosition + (endPosition - startPosition) / holesCount * i;
                    var positionRect = new Rect(
                        (holePosition.x - 0.5f * scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier) / imageSize.x,
                        (holePosition.y - 0.5f * scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier) / imageSize.y,
                        scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier / imageSize.x,
                        scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier / imageSize.y);

                    if (IsInBounds(positionRect))
                    {
                        positions.Add(new Vector3(positionRect.xMin, positionRect.yMax, 0));
                        positions.Add(new Vector3(positionRect.xMax, positionRect.yMax, 0));
                        positions.Add(new Vector3(positionRect.xMax, positionRect.yMin, 0));
                        positions.Add(new Vector3(positionRect.xMin, positionRect.yMin, 0));

                        colors.Add(Color.white);
                        colors.Add(Color.white);
                        colors.Add(Color.white);
                        colors.Add(Color.white);

                        uv.Add(Vector2.up);
                        uv.Add(Vector2.one);
                        uv.Add(Vector2.right);
                        uv.Add(Vector2.zero);

                        indices.Add(0 + count * 4);
                        indices.Add(1 + count * 4);
                        indices.Add(2 + count * 4);
                        indices.Add(2 + count * 4);
                        indices.Add(3 + count * 4);
                        indices.Add(0 + count * 4);

                        count++;
                    }
                }

                if (positions.Count > 0)
                {
                    if (meshLine != null)
                    {
                        meshLine.Clear(false);
                    }
                    else
                    {
                        meshLine = new Mesh();
                    }
                    meshLine.vertices = positions.ToArray();
                    meshLine.uv = uv.ToArray();
                    meshLine.triangles = indices.ToArray();
                    meshLine.colors = colors.ToArray();
                    GL.LoadOrtho();
                    commandBuffer.Clear();
                    commandBuffer.SetRenderTarget(rti);
                    commandBuffer.DrawMesh(meshLine, Matrix4x4.identity, scratchCard.Eraser);
                    Graphics.ExecuteCommandBuffer(commandBuffer);
                    IsScratched = true;
                }
            }
            catch
            {

            }

        }

        void ModifiedScratchHole(Vector2 position)
        {
            Rect positionRect = new Rect(
                (position.x - 0.5f * scratchCard.Eraser.mainTexture.width * (scratchCard.BrushScale.x)) / imageSize.x,
                (position.y - 0.5f * scratchCard.Eraser.mainTexture.height * (scratchCard.BrushScale.y)) / imageSize.y,
                scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x / imageSize.x,
                scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y / imageSize.y);

            if (IsInBounds(positionRect))
            {
                IsScratched = true;
            }
        }

        List<Rect> Rects = new List<Rect>();

        public void ScratchLine(Vector2 startPosition, Vector2 endPosition, float multiplier)
        {
            int holesCount = (int)Vector2.Distance(startPosition, endPosition) / (int)scratchCard.RenderTextureQuality;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> indices = new List<int>();
            List<Vector2> uv = new List<Vector2>();
            int count = 0;
            for (int i = 0; i < holesCount; i++)
            {
                Vector2 holePosition = startPosition + (endPosition - startPosition) / holesCount * i;
                Rect positionRect = new Rect(
                    (holePosition.x - 0.5f * scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier) / imageSize.x,
                    (holePosition.y - 0.5f * scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier) / imageSize.y,
                    scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x * multiplier / imageSize.x,
                    scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y * multiplier / imageSize.y);
                if (IsInBounds(positionRect))
                {
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMin, 0));
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMin, 0));

                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);

                    uv.Add(Vector2.up);
                    uv.Add(Vector2.one);
                    uv.Add(Vector2.right);
                    uv.Add(Vector2.zero);

                    indices.Add(0 + count * 4);
                    indices.Add(1 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(3 + count * 4);
                    indices.Add(0 + count * 4);
                    count++;

                }

            }


            if (positions.Count > 0)
            {
                if (meshLine != null)
                {
                    meshLine.Clear(false);
                }
                else
                {
                    meshLine = new Mesh();
                }
                meshLine.vertices = positions.ToArray();
                meshLine.uv = uv.ToArray();
                meshLine.triangles = indices.ToArray();
                meshLine.colors = colors.ToArray();

                GL.LoadOrtho();
                try
                {
                    commandBuffer.Clear();
                }
                catch { }
                commandBuffer.SetRenderTarget(rti);
                commandBuffer.DrawMesh(meshLine, Matrix4x4.identity, scratchCard.Eraser);
                Graphics.ExecuteCommandBuffer(commandBuffer);
                IsScratched = true;
            }

        }

        public void ScratchLine1(Vector2 startPosition, Vector2 endPosition)
        {
            int holesCount = (int)Vector2.Distance(startPosition, endPosition) / (int)scratchCard.RenderTextureQuality;
            List<Vector3> positions = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> indices = new List<int>();
            List<Vector2> uv = new List<Vector2>();
            int count = 0;
            for (int i = 0; i < holesCount; i++)
            {
                Vector2 holePosition = startPosition + (endPosition - startPosition) / holesCount * i;
                Rect positionRect = new Rect(
                    (holePosition.x - 0.5f * scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x) / imageSize.x,
                    (holePosition.y - 0.5f * scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y) / imageSize.y,
                    scratchCard.Eraser.mainTexture.width * scratchCard.BrushScale.x / imageSize.x,
                    scratchCard.Eraser.mainTexture.height * scratchCard.BrushScale.y / imageSize.y);
                if (IsInBounds(positionRect))
                {
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMin, 0));
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMin, 0));

                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);

                    uv.Add(Vector2.up);
                    uv.Add(Vector2.one);
                    uv.Add(Vector2.right);
                    uv.Add(Vector2.zero);

                    indices.Add(0 + count * 4);
                    indices.Add(1 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(3 + count * 4);
                    indices.Add(0 + count * 4);

                    count++;

                }

            }


            if (positions.Count > 0)
            {
                if (meshLine != null)
                {
                    meshLine.Clear(false);
                }
                else
                {
                    meshLine = new Mesh();
                }
                meshLine.vertices = positions.ToArray();
                meshLine.uv = uv.ToArray();
                meshLine.triangles = indices.ToArray();
                meshLine.colors = colors.ToArray();

                GL.LoadOrtho();
                commandBuffer.Clear();
                commandBuffer.SetRenderTarget(rti);
                commandBuffer.DrawMesh(meshLine, Matrix4x4.identity, scratchCard.Eraser);
                Graphics.ExecuteCommandBuffer(commandBuffer);
                IsScratched = true;
            }

        }

        public void ModifiedScratchLine(Vector2 startPosition, Vector2 endPosition, Vector2 scale)
        {
            var holesCount = (int)Vector2.Distance(startPosition, endPosition) / (int)scratchCard.RenderTextureQuality;
            var positions = new List<Vector3>();
            var colors = new List<Color>();
            var indices = new List<int>();
            var uv = new List<Vector2>();
            var count = 0;
            for (var i = 0; i < holesCount; i++)
            {
                var holePosition = startPosition + (endPosition - startPosition) / holesCount * i;
                var positionRect = new Rect(
                    (holePosition.x - 0.5f * scratchCard.Eraser.mainTexture.width * scale.x) / imageSize.x,
                    (holePosition.y - 0.5f * scratchCard.Eraser.mainTexture.height * scale.y) / imageSize.y,
                    scratchCard.Eraser.mainTexture.width * scale.x / imageSize.x,
                    scratchCard.Eraser.mainTexture.height * scale.y / imageSize.y);

                if (IsInBounds(positionRect))
                {
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMax, 0));
                    positions.Add(new Vector3(positionRect.xMax, positionRect.yMin, 0));
                    positions.Add(new Vector3(positionRect.xMin, positionRect.yMin, 0));

                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);
                    colors.Add(Color.white);

                    uv.Add(Vector2.up);
                    uv.Add(Vector2.one);
                    uv.Add(Vector2.right);
                    uv.Add(Vector2.zero);

                    indices.Add(0 + count * 4);
                    indices.Add(1 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(2 + count * 4);
                    indices.Add(3 + count * 4);
                    indices.Add(0 + count * 4);

                    count++;
                }
            }

            if (positions.Count > 0)
            {
                if (meshLine != null)
                {
                    meshLine.Clear(false);
                }
                else
                {
                    meshLine = new Mesh();
                }
                meshLine.vertices = positions.ToArray();
                meshLine.uv = uv.ToArray();
                meshLine.triangles = indices.ToArray();
                meshLine.colors = colors.ToArray();
                GL.LoadOrtho();
                commandBuffer.Clear();
                commandBuffer.SetRenderTarget(rti);
                commandBuffer.DrawMesh(meshLine, Matrix4x4.identity, scratchCard.Eraser);
                Graphics.ExecuteCommandBuffer(commandBuffer);
                IsScratched = true;
            }
        }

        public void FillRenderTextureWithColor(Color color)
        {
            commandBuffer.SetRenderTarget(rti);
            commandBuffer.ClearRenderTarget(false, true, color);
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }

        public void SetImageSize(Vector2 size)
        {
            imageSize = size;
        }
    }
}