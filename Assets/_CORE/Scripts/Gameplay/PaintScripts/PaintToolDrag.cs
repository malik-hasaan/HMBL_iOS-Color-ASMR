using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using FreeDraw;
using DG.Tweening;

public class PaintToolDrag : MonoBehaviour
{
	public Drawable _drawable;
	public bool is_dragable = true;
	public bool is_allowed_to_return = true;
	public bool isRotate;
	public float DragSpeed = 0.1f;
	public Vector3 old_position;
	public Vector3 old_scale;
	public Vector3 screenPoint;
	public Vector3 offset;
	public event Action ActionDownEvent;
	public event Action ActionMoveEvent;
	public event Action ActionUpEvent;

	//public Level_1 level_1;
	public AudioSource ToolPickSound;
	public AudioSource RubSound;
	public bool isDryer;
	public ParticleSystem DryerParticle;
	public GameObject Indicator;

	private void Start()
    {
		_drawable.toolTip = transform.GetChild(0);
    }

    //On action down of the sprite/gameobject .
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
			ToolPickSound.Play();

			RubSound.Play();

			if(isRotate)
            {
				transform.DOLocalRotate(new Vector3(0f, 0f, 25.75f), 0.3f);
			}
			if(isDryer)
            {
				DryerParticle.Play();
            }
		}
	}

	//On action drag of the sprite/gameobject .
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

	//On action up of the sprite/gameobject .
	public virtual void OnMouseUp()
	{
		if (ActionUpEvent != null)
		{
			ActionUpEvent();
		}
		if (is_allowed_to_return)
		{
			GetComponent<BoxCollider2D>().enabled = false;
			gameObject.transform.DOLocalMove(old_position, 0.5f).OnComplete(() => {
				GetComponent<BoxCollider2D>().enabled = true;
			});
			RubSound.Stop();
		}
		if (isRotate)
		{
			transform.DOLocalRotate(Vector3.zero, 0.3f);
		}
		if (isDryer)
		{
			DryerParticle.Stop();
		}
	}

	public void _rem_event_function()
	{
		if (ActionMoveEvent != null)
			ActionMoveEvent();
	}

	public void MoveBackRaser()
    {
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(()=>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.IntroduceFaceCleanerLiquid();
			});
		});
	}

	public void MoveBackFaceCleaner()
	{
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(() =>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.FaceCleanerCompleted();
			});
		});
	}

	public void MoveBackHairDryer()
	{
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(() =>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.HairDryerCompletedFunc();
			});
		});
	}

	public void MoveBackEyebrowTool()
	{
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		Indicator.SetActive(false);
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(() =>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.EyebrowCompletedFunc();
			});
		});
	}

	public void MoveBackEyeShadeTool()
	{
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		Indicator.SetActive(false);
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(() =>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.EyeShadeCompletedFunc();
			});
		});
	}

	public void MoveBackEyeLashesTool()
	{
		RubSound.Stop();
		is_dragable = false;
		is_allowed_to_return = false;
		Indicator.SetActive(false);
		gameObject.transform.DOLocalMove(old_position, 0.75f).OnComplete(() =>
		{
			gameObject.transform.DOLocalMoveX(6f, 0.75f).SetDelay(0.5f).OnComplete(() =>
			{
				//level_1.EyeLasheCompletedFunc();
			});
		});
	}

}

