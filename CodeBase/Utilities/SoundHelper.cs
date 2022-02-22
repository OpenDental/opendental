using System;
using System.IO;
using System.Media;

namespace CodeBase {
	public class SoundHelper {

		/// <summary>If running in ODCloud, converts the given byte array to base64 and sends the data asynchronously to the browser to be played.
		/// Otherwise, plays the sound asynchronously using SoundPlayer.Play().</summary>
		public static void PlaySound(byte[] rawData) {
			if(ODBuild.IsWeb()) {
				string base64=Convert.ToBase64String(rawData);
				ODException.SwallowAnyException(() => ODCloudClient.SendDataToBrowser(base64,(int)ODCloudClient.BrowserAction.PlaySound));
				return;
			}
			using MemoryStream stream=new MemoryStream(rawData);
			using SoundPlayer simpleSound = new SoundPlayer(stream);
			simpleSound.Play();
		}

		/// <summary>If running in ODCloud, converts the given byte array to base64 and sends the data synchronously to the browser to be played.
		/// Otherwise, plays the sound synchronously using SoundPlayer.PlaySync().</summary>
		public static void PlaySoundSync(byte[] rawData) {
			if(ODBuild.IsWeb()) {
				string base64=Convert.ToBase64String(rawData);
				ODException.SwallowAnyException(() => ODCloudClient.SendToBrowserSynchronously(base64,ODCloudClient.BrowserAction.PlaySound,5,false));
				return;
			}
			using MemoryStream stream=new MemoryStream(rawData);
			using SoundPlayer simpleSound = new SoundPlayer(stream);
			simpleSound.PlaySync();
		}

	}
}
