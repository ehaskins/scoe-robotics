/*
 * RslModelSection.cpp
 *
 *  Created on: Jan 14, 2012
 *      Author: EHaskins
 */

#include "RslModelSection.h"

RslModelSection::RslModelSection() {
	sectionId = 0;
	pin = 13;
	state = 0;
	isActive = true;
	pinMode(pin, OUTPUT);
}
void RslModelSection::update(unsigned char data[], unsigned int offset){
	state = data[offset++];
}
void RslModelSection::getStatus(unsigned char data[], unsigned int *offset){
	//RSL has no status
}
void RslModelSection::loop(bool safteyTripped){
	RobotModelSection::loop(safteyTripped);
	driveLight(safteyTripped);
}
void RslModelSection::driveLight(bool safteyTripped){
	if (safteyTripped){
		driveNoBeagleComm();
		state = 255;
	}
	else{
		switch (state){
		case RSL_NOSTATE:
			driveNoState();
			break;
		case RSL_NOFRCCOMM:
			driveNoFrcComm();
			break;
		case RSL_ENABLED:
			driveEnabled();
			break;
		case RSL_DISABLED:
			driveDisabled();
			break;
		case RSL_AUTONOMOUS:
			driveAutonomous();
			break;
		case RSL_ESTOPPED:
			driveEStopped();
			break;
		}
	}
}

void RslModelSection::driveEStopped(){
	driveDisabled();
}
void RslModelSection::driveNoState(){
	if (millis() % 400 < 100)
		digitalWrite(pin, HIGH);
	else
		digitalWrite(pin, LOW);
}

void RslModelSection::driveNoBeagleComm(){
	if (millis() % 200 < 100)
		digitalWrite(pin, HIGH);
	else
		digitalWrite(pin, LOW);
}
void RslModelSection::driveNoFrcComm(){
	if (millis() % 900 < 100)
		digitalWrite(pin, HIGH);
	else
		digitalWrite(pin, LOW);
}
void RslModelSection::driveEnabled(){
	unsigned long m = millis();
	if (m % 1200 < 50)
		digitalWrite(pin, LOW);
	else if (m % 1200 < 150)
		digitalWrite(pin, HIGH);
	else if (m % 1200 < 200)
		digitalWrite(pin, LOW);
	else
		digitalWrite(pin, HIGH);

}
void RslModelSection::driveDisabled(){
	if (millis() % 2000 < 1000)
		digitalWrite(pin, HIGH);
	else
		digitalWrite(pin, LOW);
}
void RslModelSection::driveAutonomous(){
	unsigned long m = millis();
	if (m % 1200 < 50)
		digitalWrite(pin, LOW);
	else if (m % 1200 < 150)
		digitalWrite(pin, HIGH);
	else if (m % 1200 < 250)
		digitalWrite(pin, LOW);
	else if (m % 1200 < 350)
		digitalWrite(pin, HIGH);
	else if (m % 1200 < 400)
		digitalWrite(pin, LOW);
	else
		digitalWrite(pin, HIGH);
}

