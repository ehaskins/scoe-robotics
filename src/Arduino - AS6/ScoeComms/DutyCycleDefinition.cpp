/*
 * DutyCycleDefinition.cpp
 *
 *  Created on: Feb 15, 2012
 *      Author: EHaskins
 */

#include "DutyCycleDefinition.h"

DutyCycleDefinition::DutyCycleDefinition() {
	// TODO Auto-generated constructor stub

}

void DutyCycleDefinition::update(unsigned char pin, unsigned char value){
	if (pin != lastPin && pin != 0){
		digitalWrite(lastPin, LOW);
		pinMode(pin, OUTPUT);
	}

	if (pin != 0 && !isAttached){
		pinMode(pin, OUTPUT);
		isAttached = true;
	}
	else if (pin == 0 && isAttached){
		digitalWrite(lastPin, LOW);
		isAttached = false;
	}

	if (isAttached)
		analogWrite(pin, value);

	lastPin = pin;
	lastValue = value;
}
