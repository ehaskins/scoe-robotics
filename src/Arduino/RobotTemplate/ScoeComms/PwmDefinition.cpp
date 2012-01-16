/*
 * PwmDefinition.cpp
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#include "PwmDefinition.h"
#include <WProgram.h>
PwmDefinition::PwmDefinition() {
	// TODO Auto-generated constructor stub

}

void PwmDefinition::update(unsigned char pin, unsigned char value){
	if (pin != 0 && !isAttached)
		pwmDriver.attach(pin);
	else if (pin == 0 && isAttached)
		pwmDriver.detach();

	if (pin != lastPin && pin != 0){
		pwmDriver.detach();
		pwmDriver.attach(pin);
	}

	if (isAttached)
		pwmDriver.writeMicroseconds(map(value, 0, 255, minPulse, maxPulse));

	lastPin = pin;
}
