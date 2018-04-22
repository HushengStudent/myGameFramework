using UnityEngine;
using System.Collections;
using System.Text;

namespace FSP_Samples {

	public class FPSLabel : MonoBehaviour {
		
		FPSCounter m_FPSCounter;
		GUIText m_Label;
		
		public bool m_ShowFPSDrop;
		StringBuilder m_FPSTextBuilder;
		
		char[] m_FPSChars;
		char[] m_FPSDropChars;

		const int FPSTextLen = 10;
		
		const float LabelUpdateFrequency = 0.5f;
		float m_TimeSinceLastLabelUpdate = 0.0f;
		
		void Awake () {
			//this.useGUILayout = false;

			m_FPSCounter = new FPSCounter();
			m_Label = gameObject.GetComponent<GUIText>();
			m_FPSTextBuilder = new StringBuilder(FPSTextLen*3);
			
			m_FPSChars = new char[FPSTextLen];
			m_FPSDropChars = new char[FPSTextLen];
			
			m_TimeSinceLastLabelUpdate = LabelUpdateFrequency;
		}
		
		void Update () {	
			FloatToCharArray(m_FPSChars, m_FPSCounter.GetFPS());

			m_FPSCounter.Update();
			
			m_TimeSinceLastLabelUpdate -= m_FPSCounter.timeDelta;
			if (m_TimeSinceLastLabelUpdate <= 0.0f ) {

				m_FPSTextBuilder.Remove(0, m_FPSTextBuilder.Length);
				m_FPSTextBuilder.Append(m_FPSChars);
				
				if (m_ShowFPSDrop) {
					FloatToCharArray(m_FPSDropChars, m_FPSCounter.GetDropFPS());
					m_FPSTextBuilder.Append(" : ");
					m_FPSTextBuilder.Append(m_FPSDropChars);
				}
			
				m_Label.text = m_FPSTextBuilder.ToString();

				m_TimeSinceLastLabelUpdate = LabelUpdateFrequency;
			}
		}
		
		void FloatToCharArray(char[] charArray, float number) {

			int len = GetDecLen(number);
			int floatPart = 2;
			
			if (len > 1) {
				for (int n = 0; n < len; n++) {
					int div = ((len-1 - n) * 10);
					
					if (div == 0) {
						div = 1;
					}
					
					int digit = ((int)number / div) % 10;
					charArray[n] = (char) (digit + 48);
				}
			} else {
				charArray[0] = (char) ((int)number + 48);
			}
			
			charArray[len] = '.';
			charArray[len+1] = (char)(((int)(number * 10.0f)) % 10 + 48);
			
			for (int n = len + floatPart; n < FPSTextLen; n++) {
				charArray[n] = '\0';
			}
			
		}
		
		int GetDecLen(float number) {
			int len = 0;
			int fpsDec = (int)number;
			
			do {
				fpsDec = fpsDec / 10;
				len++;
			} while (fpsDec != 0);
			
			return len;
		}
		
	}
}
