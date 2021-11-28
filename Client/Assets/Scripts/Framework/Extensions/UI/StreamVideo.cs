/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/26 00:00:14
** desc:  Unity ”∆µ≤•∑≈;
*********************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Framework
{
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioListener))]
    public class StreamVideo : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;

        private bool _paused = false;
        private bool _running = false;

        private IEnumerator Run(string url)
        {
            _running = true;

            var rawImage = gameObject.GetComponent<RawImage>();
            _audioSource = gameObject.GetComponent<AudioSource>();
            _videoPlayer = gameObject.GetComponent<VideoPlayer>();

            _videoPlayer.playOnAwake = false;
            _audioSource.playOnAwake = false;
            _audioSource.Pause();

            _videoPlayer.source = VideoSource.VideoClip;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

            _videoPlayer.EnableAudioTrack(0, true);
            _videoPlayer.SetTargetAudioSource(0, _audioSource);

            _videoPlayer.url = url;
            _videoPlayer.Prepare();

            while (!_videoPlayer.isPrepared)
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
            rawImage.texture = _videoPlayer.texture;

            _videoPlayer.Play();
            _audioSource.Play();

            while (_videoPlayer.isPlaying)
            {
                yield return CoroutineMgr.WaitForEndOfFrame;
            }
        }

        public IEnumerator Play(string url)
        {
            if (_running && !_paused)
            {
                _videoPlayer.Pause();
                _audioSource.Pause();
                _paused = true;
            }
            else if (_running && _paused)
            {
                _videoPlayer.Play();
                _audioSource.Play();
                _paused = false;
            }
            else
            {
                var itor = Play(url);
                while (itor.MoveNext())
                {
                    yield return CoroutineMgr.WaitForEndOfFrame;
                }
            }
        }
    }
}