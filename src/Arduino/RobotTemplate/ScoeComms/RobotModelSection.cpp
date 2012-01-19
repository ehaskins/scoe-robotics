/*
 * RobotModelSection.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "RobotModelSection.h"

RobotModelSection::RobotModelSection() {
	init();
}

void RobotModelSection::init(){
	lastIsSafteyTripped = false;
	isActive = false;
}

void RobotModelSection::update(unsigned char data[], unsigned int offset){

}
void RobotModelSection::getStatus(unsigned char data[], unsigned int *offset){

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
