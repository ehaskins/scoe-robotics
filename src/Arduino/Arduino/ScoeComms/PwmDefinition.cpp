/*
 * PwmDefinition.cpp
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#include "PwmDefinition.h"
#include <Arduino.h>
PwmDefinition::PwmDefinition() {
	// TODO Auto-generated constructor stub

}

void PwmDefinition::update(unsigned char pin, unsigned char value){
	if (pin != lastPin && pin != 0){
		pwmDriver.detach();
		pwmDriver.attach(pin);
	}

	if (pin != 0 && !isAttached){
		pwmDriver.attach(pin);
		isAttached = true;
	}
	else if (pin == 0 && isAttached){
		pwmDriver.detach();
		isAttached = false;
	}

	if (isAttached)
		pwmDriver.writeMicroseconds(map(value, 0, 255, minPulse, maxPulse));

	lastPin = pin;
	lastValue = value;
}
