using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightmapPrefab : MonoBehaviour 
{

	Renderer[] targetRenderers;
	public LightmapBase[] targetLightmaps;

	public int lightmapID;

	void Start () 
	{
		UpdateLightmap();
	}

	void Update()
	{
		if(!Application.isPlaying)
			UpdateLightmap();
	}

	void UpdateLightmap()
	{

		targetLightmaps = GameObject.FindObjectsOfType<LightmapBase>();

		foreach(LightmapBase l in targetLightmaps)
		{
			if(l.lightmapId == lightmapID)
			{
				Renderer[] targetRenderers = l.GetComponentsInChildren<Renderer>();
				Renderer[] currentRenderers = GetComponentsInChildren<Renderer>();

				for(int a = 0;a<targetRenderers.Length;a++)
				{
					currentRenderers[a].lightmapIndex = targetRenderers[a].lightmapIndex;
					currentRenderers[a].lightmapScaleOffset = targetRenderers[a].lightmapScaleOffset;
				}
			}

		}
	}

}