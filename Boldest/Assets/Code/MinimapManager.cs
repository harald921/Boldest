using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
	struct ObjectInMap
	{
		public GameObject worldObject;
		public Image icon;

	}

	

	[SerializeField]
	Camera _miniCamera;

	public Canvas _canvas;
	Image _mapImage;

	static List<ObjectInMap> _icons = new List<ObjectInMap>();
	private void Start()
	{
		_mapImage = GetComponent<Image>();
	}

	public static void AddIcon(GameObject obj, Image img)
	{
		Image image = Instantiate(img);
		ObjectInMap mapObj = new ObjectInMap();
		mapObj.worldObject = obj;
		mapObj.icon = image;
		_icons.Add(mapObj);
	}
	
	
	void Update ()
	{
		UpdateIcons();
	}

	void UpdateIcons()
	{
		foreach (ObjectInMap obj in _icons)
		{
			Vector3 screenPos = _miniCamera.WorldToViewportPoint(obj.worldObject.transform.position);
			obj.icon.transform.SetParent(transform,false);

			Vector3[] corners = new Vector3[4];
			GetComponent<RectTransform>().GetWorldCorners(corners);

			screenPos.x = screenPos.x * (GetComponent<RectTransform>().rect.width  * _canvas.transform.localScale.x) + corners[0].x;
			screenPos.y = screenPos.y * (GetComponent<RectTransform>().rect.height * _canvas.transform.localScale.y) + corners[0].y;
			screenPos.z = 0;
						
			obj.icon.transform.position = screenPos;

		}
	}

	
}
