/*
 * DioSection.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "DioSection.h"
#include <FrcComms\ByteReader.h>
#include <FrcComms\ByteWriter.h>
DioSection::DioSection() {
	// TODO Auto-generated constructor stub

}

void DioSection::update(unsigned char data[], unsigned int offset) {
	int position = (int)offset;
	inUse = readUInt32(data, &position);
	mode = readUInt32(data, &position);
	state = readUInt32(data, &position);

	for (int i = 0; i < 32; i++) {
		unsigned long bit = 2 ^ i;
		if ((inUse & bit) == bit) {
			if ((mode & bit) == bit) {
				bool pinState = (state & bit) == bit;
				pinMode(i, OUTPUT);
				digitalWrite(i, pinState ? HIGH : LOW);
			}
			else{
				pinMode(i, INPUT);
				bool value = digitalRead(i);
				if (value){
					state |= bit;
				}
				else{
					state &= ~bit;
				}
			}
		}
	}
}

void DioSection::getStatus(unsigned char data[], unsigned int *offset) {
	writeUInt32(data, inUse, (int)offset); offset +=4;
	writeUInt32(data, mode, (int)offset); offset +=4;
	writeUInt32(data, state, (int)offset); offset +=4;
}
