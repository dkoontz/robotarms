// This project is licensed under The MIT License (MIT)
// 
// Copyright 2014 David Koontz, Trenton Kennedy
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// 	 The above copyright notice and this permission notice shall be included in
// 	 all copies or substantial portions of the Software.
//   
// 	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// 	 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// 	 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// 	 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// 	 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// 	 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// 	 THE SOFTWARE.
//   
// 	 Please direct questions, patches, and suggestions to the project page at
// 	 https://bitbucket.org/dkoontz/robotarms

using UnityEngine;

namespace RobotArms {
	public class RobotArmsComponent : MonoBehaviour {
		public static IRobotArmsCoordinator RobotArmsCoordinator;

		// Set this to false during Initialize or Awake to prevent the component
		// from being registered with RobotArms. This is useful for situations
		// where the object is created but not yet active such as with an object pool
		protected bool autoRegister = true;
		protected bool reRegisterOnSceneLoad;

		public void Awake() {
			Initialize();
		}

		public void OnEnable() {
			RegisterComponent();
		}

		// This is needed for the very first update since Unity will call
		// Awake and OnEnable of a GameObject during the Instantiate call
		// before other objects can run their Awake, therefore the RobotArmsCoordinator
		// will not be set yet and we need to run the registration later
		// when we can guarantee that reference is set. On all subsequent
		// enable/disables OnEnable will function correctly.
		public void Start() {
			RegisterComponent();
		}

		public void OnDisable() {
			RobotArmsCoordinator.UnregisterComponent(this);
		}

		public void OnLevelWasLoaded(int levelId) {
			if (reRegisterOnSceneLoad) {
				RegisterComponent();
			}
		}

		/// <summary>
		/// Called before component registers itself with RobotArms.
		/// </summary>
		protected virtual void Initialize() { }

		void RegisterComponent() {
			if (autoRegister && RobotArmsCoordinator != null) {
				RobotArmsCoordinator.RegisterComponent(this);
			}
		}
	}
}