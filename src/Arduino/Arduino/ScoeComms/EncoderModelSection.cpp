/*
 * EncoderModelSection.cpp
 *
 *  Created on: Feb 19, 2012
 *      Author: EHaskins
 */

#include "EncoderModelSection.h"
#include "ByteWriter.h"
EncoderModelSection::EncoderModelSection() {
	init();
	sectionId = 5;
	encoderCount = 0;
	for (int i = 0; i < MAX_ENCODERS; i++){
		encoders[i].isConfigured = false;
		encoders[i].fault = false;
	}
}
void EncoderModelSection::update(unsigned char data[], unsigned int offset) {
	unsigned char count = data[offset++];
	if (count > MAX_ENCODERS)
		count = MAX_ENCODERS;

	for (int i = 0; i < count; i++) {
		unsigned char pinA = data[offset++];
		unsigned char pinB = data[offset++];

		if (!encoders[i].isConfigured) {
			encoders[i].init(pinA, pinB);
			encoderCount++;
		} else if (encoders[i].pinA != pinA || encoders[i].pinB != pinB) {
			encoders[i].fault = true;
		}
	}
}
void EncoderModelSection::getStatus(unsigned char data[], unsigned int *offset) {
	data[(*offset)++] = encoderCount;
	for (unsigned char i = 0; i < encoderCount; i++) {
		unsigned char * pos = data + *offset;
		long * ticks = (long *) pos;
		*ticks = encoders[i].read();
		(*offset) += 4;
		data[(*offset)++] = encoders[i].fault ? 1 : 0;
	}
}
