/*
 * PwmModelSection.cpp
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#include "PwmModelSection.h"

PwmModelSection::PwmModelSection(unsigned int minPulse, unsigned int maxPulse) {
	init();
	sectionId=1;
	this->minPulse = minPulse;
	this->maxPulse = maxPulse;

	for(int i = 0; i < MAX_PWMS; i++){
		pwmDefinitions[i].maxPulse = maxPulse;
		pwmDefinitions[i].minPulse = minPulse;
	}
}

void PwmModelSection::update(unsigned char data[], unsigned int offset){
	unsigned char count = data[offset++];
	if (count > MAX_PWMS)
		count = MAX_PWMS;

	for (unsigned char i = 0; i < count; i++){
		unsigned char pin = data[offset++];
		unsigned char value = data[offset++];

		pwmDefinitions[i].update(pin, value);
	}
	if (count < MAX_PWMS){
		for (int i = count; i < MAX_PWMS; i++){
			pwmDefinitions[i].update(0, 0);
		}
	}
}
void PwmModelSection::getStatus(unsigned char data[], unsigned int *offset){
	//PWM has no status
}

void PwmModelSection::disableOutputs(){

}
