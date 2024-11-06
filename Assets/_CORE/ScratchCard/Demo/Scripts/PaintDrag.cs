using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using ScratchCardAsset;

public class PaintDrag : MonoBehaviour
{
	public bool is_dragable = true;
	public bool is_allowed_to_return = true;
	public float DragSpeed = 0.1f;
	public Vector3 old_position;
	public Vector3 old_scale;
	public Vector3 screenPoint;
	public Vector3 offset;
	public event Action ActionDownEvent;
	public event Action ActionMoveEvent;
	public event Action ActionUpEvent;
	public ScratchCard _scratchCard;
	private Vector2 firstScale, finalScale;


    private void Start()
    {
		if (_scratchCard == null)
			firstScale = _scratchCard.BrushScale;
    }

    //On action down of the nibSprite/gameobject .
    public virtual void OnMouseDown()
	{
		if (is_dragable)
		{
			old_position = gameObject.transform.localPosition;
			old_scale = gameObject.transform.localScale;

			offset = gameObject.transform.position -
			Camera.main.ScreenToWorldPoint(
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

			if (ActionDownEvent != null)
			{
				ActionDownEvent();
			}

			if(_scratchCard != null)
			   _scratchCard.InputEnabled = true;


		}
	}

	//On action drag of the nibSprite/gameobject .
	public virtual void OnMouseDrag()
	{
		if (is_dragable)
		{
			Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
			transform.position = Vector3.Lerp(transform.position, curPosition, DragSpeed);

			if (ActionMoveEvent != null)
				ActionMoveEvent();
		}

	}

	//On action up of the nibSprite/gameobject .
	public virtual void OnMouseUp()
	{
		if (ActionUpEvent != null)
		{
			ActionUpEvent();
		}
		if (is_allowed_to_return)
		{
			gameObject.transform.localPosition = old_position;
		}

		if (_scratchCard != null)
			_scratchCard.InputEnabled = false;
	}

	public void _rem_event_function()
	{
		if (ActionMoveEvent != null)
			ActionMoveEvent();
	}



	// Custom Methods
	private void ChangeScale()
	{

	}
}

