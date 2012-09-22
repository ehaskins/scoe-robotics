/*
 * AnalogIODefinition.h
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#ifndef ANALOGIODEFINITION_H_
#define ANALOGIODEFINITION_H_

class AnalogIODefinition {
public:
	AnalogIODefinition();
	unsigned char pin;
	unsigned int value;
	bool enabled;
	void update();
};

#endif /* ANALOGIODEFINITION_H_ */
