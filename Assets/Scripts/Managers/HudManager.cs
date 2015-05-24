using UnityEngine;
using System.Collections;

public class HudManager : SingletonApplicationLifetime< HudManager > 
{
	public GameObject tetheredCameraPanel;
	public GameObject shipCameraPanel;

	public void HandleModeChange(AppManager.EMode mode)
	{
		if ( mode == AppManager.EMode.ShipCamera )
		{
			tetheredCameraPanel.SetActive(false);
			shipCameraPanel.SetActive(true);
		}
		else if ( mode == AppManager.EMode.TetheredCamera )
		{
			shipCameraPanel.SetActive(false);
			tetheredCameraPanel.SetActive(true);
		}

	}
}
