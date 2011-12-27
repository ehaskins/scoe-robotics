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

void PwmDefinition::update(bool attach, unsigned char pin, unsigned char value){
	if (attach && !isAttached)
		pwmDriver.attach(pin);
	else if (!attach && isAttached)
		pwmDriver.detach();

	if (pin != lastPin && attach){
		pwmDriver.detach();
		pwmDriver.attach(pin);
	}

	if (attach)
		pwmDriver.writeMicroseconds(map(value, 0, 255, minPulse, maxPulse));

	lastPin = pin;
}
