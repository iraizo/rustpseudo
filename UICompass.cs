using System.Collections.Generic;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class UICompass : MonoBehaviour
{
	public RawImage compassStrip;

	public CanvasGroup compassGroup;

	public CompassMapMarker CompassMarker;

	public CompassMapMarker TeamLeaderCompassMarker;

	public List<CompassMissionMarker> MissionMarkers;

	public static readonly Phrase IslandInfoPhrase = new Phrase("nexus.compass.island_info", "Continue for {distance} to travel to {zone}");

	public RectTransform IslandInfoContainer;

	public RustText IslandInfoText;

	public float IslandInfoDistanceThreshold = 250f;

	public float IslandLookThreshold = -0.8f;

	public UICompass()
		: this()
	{
	}
}
