/*
 * UserCode.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef USERCODE_H_
#define USERCODE_H_
#include <Servo.h>
#include <FrcComms\Configuration.h>
#include <FrcComms\FRCCommunication.h>
#include "Drivers/Drivers.h"

//Declared and initialized in main.
class UserRobot
{
private:
	int teleopCounter;

	int start;
	int position;
	int direction;
	int range;
	int autonomousCounter;

	bool autoInitComplete;
	bool teleInitComplete;
	bool disabledInitComplete;

	bool liftState;
	bool lastLiftInput;
	bool lastGateInput;
	bool gateState;
	bool attached;
	Servo pan;
	Servo tilt;
	Servo right;
	Servo left;
	Servo rear;
	Servo lift;
	Servo shooter;
	Servo gate;

	Configuration *config;
	FRCCommunication *comm;
public:
	void fastLoop(void);
	void commLoop(void);

	void teleopInit(void);
	void teleopLoop(void);
	void disabledInit(void);
	void disabledLoop(void);
	void autonomousInit(void);
	void autonomousLoop(void);
	void fixedLoop(int delayed, int elapsed);
	void userInit(void);
	void setOutputsEnabled(bool enabled);
};

#endif /* USERCODE_H_ */
