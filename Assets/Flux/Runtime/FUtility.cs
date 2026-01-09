using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Flux
{
	/**
	 * @brief Runtime Utility class for Flux
	 */
	public class FUtility {

		public const int FLUX_VERSION = 220;

		public static bool IsAnimationEditable( AnimationClip clip )
		{
			return clip == null || ( ((clip.hideFlags & HideFlags.NotEditable) == 0) && !clip.isLooping );
		}

		public static void ResizeAnimationCurve( AnimationCurve curve, float newLength )
		{
			float frameRate = 60;

			float oldLength = curve.length == 0 ? 0 : curve.keys[curve.length-1].time;
//			float newLength = newLength;
			
			if( oldLength == 0 )
			{
				// handle no curve
				curve.AddKey(0, 1);
				curve.AddKey(newLength, 1);
				return;
			}
			
			float ratio = newLength / oldLength;
			float inverseRatio = 1f / ratio;
			
			int start = 0;
			int limit = curve.length;
			int increment = 1;
			
			if( ratio > 1 )
			{
				start = limit - 1;
				limit = -1;
				increment = -1;
			}
			
			for( int i = start; i != limit; i += increment )
            {
                Keyframe oldKeyframe = curve.keys[i];
				Keyframe newKeyframe = new Keyframe( Mathf.RoundToInt(oldKeyframe.time * ratio * frameRate)/frameRate, oldKeyframe.value, oldKeyframe.inTangent*inverseRatio, oldKeyframe.outTangent*inverseRatio, oldKeyframe.inWeight, oldKeyframe.outWeight);

                AnimationUtility.TangentMode leftTangentMode = AnimationUtility.GetKeyLeftTangentMode(curve, i);
                AnimationUtility.TangentMode rightTangentMode = AnimationUtility.GetKeyRightTangentMode(curve, i);
                curve.MoveKey(i, newKeyframe );

                AnimationUtility.SetKeyLeftTangentMode(curve, i, leftTangentMode );
                AnimationUtility.SetKeyRightTangentMode(curve, i, rightTangentMode);
            }
		}
	}
}
