/*
 * AnalogIOSection.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "AnalogIOSection.h"
#include <ScoeComms\ByteWriter.h>
#include <ScoeComms\ByteReader.h>
AnalogIOSection::AnalogIOSection() {
	sectionId = 3;
	inputCount = 0;
	for (int i = 0; i < MAX_ANALOGINPUT; i++){
		inputDefinitions[i].sampleCount = 0;
	}
}

void AnalogIOSection::update(unsigned char data[], unsigned int offset) {
	inputCount = data[offset++];
	for (int i = 0; i < inputCount; i++) {
		if (i < MAX_ANALOGINPUT) {
			inputDefinitions[i].pin = data[offset++];
			inputDefinitions[i].enabled = true;
			inputDefinitions[i].sampleInterval = readUInt16(data, &offset);
			offset += 2;
		}
	}
	for (int i = inputCount; i < MAX_ANALOGINPUT; i++) {
		inputDefinitions[i].enabled = false;
	}
}

void AnalogIOSection::loop(bool safteyTripped) {
	for (int i = 0; i < inputCount; i++) {
		inputDefinitions[i].update();
	}
}

void AnalogIOSection::getStatus(unsigned char data[], unsigned int *offset) {
	data[(*offset)++] = inputCount;
	for (int i = 0; i < inputCount; i++) {
		inputDefinitions[i].commLoop();
		data[(*offset)++] = inputDefinitions[i].pin;
		unsigned char samples = inputDefinitions[i].sampleCount;
		data[(*offset)++] = samples;

		for (int iSample = 0; i < samples; i++) {
			writeUInt32(data, 0x01020304, *offset); // inputDefinitions[i].samples[iSample].delay, *offset);
			(*offset) += 4;
			writeUInt16(data, 0x0506, *offset); //inputDefinitions[i].samples[iSample].value, *offset);
			(*offset) += 2;
		}
	}

	/*for (int j = 0; j < 6; j++){
		data[(*offset)++] = 255 - j;
	}*/
}
