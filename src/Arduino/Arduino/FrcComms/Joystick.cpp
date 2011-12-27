/*
 * Joystick.cpp
 *
 *  Created on: Feb 15, 2011
 *      Author: EHaskins
 */
#include "Joystick.h"
#include "ByteReader.h"
#include "../WProgram.h"

void Joystick::parse(unsigned char data[], int *offset) {
	for (int i = 0; i < NUM_AXES; i++){
		axis[i] = (char)((int)readUInt8(data, offset) - 128);
	}
	buttons.data = readUInt16(data, offset);
}
