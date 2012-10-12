/*
 * DutyCycleModelSection.h
 *
 *  Created on: Feb 15, 2012
 *      Author: EHaskins
 */

#ifndef DUTYCYCLEMODELSECTION_H_
#define DUTYCYCLEMODELSECTION_H_

#include "DutyCycleDefinition.h"
#include "RobotModelSection.h"

#define MAX_DUTYCYCLE_PWMS 8
class DutyCycleModelSection : public RobotModelSection {
public:
	DutyCycleModelSection();
	DutyCycleDefinition dutyCycleDefinitions[MAX_DUTYCYCLE_PWMS];
	unsigned int minPulse;
	unsigned int maxPulse;
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
	virtual void disableOutputs();
	virtual void enableOutputs();
	bool isEnabled;
	bool newLoop;
private:
	bool isOutputEnabled;
};

#endif /* DUTYCYCLEMODELSECTION_H_ */
