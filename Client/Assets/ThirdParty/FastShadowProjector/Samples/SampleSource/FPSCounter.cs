using UnityEngine;
using System.Collections;

namespace FSP_Samples {
	
	public class FPSCounter {
		
		double m_LastTickCount;
		float m_FPS;
		float m_FrameTime;
		float m_DropFrameTime;
		float m_LastDropTime;
		
		const float FPSDropThres = 0.002f;
		const float DropTimeout = 0.5f;
		
		const float AvgFactor = 0.1f;
		
		float m_Dt = 0.0f;
		
		public float timeDelta { 
			get { 
				return m_Dt;
			}
		}

		public FPSCounter() {
			m_FPS = 0.0f;
			m_FrameTime = 0.0f;
			m_DropFrameTime = 0.0f;
			m_LastDropTime = 0.0f;
			
			m_LastTickCount = 0.0f;
		}
		
		public void Update() {
			if (m_LastTickCount < 0.0f || m_Dt > 1.0f) {
				m_LastTickCount = Time.realtimeSinceStartup;
			}
			
			m_Dt = (float)(Time.realtimeSinceStartup - m_LastTickCount);
			
			float curFrameTime = m_FrameTime * (1.0f - AvgFactor) + m_Dt * AvgFactor;
			
			if (Mathf.Abs(m_FrameTime - curFrameTime) > FPSDropThres) {
				if (m_LastDropTime > DropTimeout) {
					m_DropFrameTime = curFrameTime;
					m_LastDropTime = 0.0f;
				} else if ( curFrameTime < m_DropFrameTime) {
					m_DropFrameTime = curFrameTime;
				}
			
			}
			
			m_LastDropTime += Time.deltaTime;
			
			m_FrameTime = curFrameTime;
			
			if (m_FrameTime > 0.01f) {
				m_FPS = 1.0f / m_FrameTime;	
			}
				
			m_LastTickCount = Time.realtimeSinceStartup;
		}
		
		public float GetFPS() { 
			return m_FPS;
		}
		
		public float GetDropFPS() {
			float dropFPS = 0.0f;
			if (m_DropFrameTime > 0.01f) {
			    dropFPS = 1.0f / m_DropFrameTime;
			}
			
			return dropFPS;
		}
		
		public float GetAverageFrameTime() {
			return m_FrameTime;
		}
	}

}

