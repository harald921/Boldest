using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    //struct for object in minimap
	struct ObjectInMap
	{
		public GameObject worldObject;
		public Image icon;
	}
	
	[SerializeField] Camera _miniCamera; // camera that renders the minimap
    [SerializeField] Canvas _canvas; // reference to main canvas for calculating scale

    RectTransform _rectTranform; // transform of minimaps rectangle

	static List<ObjectInMap> _icons = new List<ObjectInMap>(); // list containing all active objects that should appear in map

	private void Start()
	{
        _rectTranform = GetComponent<RectTransform>();
	}

	public static void AddIcon(GameObject obj, Image img)
	{
        // instantiate image prefab and create mapobject 
		Image image = Instantiate(img);
		ObjectInMap mapObj = new ObjectInMap();

        // set members of struct to the instantiated image and the reference to the gameobject in world
		mapObj.worldObject = obj;
		mapObj.icon = image;

		_icons.Add(mapObj);
	}

    public static void RemoveIcon(GameObject obj)
    {
        for(int i =0; i < _icons.Count; i++)
        {
            if(_icons[i].worldObject == obj)
            {
                Destroy(_icons[i].icon.gameObject);
                _icons.Remove(_icons[i]);
                break;
            }
        }
    }
		
	void Update ()
	{
		UpdateIcons();
	}

	void UpdateIcons()
	{
		foreach (ObjectInMap obj in _icons)
		{
            // transform world pos to viewportpoint of the minimap camera and child to canvas
			Vector3 screenPos = _miniCamera.WorldToViewportPoint(obj.worldObject.transform.position);
			obj.icon.transform.SetParent(transform,false);

            //get all corners of the minimap image
			Vector3[] corners = new Vector3[4];
			_rectTranform.GetWorldCorners(corners);

            // multiply the pos from viewport space with the size of the image to get the correct pos in "image space"
            // multiply with canvas scale to get the same result no matter resolution
            // last add on the pixel offset from where the minimap image start (bottom left corner) 
			screenPos.x = screenPos.x * (_rectTranform.rect.width  * _canvas.transform.localScale.x) + corners[0].x;
			screenPos.y = screenPos.y * (_rectTranform.rect.height * _canvas.transform.localScale.y) + corners[0].y;
			screenPos.z = 0;
						
			obj.icon.transform.position = screenPos;

		}
	}

	
}
