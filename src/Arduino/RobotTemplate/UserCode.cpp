/*
 * UserCode.cpp
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#include "UserCode.h"
#include <WProgram.h>
#include <FrcComms\FRCCommunication.h>
#include <Servo.h>
#include "Utils.h"

/*
 * Guaranteed to be called after the following have been initialized:
 * Ethernet, configuration, serial, and FRCComms.
 *
 * Place any one time robot specific initialization code here.
 */
void UserRobot::userInit(void) {
	//Initialize user code, robot sensors, etc. here.

	start = 0;
	position = 0;
	direction = 1;
	range = 40;

	autoInitComplete = false;
	teleInitComplete = false;
	disabledInitComplete = false;

	comm = FRCCommunication::getInstance();
	Serial.println("User init complete.");
}

/*
 * Disabled and enables all outputs. Must be implemented or a disabled robot may behave irratically.
 */
void UserRobot::setOutputsEnabled(bool enabled) {
	if (enabled && !attached) {
		left.attach(5);
		right.attach(6);
		rear.attach(7);
		lift.attach(12);
		shooter.attach(8);
		gate.attach(11);
		attached = true;
	} else if (!enabled && attached) {
		left.detach();
		right.detach();
		rear.detach();
		lift.detach();
		shooter.detach();
		gate.detach();
		attached = false;
	}
}

/*
 * Perform any initialization needed before teleop mode.
 *
 * Guaranteed to be called prior to teleopInit's first call after first boot or autoInit or disabledInit have been called.
 */
void UserRobot::teleopInit() {
	Serial.println("Entering tele-op");
}

//Control scale factor. Must be fraction.

/*
 * Process teleop control data here.
 * This code is called every time new control data is received.
 * In theory this will be called at 50hz, but due to network losses, etc. the
 * actual rate will be less than 50hz, and not guaranteed.
 */
void UserRobot::teleopLoop() {
	Joystick stick = comm->controlData->joysticks[0];

	int y = stick.axis[1];
	int x = stick.axis[2];
	int z = stick.axis[0];

	/*y = y * 1 / 4;
	 x = x * 1 / 4;
	 z = z * 15 / 100;*/
	z = z / 2;
	int shooterValue = stick.axis[3];

	bool liftInput = stick.buttons.getBit(0);

	if (liftInput == true && liftInput != lastLiftInput)
		liftState = !liftState;
	lastLiftInput = liftInput;
	int liftValue = liftState ? 128 : 0;

	bool gateInput = stick.buttons.getBit(1);
	if (gateInput == true && gateInput != lastGateInput)
		gateState = !gateState;
	lastGateInput = gateInput;
	int gateValue = gateState ? 0 : 128;

	liftValue = stick.axis[5];
	if (teleopCounter % 25 == 0) {
		Serial.println(z);
		Serial.println(y);
		Serial.println(x);
	}
	int panVal = position; //= stick.axis[3];
	position += 1;
	if (position > 127)
		position = -128;
	int tiltVal = stick.axis[4];

	int rearValue = x + z;
	int rightValue = (-x / 2) + ((-y * 216) / 250) + z;
	int leftValue = (-x / 2) + ((y * 216) / 250) + z;

	rightValue = limit(rightValue, -128, 127);
	leftValue = limit(leftValue, -128, 127);
	rearValue = limit(rearValue, -128, 127);

	right.writeMicroseconds(map(rightValue, -128, 127, 1000, 2000));
	left.writeMicroseconds(map(leftValue, -128, 127, 1000, 2000));
	rear.writeMicroseconds(map(rearValue, -128, 127, 1000, 2000));
	shooter.writeMicroseconds(map(shooterValue, -128, 127, 1000, 2000));
	lift.writeMicroseconds(map(liftValue, -128, 127, 1000, 2000));
	gate.writeMicroseconds(map(gateValue, -128, 127, 1000, 2000));
	teleopCounter++;
}

/*
 * Perform any initialization needed before disabled mode.
 *
 * Guaranteed to be called prior to teleopInit's first call after first boot or autoInit or disabledInit have been called.
 */
void UserRobot::disabledInit() {
	Serial.println("Entering disabled");
}
/*
 * Process disabled control data here.
 * This code is called every time new control data is received.
 * In theory this will be called at 50hz, but due to network losses, etc. the
 * actual rate will be less than 50hz, and not guaranteed.
 */
void UserRobot::disabledLoop() {

}

/*
 * Perform any initialization needed before autonomous mode.
 *
 * Guaranteed to be called prior to teleopInit's first call after first boot or autoInit or disabledInit have been called.
 */
void UserRobot::autonomousInit() {
	Serial.println("Entering autonomous");
}

/*
 * Process autonomous control data here.
 * This code is called every time new control data is received.
 * In theory this will be called at 50hz, but due to network losses, etc. the
 * actual rate will be less than 50hz, and not guaranteed.
 */
void UserRobot::autonomousLoop() {
}

/*
 * Will be called at the frequency specified in UserConstants.h.
 * Guaranteed to be accurate over time, but inaccurate in the short term.
 *
 * The delayed argument is the number of milliseconds past the scheduled execution
 * time the loop is being called.
 *
 * The elapsed argument is the number of milliseconds since the last execution.
 */
void UserRobot::fixedLoop(int delayed, int elapsed) {
	/*Serial.print("Fixed loop delayed ");
	 Serial.print(delayed);
	 Serial.println("ms.");*/
}

/*
 * Called as fast as possible. Guaranteed to be called at least once
 * between every call to commLoop.
 */
void UserRobot::fastLoop(void) {
	ControlData *control = comm->controlData;
	//Do fast loop stuff here.

	if (!comm->isConnected) {
		setOutputsEnabled(false);
	}
}

/*
 * Processes control data here, and calls the appropriate init and/or loop functions.
 * This code is called every time new control data is received.
 * In theory this will be called at 50hz, but due to network losses, etc. the
 * actual rate will be less than 50hz, and not guaranteed.
 */
void UserRobot::commLoop(void) {
	//Serial.println((int)communication.commandData.joysticks[1].axis[1]);
	//Serial.println("Slow loop: Implement me!");
	Mode mode = comm->controlData->mode;
	//Serial.println((int)mode.data.data);
	if (mode.getEnabled()) {
		setOutputsEnabled(true);
		if (mode.getAutonomous()) {
			if (!autoInitComplete) {
				autonomousInit();
				autoInitComplete = true;
				teleInitComplete = false;
				disabledInitComplete = false;
			}
			autonomousLoop();
		} else {
			if (!teleInitComplete) {
				teleopInit();
				autoInitComplete = false;
				teleInitComplete = true;
				disabledInitComplete = false;
			}
			teleopLoop();
		}
	} else {
		setOutputsEnabled(false);
		if (!disabledInitComplete) {
			disabledInit();
			autoInitComplete = false;
			teleInitComplete = false;
			disabledInitComplete = true;
		}
		disabledLoop();
	}
}
