/*
 * DutyCycleModelSection.cpp
 *
 *  Created on: Feb 15, 2012
 *      Author: EHaskins
 */

#include "DutyCycleModelSection.h"

#include <Arduino.h>
DutyCycleModelSection::DutyCycleModelSection() {
	init();
	sectionId = 4;

	for (int i = 0; i < MAX_DUTYCYCLE_PWMS; i++) {
		dutyCycleDefinitions[i].isAttached = false;
	}
}

void DutyCycleModelSection::update(unsigned char data[], unsigned int offset) {
	unsigned char count = 0;
	newLoop = true;

	if (isEnabled){
		count = data[offset++];
		if (count > MAX_DUTYCYCLE_PWMS)
			count = MAX_DUTYCYCLE_PWMS;
	}

	for (unsigned char i = 0; i < count; i++) {
		unsigned char pin = data[offset++];
		unsigned char value = data[offset++];

		dutyCycleDefinitions[i].update(pin, value);
	}
	if (count < MAX_DUTYCYCLE_PWMS) {
		for (int i = count; i < MAX_DUTYCYCLE_PWMS; i++) {
			dutyCycleDefinitions[i].update(0, 0);
		}
	}
}
void DutyCycleModelSection::getStatus(unsigned char data[], unsigned int *offset) {
	//PWM has no status
}

void DutyCycleModelSection::disableOutputs() {
	isEnabled = false;
	for(int i = 0; i < MAX_DUTYCYCLE_PWMS; i++){
		dutyCycleDefinitions[i].update(0, 0);
	}
}

void DutyCycleModelSection::enableOutputs(){
	isEnabled = true;
}
