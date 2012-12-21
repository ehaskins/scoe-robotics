/*
 * EncoderModelSection.h
 *
 *  Created on: Feb 19, 2012
 *      Author: EHaskins
 */

#ifndef ENCODERMODELSECTION_H_
#define ENCODERMODELSECTION_H_

#include "EncoderDefinition.h"
#include "RobotModelSection.h"

#define MAX_ENCODERS 6

class EncoderModelSection : public RobotModelSection {
public:
	EncoderModelSection();
	EncoderDefinition encoders[MAX_ENCODERS];
	unsigned char encoderCount;
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
};

#endif /* ENCODERMODELSECTION_H_ */
