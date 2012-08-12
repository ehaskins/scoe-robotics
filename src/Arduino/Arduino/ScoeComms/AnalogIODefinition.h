/*
 * AnalogIODefinition.h
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#ifndef ANALOGIODEFINITION_H_
#define ANALOGIODEFINITION_H_

#include "AnalogIOSample.h"

#define MAX_AIO_SAMPLES 4
class AnalogIODefinition {
public:
	AnalogIODefinition();
	unsigned char pin;
	unsigned long lastSampleTime;
	unsigned int sampleInterval;
	unsigned int sampleCount;
	AnalogIOSample samples[MAX_AIO_SAMPLES];
	bool enabled;
	void update();
	void update(bool force);
	void commLoop();
};

#endif /* ANALOGIODEFINITION_H_ */
