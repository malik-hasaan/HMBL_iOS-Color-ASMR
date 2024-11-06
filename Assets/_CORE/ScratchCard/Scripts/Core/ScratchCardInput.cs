using UnityEngine;

namespace ScratchCardAsset.Core
{
	/// <summary>
	/// Process Input for ScratchCard
	/// </summary>
	public class ScratchCardInput
	{
		#region Events

		public event ScratchHandler OnScratch;
		public event ScratchStartHandler OnScratchStart;
		public event ScratchLineHandler OnScratchLine;
		public event ScratchHoleHandler OnScratchHole;
		public delegate Vector2 ScratchHandler(Vector2 position);
		public delegate void ScratchStartHandler();
		public delegate void ScratchLineHandler(Vector2 start, Vector2 end);
		public delegate void ScratchHoleHandler(Vector2 position);
		
		#endregion


		private ScratchCard scratchCard;
		private Vector2 eraseStartPositions;
		private Vector2 eraseEndPositions;
		private Vector2 erasePosition;
		private bool isStartPosition;

		private const int MaxTouchCount = 10;

		public ScratchCardInput(ScratchCard card)
		{
			scratchCard = card;
			isStartPosition = true;
		}

		public void Update()
		{
			if (!scratchCard.InputEnabled)
                return;
			

			if (Input.GetMouseButtonDown(0))
			{
				scratchCard.IsScratching = false;
				isStartPosition = true;
				//CardRendererHelper.instance.meshCreator.StartDrawing(scratchCard.GetCardrenderer());
			}
			if (Input.GetMouseButton(0))
			{
				TryScratch(Input.mousePosition);
			}
			if (Input.GetMouseButtonUp(0))
			{
				scratchCard.IsScratching = false;
				//CardRendererHelper.instance.meshCreator.EndDrawing();
			}

		}

		private void TryScratch(Vector2 position)
		{
			try
			{
				if (OnScratch != null)
				{
					erasePosition = OnScratch(position);
				}

				if (isStartPosition)
				{
					eraseStartPositions = erasePosition;
					eraseEndPositions = eraseStartPositions;
					isStartPosition = false;
				}
				else
				{
					eraseStartPositions = eraseEndPositions;
					eraseEndPositions = erasePosition;
				}

				if (!scratchCard.IsScratching)
				{
					eraseEndPositions = eraseStartPositions;
					scratchCard.IsScratching = true;
				}
			}
			catch
			{

			}
		}
		
		public void Scratch()
		{
			if (OnScratchStart != null)
			{
				OnScratchStart();
			}

			if (eraseStartPositions == eraseEndPositions)
			{
				if (OnScratchHole != null)
				{
					OnScratchHole(erasePosition);
				}
			}
			else
			{
				if (OnScratchLine != null)
				{
					/*if(scratchCard.useChangingScale)
					{
						OnScratchHole(erasePosition);
					}
					else
					{*/
						OnScratchLine(eraseStartPositions, eraseEndPositions);
					//}
                }
			}
		}
        public void ScratchAtPoint(Vector2 pos)
        {
            if (OnScratch != null)
                OnScratchHole(OnScratch(pos));
        }
    }
}