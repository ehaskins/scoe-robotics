/*
 * PwmModelSection.h
 *
 *  Created on: Dec 27, 2011
 *      Author: EHaskins
 */

#ifndef PWMMODELSECTION_H_
#define PWMMODELSECTION_H_

#include "RobotModelSection.h"
#include "PwmDefinition.h"

#define MAX_PWMS 8

class PwmModelSection: public RobotModelSection {
public:
	PwmModelSection(unsigned int minPulse, unsigned int maxPluse);
	PwmDefinition pwmDefinitions[MAX_PWMS];
	unsigned int minPulse;
	unsigned int maxPulse;
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int offset, unsigned short *position);
	virtual void disableOutputs();
private:
	bool isOutputEnabled;
};

#endif /* PWMMODELSECTION_H_ */
