/*
 * EncoderDefinition.h
 *
 *  Created on: Feb 19, 2012
 *      Author: EHaskins
 */

#ifndef ENCODERDEFINITION_H_
#define ENCODERDEFINITION_H_
#include "..\Encoder\Encoder.h"

class EncoderDefinition {
public:
	EncoderDefinition();
	void init(unsigned char pinA, unsigned char pinB);
	long read();
	bool isConfigured;
	bool fault;
	unsigned char pinA;
	unsigned char pinB;
private:
	Encoder *encoder;
};

#endif /* ENCODERDEFINITION_H_ */
