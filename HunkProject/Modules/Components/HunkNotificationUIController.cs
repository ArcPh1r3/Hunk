using System;
using UnityEngine;
using RoR2;
using RoR2.UI;

namespace HunkMod.Modules.Components
{
	public class HunkNotificationUIController : MonoBehaviour
	{
		public HUD hud;
		public GameObject genericNotificationPrefab;
		public HunkNotification currentNotification;
		public CharacterMaster targetMaster;
		public HunkNotificationQueue notificationQueue;

		public void OnEnable()
		{
			CharacterMaster.onCharacterMasterLost += this.OnCharacterMasterLost;
		}

		public void OnDisable()
		{
			CharacterMaster.onCharacterMasterLost -= this.OnCharacterMasterLost;
			this.CleanUpCurrentMaster();
		}

		public void Update()
		{
			if (this.hud.targetMaster != this.targetMaster)
			{
				this.SetTargetMaster(this.hud.targetMaster);
			}

			if (this.currentNotification && this.notificationQueue)
			{
				this.currentNotification.SetNotificationT(this.notificationQueue.GetCurrentNotificationT());
			}
		}

		private void SetUpNotification(CharacterMasterNotificationQueue.NotificationInfo notificationInfo)
		{
			this.currentNotification = UnityEngine.Object.Instantiate<GameObject>(this.genericNotificationPrefab).GetComponent<HunkNotification>();

			var obj = notificationInfo.data;
			if (obj != null)
			{
				this.currentNotification.SetText((string)obj);
			}

			this.currentNotification.GetComponent<RectTransform>().SetParent(base.GetComponent<RectTransform>(), false);
		}

		private void OnCurrentNotificationChanged(HunkNotificationQueue notificationQueue)
		{
			this.ShowCurrentNotification(notificationQueue);
		}

		private void ShowCurrentNotification(HunkNotificationQueue notificationQueue)
		{
			this.DestroyCurrentNotification();
			CharacterMasterNotificationQueue.NotificationInfo notificationInfo = notificationQueue.GetCurrentNotification();
			if (notificationInfo != null)
			{
				this.SetUpNotification(notificationInfo);
			}
		}

		private void DestroyCurrentNotification()
		{
			if (this.currentNotification)
			{
				UnityEngine.Object.Destroy(this.currentNotification.gameObject);
				this.currentNotification = null;
			}
		}

		private void SetTargetMaster(CharacterMaster newMaster)
		{
			this.DestroyCurrentNotification();
			this.CleanUpCurrentMaster();
			this.targetMaster = newMaster;
			if (newMaster)
			{
				this.notificationQueue = HunkNotificationQueue.GetNotificationQueueForMaster(newMaster);
				this.notificationQueue.onCurrentNotificationChanged += this.OnCurrentNotificationChanged;
				this.ShowCurrentNotification(this.notificationQueue);
			}
		}

		private void OnCharacterMasterLost(CharacterMaster master)
		{
			if (master == this.targetMaster)
			{
				this.CleanUpCurrentMaster();
			}
		}

		private void CleanUpCurrentMaster()
		{
			if (this.notificationQueue)
			{
				this.notificationQueue.onCurrentNotificationChanged -= this.OnCurrentNotificationChanged;
			}
			this.notificationQueue = null;
			this.targetMaster = null;
		}
	}
}