﻿/**
Copyright (c) 2014, Michael Notarnicola
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

#if (UNITY_STANDALONE_WIN || UNITY_METRO) && !UNITY_EDITOR_OSX
using BSGTools.IO.Xbox;
#endif

namespace BSGTools.IO {
	/// <summary>
	/// Simple extensions class for commonly used enum functionality.
	/// </summary>
	public static class EnumExt {
		/// <summary>
		/// Similar to .NET 4.0+'s method to check if a flag is set on an enum.
		/// </summary>
		/// <param name="value">The current value.</param>
		/// <param name="flag">The flag to check.</param>
		/// <returns></returns>
		public static bool HasFlag(this Enum value, Enum flag) {
			return (Convert.ToInt64(value) & Convert.ToInt64(flag)) != 0;
		}
	}

	/// <summary>
	/// A single instance of this exists in the application.
	/// Updates and maintains all Control states.
	/// </summary>
	[DisallowMultipleComponent]
	public class InputMaster : MonoBehaviour {
		private List<Control> controls = new List<Control>();

		/// <summary>
		/// Not yet implemented.
		/// </summary>
		public bool AnyKeyDown { get; private set; }
		/// <summary>
		/// Not yet implemented.
		/// </summary>
		public bool AnyKeyHeld { get; private set; }
		/// <summary>
		/// Not yet implemented.
		/// </summary>
		public bool AnyKeyUp { get; private set; }

		void Update() {
#if (UNITY_STANDALONE_WIN || UNITY_METRO) && !UNITY_EDITOR_OSX
			XboxUtils.UpdateStates();
#endif

			foreach(var c in controls) {
				if((c.IsDebugControl && Debug.isDebugBuild) || c.IsDebugControl == false)
					c.Update();
			}

			AnyKeyDown = controls.OfType<Control>().Any(c => c.Down.HasFlag(ControlState.Either));
			AnyKeyHeld = controls.OfType<Control>().Any(c => c.Held.HasFlag(ControlState.Either));
			AnyKeyUp = controls.OfType<Control>().Any(c => c.Up.HasFlag(ControlState.Either));
		}

		/// <summary>
		/// Resets all states/values on all controls.
		/// </summary>
		/// <seealso cref="ResetAll(bool)"/>
		public void ResetAll() {
			foreach(var c in controls)
				c.Reset();
		}

		/// <summary>
		/// Blocks or unblocks all controls.
		/// This has the side effect of resetting all control states.
		/// </summary>
		/// <param name="blocked">To block/unblock.</param>
		/// <seealso cref="Control.IsBlocked"/>
		public void SetBlockAll(bool blocked) {
			foreach(var c in controls)
				c.IsBlocked = blocked;
		}

		/// <summary>
		/// Internally used to get all <see cref="Control"/> variables using reflection from a class instance.
		/// Not the most performance efficient method of supplying InputMaster with controls.
		/// </summary>
		/// <param name="controlClass">The instance of a class to get the controls from.</param>
		/// <returns>An array of all Control objects.</returns>
		private static Control[] GetAllControls(object controlClass) {
			var type = controlClass.GetType();

			var bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

			var properties = type.GetProperties(bf).Where(p => p.GetValue(controlClass, null) != null).Select(p => p.GetValue(controlClass, null));
			var fields = type.GetFields(bf).Where(f => f.GetValue(controlClass) != null).Select(f => f.GetValue(controlClass));

			var p_bare = properties.OfType<Control>();
			var p_enum = properties.OfType<IEnumerable<Control>>();
			var f_bare = fields.OfType<Control>();
			var f_enum = fields.OfType<IEnumerable<Control>>();

			var final = p_bare.Concat(f_bare);


			foreach(var p_array in p_enum)
				final = final.Concat(p_array);
			foreach(var f_array in f_enum)
				final = final.Concat(f_array);

			return final.ToArray();
		}

		/// <summary>
		/// Uses reflection to get all controls in a class.
		/// Depending on the control count from your controlClass, this could have a noticable performance spike unless used during loading.
		/// </summary>
		/// <param name="controlClass">The instance of a class to get the controls from.</param>
		/// <returns>The new InputMaster instance.</returns>
		public static InputMaster CreateMaster(object controlClass) {
			return CreateMaster(GetAllControls(controlClass));
		}

		/// <summary>
		/// Creates a new, empty, hidden GameObject, adds a new instance of InputMaster to it,
		/// and adds the provided controls to the master's control list.
		/// </summary>
		/// <param name="controls">A full listing of all of the games controls with the default bindings.</param>
		/// <returns>The new InputMaster instance.</returns>
		public static InputMaster CreateMaster(params Control[] controls) {
			var parent = new GameObject("_InputMaster");
			DontDestroyOnLoad(parent);
			var master = parent.AddComponent<InputMaster>();
			master.controls.AddRange(controls);
			return master;
		}

		/// <summary>
		/// Destroys the InputMaster object.
		/// </summary>
		public void DestroyMaster() {
			Destroy(gameObject);
		}

		/// <summary>
		/// Searches the control list for the provided Control objects
		/// and replaces them with said provided object
		/// </summary>
		/// <param name="controls">The Control objects to search and replace</param>
		public void UpdateControls(params Control[] controls) {
			foreach(var c in controls) {
				int index = this.controls.IndexOf(c);
				if(index == -1)
					throw new System.ArgumentException(string.Format("Could not find Control {0} in master control list. Aborting!", c.ToString()));
				this.controls[index] = c;
			}
		}

		#region XboxUtils
#if (UNITY_STANDALONE_WIN || UNITY_METRO) && !UNITY_EDITOR_OSX
		void OnApplicationPause(bool paused) {
			if(XboxUtils.StopVibrateOnAppPause && paused == true)
				XboxUtils.SetVibrationAll(0f);
		}

		void OnApplicationQuit() {
			XboxUtils.SetVibrationAll(0f);
		}

		void OnApplicationFocus(bool focused) {
			if(XboxUtils.StopVibrateOnAppFocusLost && focused == false)
				XboxUtils.SetVibrationAll(0f);
		}
#endif
		#endregion
	}
}