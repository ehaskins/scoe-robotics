/*
 * PwmDefinition.h
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#ifndef PWMDEFINITION_H_
#define PWMDEFINITION_H_

#include "..\Servo\Servo.h"

class PwmDefinition {
public:
	PwmDefinition();
	bool isAttached;
	unsigned int minPulse;
	unsigned int maxPulse;
	Servo pwmDriver;
	void update(unsigned char pin, unsigned char value);
	unsigned char lastPin;

	unsigned char lastValue;
private:

};

#endif /* PWMDEFINITION_H_ */
