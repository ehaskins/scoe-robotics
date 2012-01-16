/*
 * PwmDefinition.h
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#ifndef PWMDEFINITION_H_
#define PWMDEFINITION_H_

#include <Servo.h>

class PwmDefinition {
public:
	PwmDefinition();
	bool isAttached;
	unsigned int minPulse;
	unsigned int maxPulse;
	Servo pwmDriver;
	void update(unsigned char pin, unsigned char value);
private:
	unsigned char lastPin;
};

#endif /* PWMDEFINITION_H_ */
