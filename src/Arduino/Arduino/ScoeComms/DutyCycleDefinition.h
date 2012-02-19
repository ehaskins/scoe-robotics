/*
 * DutyCycleDefinition.h
 *
 *  Created on: Feb 15, 2012
 *      Author: EHaskins
 */

#ifndef DUTYCYCLEDEFINITION_H_
#define DUTYCYCLEDEFINITION_H_

class DutyCycleDefinition {
public:
public:
	DutyCycleDefinition();
	bool isAttached;
	void update(unsigned char pin, unsigned char value);
	unsigned char lastPin;

	unsigned char lastValue;
private:
};

#endif /* DUTYCYCLEDEFINITION_H_ */
