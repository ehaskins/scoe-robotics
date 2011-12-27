/*
 * RobotStatusLight.cpp
 *
 *  Created on: Feb 15, 2011
 *      Author: EHaskins
 */

#include "RobotStatusLight.h"
#include "../WProgram.h"

/*
Solid ON = Autonomous Enabled

Solid ON, but blinks OFF every 1.5s = Teleoperated Enabled

Slow Blink (900ms on/ 900ms off) = System Disabled; caused by system watchdog, user watchdog or Driver's Station set to disabled

Fast-Slow Blink (200ms on/900ms off) = Low Battery (<12V) or no user code AND system disabled either by system watchdog, user watchdog, or Driver;s station set to disabled

Fast Blink (200ms on/200ms off) = System error: No driver's station communication, bad cRIO Image, bad team ID, extensive comm. error
 */

void RobotStatusLight::init(int pin){
	ledPin = pin;
	pinMode(ledPin, OUTPUT);
	lastStateChangeTime = millis();
	periods[0].value = true;
	periods[1].value = false;
	periods[2].value = true;
	periods[3].value = false;
}

void RobotStatusLight::update(bool connected, bool droppedPacket, Mode mode){
	if (connected){
		if (mode.getEnabled()){
			if(mode.getAutonomous()){
				periods[0].length = 1500;
				periods[1].length = 0;
				periods[2].length = 0;
				periods[3].length = 0;
			}
			else{
				periods[0].length = 1500;
				periods[1].length = 100;
				periods[2].length = 0;
				periods[3].length = 0;
			}
		}
		else{
			periods[0].length = 900;
			periods[1].length = 900;
			periods[2].length = 0;
			periods[3].length = 0;
		}
	}
	else{
		periods[0].length = 200;
		periods[1].length = 200;
		periods[2].length = 0;
		periods[3].length = 0;
	}

	bool waiting = false;
	//Serial.print(".");

	unsigned long now = millis();
	bool output;
	while (!waiting){
		unsigned long elapsed = now - lastStateChangeTime;
		if (elapsed >= periods[period].length){
			lastStateChangeTime = now;
			period++;
			if (period == NUM_PERIODS)
				period = 0;
		}
		else{
			waiting = true;
		}
		output = periods[period].value;
	}

	if (droppedPacket) droppedStartTime = now;
	if (droppedStartTime != 0){
		unsigned long droppedElapsed = now - droppedStartTime;
		if (droppedElapsed < DROPPED_BLINK_DURATION){
			output = !output;
		}
		else {
			droppedStartTime = 0;
		}
	}

	digitalWrite(ledPin, output ? HIGH : LOW);

	/*	bool output = false;
	unsigned long now = millis();
	  while (!waiting){
		unsigned long elapsed = now - lastStateChangeTime;
		if (elapsed >= periods[period].length){
			lastStateChangeTime = now;
			period++;
			if (period == NUM_PERIODS)
				period = 0;
			//Serial.println(period);
			output = periods[period].value;
		}
		else{
			waiting = true;
		}
	}
	if (droppedPacket) droppedStartTime = now;
	if (droppedStartTime != 0){
		unsigned long droppedElapsed = now - droppedStartTime;
		if (droppedElapsed < DROPPED_BLINK_DURATION){
			output = !output;
		}
		else {
			droppedStartTime = 0;
		}
	}
*/
}
