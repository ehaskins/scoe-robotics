/*
 * AnalogIOSection.h
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#ifndef ANALOGIOSECTION_H_
#define ANALOGIOSECTION_H_

#include "AnalogIODefinition.h"
#include "RobotModelSection.h"

#define MAX_ANALOGINPUT 8
class AnalogIOSection : public RobotModelSection {
public:
	AnalogIOSection();
	unsigned char inputCount;
	AnalogIODefinition inputDefinitions[MAX_ANALOGINPUT];
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
	virtual void loop(bool safteyTripped);
};

#endif /* ANALOGIOSECTION_H_ */
