/*
 * RobotModelSection.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "RobotModelSection.h"

RobotModelSection::RobotModelSection() {
	// TODO Auto-generated constructor stub
	lastIsSafteyTripped = false;
}

void RobotModelSection::update(unsigned char data[], unsigned int offset){

}
void RobotModelSection::getStatus(unsigned char data[], unsigned int offset, unsigned short *position){

}

void RobotModelSection::loop(bool safteyTripped){
	if (safteyTripped && !lastIsSafteyTripped)
		enableOutputs();
	else if (!safteyTripped && lastIsSafteyTripped){
		disableOutputs();
	}
}
void RobotModelSection::disableOutputs(){}
void RobotModelSection::enableOutputs(){}
