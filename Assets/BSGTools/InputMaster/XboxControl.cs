﻿/**
Copyright (c) 2014, Michael Notarnicola
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#if (UNITY_STANDALONE_WIN || UNITY_METRO) && !UNITY_EDITOR_OSX
#define XBOX_ALLOWED
using XInputDotNetPure;
#endif

using System;
using UnityEngine;

namespace BSGTools.IO.Xbox {
	/// <value>
	/// The base class for all Xbox 360 control classes.
	/// </value>
	/// <typeparam name="T">Used for generic self-creation.</typeparam>
	[Serializable]
	public abstract class XboxControl : Control {
		public byte currentController { get; set; }


		public float[] values { get; private set; }
		public float[] realValues { get; private set; }
		public sbyte[] fixedValues { get; private set; }
		public new float value {
			get { return values[currentController]; }
			protected set { values[currentController] = value; }
		}
		public new float realValue {
			get { return realValues[currentController]; }
			protected set { realValues[currentController] = value; }
		}
		public new sbyte fixedValue {
			get { return fixedValues[currentController]; }
			protected set { fixedValues[currentController] = value; }
		}

		public ControlState[] downs { get; private set; }
		public ControlState[] helds { get; private set; }
		public ControlState[] ups { get; private set; }
		public new ControlState down {
			get { return downs[currentController]; }
			protected set { downs[currentController] = value; }
		}
		public new ControlState held {
			get { return helds[currentController]; }
			protected set { helds[currentController] = value; }
		}
		public new ControlState up {
			get { return ups[currentController]; }
			protected set { ups[currentController] = value; }
		}

		public float[] gravities { get; private set; }
		public float[] sensitivities { get; private set; }
		public float[] deads { get; private set; }
		public new float gravity {
			get { return gravities[currentController]; }
			set { gravities[currentController] = value; }
		}
		public new float sensitivity {
			get { return sensitivities[currentController]; }
			set { sensitivities[currentController] = value; }
		}
		public new float dead {
			get { return deads[currentController]; }
			set { deads[currentController] = value; }
		}

		public bool[] snaps { get; private set; }
		public bool[] inverts { get; private set; }
		public bool[] debugOnlys { get; private set; }
		public new bool snap {
			get { return snaps[currentController]; }
			set { snaps[currentController] = value; }
		}
		public new bool invert {
			get { return inverts[currentController]; }
			set { inverts[currentController] = value; }
		}
		public new bool debugOnly {
			get { return debugOnlys[currentController]; }
			set { debugOnlys[currentController] = value; }
		}

		public XboxControl() {
			values = new float[4];
			realValues = new float[4];
			fixedValues = new sbyte[4];

			downs = new ControlState[4];
			ups = new ControlState[4];
			helds = new ControlState[4];

			gravities = new float[4];
			sensitivities = new float[4];
			deads = new float[4];

			snaps = new bool[4];
			inverts = new bool[4];
			debugOnlys = new bool[4];
		}
	}
}