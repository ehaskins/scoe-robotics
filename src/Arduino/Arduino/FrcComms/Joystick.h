/*
 * Joystick.h
 *
 *  Created on: Feb 12, 2011
 *      Author: EHaskins
 */

#ifndef JOYSTICK_H_
#define JOYSTICK_H_

#include "BitField16.h"

const int NUM_AXES = 6;

struct Joystick{
	void parse(unsigned char[], int*);
	signed char axis[NUM_AXES];
	BitField16 buttons;
};

#endif /* JOYSTICK_H_ */
