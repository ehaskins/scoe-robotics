/*
 * AnalogGyroSections.h
 *
 *  Created on: Aug 13, 2012
 *      Author: EHaskins
 */

#ifndef ANALOGGYROSECTIONS_H_
#define ANALOGGYROSECTIONS_H_

#include "RobotModelSection.h"
#include "AnalogGyroDefinition.h"

#define MAX_GYROS 3

class AnalogGyroSection : public RobotModelSection {
public:
	AnalogGyroSection();
	unsigned char inputCount;
	AnalogGyroDefinition inputDefinitions[MAX_GYROS];
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
	virtual void loop(bool safteyTripped);
};

#endif /* ANALOGGYROSECTIONS_H_ */
