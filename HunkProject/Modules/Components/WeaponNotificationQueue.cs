using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using System;

namespace HunkMod.Modules.Components
{
	public class WeaponNotificationQueue : MonoBehaviour
	{
		public const float firstNotificationLengthSeconds = 6f;
		public const float repeatNotificationLengthSeconds = 6f;
		private CharacterMaster master;
		private CharacterMasterNotificationQueue.TimedNotificationInfo notification;

		public event Action<WeaponNotificationQueue> onCurrentNotificationChanged;

		public static WeaponNotificationQueue GetNotificationQueueForMaster(CharacterMaster master)
		{
			if (master != null)
			{
				WeaponNotificationQueue characterMasterNotificationQueue = master.GetComponent<WeaponNotificationQueue>();
				if (!characterMasterNotificationQueue)
				{
					characterMasterNotificationQueue = master.gameObject.AddComponent<WeaponNotificationQueue>();
					characterMasterNotificationQueue.master = master;
				}
				return characterMasterNotificationQueue;
			}
			return null;
		}

		public static void PushWeaponNotification(CharacterMaster characterMaster, int weaponIndex)
		{
			if (!characterMaster.hasAuthority)
			{
				//Debug.LogError("Can't PushItemNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");
				return;
			}

			WeaponNotificationQueue notificationQueueForMaster = WeaponNotificationQueue.GetNotificationQueueForMaster(characterMaster);
			if (notificationQueueForMaster)
			{
				HunkWeaponDef weaponDef = HunkWeaponCatalog.GetWeaponFromIndex(weaponIndex);

				if (weaponDef)
				{
					notificationQueueForMaster.PushNotification(new CharacterMasterNotificationQueue.NotificationInfo(weaponDef, null), 3.5f);
				}
			}
		}

		private void PushNotification(CharacterMasterNotificationQueue.NotificationInfo info, float duration)
		{
			this.notification = new CharacterMasterNotificationQueue.TimedNotificationInfo
			{
				notification = info,
				startTime = Run.instance.fixedTime,
				duration = duration
			};

			Action<WeaponNotificationQueue> action = this.onCurrentNotificationChanged;
			if (action == null) return;
			action(this);
		}

		public float GetCurrentNotificationT()
		{
			return (Run.instance.fixedTime - this.notification.startTime) / this.notification.duration;
		}

		public CharacterMasterNotificationQueue.NotificationInfo GetCurrentNotification()
		{
			return this.notification?.notification;
		}
	}
}