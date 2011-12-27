/*
 * PwmModelSection.cpp
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#include "PwmModelSection.h"

PwmModelSection::PwmModelSection(unsigned int minPulse, unsigned int maxPulse) {
	sectionId=1;
	this->minPulse = minPulse;
	this->maxPulse = maxPulse;

	for(int i = 0; i < MAX_PWMS; i++){
		pwmDefinitions[i].maxPulse = maxPulse;
		pwmDefinitions[i].minPulse = minPulse;
	}
}

void PwmModelSection::update(bool robotDisabled, unsigned char data[], unsigned int offset){
	unsigned char count = data[offset++];
	if (count > MAX_PWMS)
		count = MAX_PWMS;

	for (unsigned char i = 0; i < count; i++){
		unsigned char pin = data[offset++];
		unsigned char value = data[offset++];

		pwmDefinitions[i].update(!robotDisabled, pin, value);
	}
	if (count < MAX_PWMS){
		for (int i = count; i < MAX_PWMS; i++){
			pwmDefinitions[i].update(false, 0, 0);
		}
	}
}
void PwmModelSection::getStatus(unsigned char data[], unsigned int offset, unsigned short *position){
	//PWM has no status
}
