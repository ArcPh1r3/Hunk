using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using System;

namespace HunkMod.Modules.Components
{
	public class HunkNotificationQueue : MonoBehaviour
	{
		public const float firstNotificationLengthSeconds = 6f;
		public const float repeatNotificationLengthSeconds = 6f;
		private CharacterMasterNotificationQueue.TimedNotificationInfo notification;

		public event Action<HunkNotificationQueue> onCurrentNotificationChanged;

		public static HunkNotificationQueue GetNotificationQueueForMaster(CharacterMaster master)
		{
			if (master != null)
			{
				HunkNotificationQueue characterMasterNotificationQueue = master.GetComponent<HunkNotificationQueue>();
				if (!characterMasterNotificationQueue)
				{
					characterMasterNotificationQueue = master.gameObject.AddComponent<HunkNotificationQueue>();
				}
				return characterMasterNotificationQueue;
			}
			return null;
		}

		public static void PushNotification(CharacterMaster characterMaster, string token)
		{
			if (!characterMaster.hasAuthority)
			{
				//Debug.LogError("Can't PushItemNotification for " + Util.GetBestMasterName(characterMaster) + " because they aren't local.");
				return;
			}

			HunkNotificationQueue notificationQueueForMaster = HunkNotificationQueue.GetNotificationQueueForMaster(characterMaster);
			if (notificationQueueForMaster)
			{
				notificationQueueForMaster.PushNotification(new CharacterMasterNotificationQueue.NotificationInfo(token, null), 6f);
			}
		}

		private void PushNotification(CharacterMasterNotificationQueue.NotificationInfo info, float duration)
		{
			this.notification = new CharacterMasterNotificationQueue.TimedNotificationInfo
			{
				notification = info,
				startTime = Run.instance.fixedTime,
				duration = duration,
			};

			Action<HunkNotificationQueue> action = this.onCurrentNotificationChanged;
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