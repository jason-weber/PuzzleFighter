using UnityEngine;
using System;
using System.Collections;

public delegate void SelectEventHandler(Block sender);

public class Block : MonoBehaviour {
	public enum BlockTypes {
		Air,
		Damage,
		Earth,
		Fire,
		Special,
		Water
	}
	public static int size = 100;
	public bool selected = false;
	public event SelectEventHandler selectEvent;

	public int x;
	public int y;

	public BlockTypes blockType;

	void OnMouseDown() {
		if(selectEvent != null) {
			selectEvent(this);
		}

	}
}
