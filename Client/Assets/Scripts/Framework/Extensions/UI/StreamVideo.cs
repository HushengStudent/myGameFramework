/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/26 00:00:14
** desc:  Unity ”∆µ≤•∑≈;
*********************************************************************************/

using MEC;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Framework
{
    public class StreamVideo : MonoBehaviour
    {
        [SerializeField]
        public RawImage RawImage;
        [SerializeField]
        public GameObject PlayButton;
        [SerializeField]
        public VideoClip VideoClip;

        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;

        private bool _isPaused = false;
        private bool _firstRun = true;

        IEnumerator<float> Play()
        {
            PlayButton.SetActive(false);
            _firstRun = false;

            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            _audioSource = gameObject.AddComponent<AudioSource>();

            _videoPlayer.playOnAwake = false;
            _audioSource.playOnAwake = false;
            _audioSource.Pause();

            _videoPlayer.source = VideoSource.VideoClip;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

            _videoPlayer.EnableAudioTrack(0, true);
            _videoPlayer.SetTargetAudioSource(0, _audioSource);

            _videoPlayer.clip = VideoClip;
            _videoPlayer.Prepare();

            while (!_videoPlayer.isPrepared)
            {
                yield return Timing.WaitForOneFrame;
            }
            RawImage.texture = _videoPlayer.texture;

            _videoPlayer.Play();
            _audioSource.Play();

            while (_videoPlayer.isPlaying)
            {
                yield return Timing.WaitForOneFrame;
            }
        }

        public IEnumerator<float> PlayVideo()
        {
            if (!_firstRun && !_isPaused)
            {
                _videoPlayer.Pause();
                _audioSource.Pause();
                _isPaused = true;
                PlayButton.SetActive(true);
            }
            else if (!_firstRun && _isPaused)
            {
                _videoPlayer.Play();
                _audioSource.Play();
                _isPaused = false;
                PlayButton.SetActive(false);
            }
            else
            {
                var itor = Play();
                while (itor.MoveNext())
                {
                    yield return Timing.WaitForOneFrame;
                }
            }
        }
    }
}