using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using EFT.UI;
using UnityEngine;
using SPT.Reflection.Patching;
using UnityEngine.Networking;
using HarmonyLib;

namespace Junkbox
{
    public class MusicPatch : ModulePatch
    {
        private static List<AudioClip> _tracks = new List<AudioClip>();

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GUISounds), nameof(GUISounds.method_1));
        }

        [PatchPostfix]
        private static void PatchPostfix(ref AudioClip[] ___audioClip_0)
        {
            int n = _tracks.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                (_tracks[k], _tracks[n]) = (_tracks[n], _tracks[k]);
            }

            if (_tracks.Count == 1)
            {
                _tracks.Add(_tracks[0]);
            }
            ___audioClip_0 = _tracks.ToArray();
        }

        public MusicPatch()
        {
            var pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesInDir = Directory.GetFiles($"{pluginDir}/menu_music");

            foreach (var musicFile in filesInDir)
            {
                var uri = "file:///" + musicFile.Replace("\\", "/");

                if (musicFile.Contains(".mp3"))
                    LoadAudio(uri, AudioType.MPEG);
                else if (musicFile.Contains(".ogg"))
                    LoadAudio(uri, AudioType.OGGVORBIS);
                else if (musicFile.Contains(".wav"))
                    LoadAudio(uri, AudioType.WAV);
                else
                    LoadAudio(uri, AudioType.UNKNOWN);
            }
        }

        private async void LoadAudio(string url, AudioType audioType)
        {
            using (var web = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                var operation = web.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                var track = DownloadHandlerAudioClip.GetContent(web);
                track.name = Path.GetFileNameWithoutExtension(url);
                _tracks.Add(track);
            }
        }
    }
}
