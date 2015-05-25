using UnityEngine;
using System.Collections;

public class HudManager : SingletonApplicationLifetime< HudManager > 
{
	public GameObject tetheredCameraPanel;
	public GameObject shipCameraPanel;
	public GameObject internalCameraPanel;

	public void HandleModeChange(AppManager.EMode mode)
	{
		if ( mode == AppManager.EMode.ShipCamera )
		{
			tetheredCameraPanel.SetActive(false);
			shipCameraPanel.SetActive(true);
			internalCameraPanel.SetActive(false);
		}
		else if ( mode == AppManager.EMode.TetheredCamera )
		{
			shipCameraPanel.SetActive(false);
			tetheredCameraPanel.SetActive(true);
			internalCameraPanel.SetActive(false);
		}
		else if ( mode == AppManager.EMode.InternalCamera )
		{
			shipCameraPanel.SetActive(false);
			tetheredCameraPanel.SetActive(false);
			internalCameraPanel.SetActive(true);
		}

	}
}
