﻿// This project is licensed under The MIT License (MIT)
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
using System.Collections;
using RobotArms;

namespace RobotArms.BuiltIn {
	public class DelayedActionProcessor : RobotArmsProcessor<DelayedAction> {

		public override void Process(GameObject entity, DelayedAction delayedAction) {
			if (!delayedAction.Running) {
				return;
			}
			
			delayedAction.Delay -= Time.deltaTime;
			
			if (delayedAction.Delay > 0) {
				if (delayedAction.OnUpdate != null) {
					delayedAction.OnUpdate();
				}
			}
			else {
				if (delayedAction.OnComplete != null) {
					delayedAction.OnComplete();
				}
				RobotArmsUtils.DestroyComponent(delayedAction);
			}
		}
	}
}