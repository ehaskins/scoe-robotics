/*
 * AnalogIOSection.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "AnalogIOSection.h"
#include "ByteWriter.h"
AnalogIOSection::AnalogIOSection() {
	// TODO Auto-generated constructor stub
	sectionId = 3;
	inputCount = 0;
}

void AnalogIOSection::update(unsigned char data[], unsigned int offset){
	inputCount = data[offset++];
	for(int i = 0; i < inputCount; i++){
		if (i < MAX_ANALOGINPUT){
			inputDefinitions[i].pin = data[offset++];
			inputDefinitions[i].enabled = true;
		}
	}
	for (int i = inputCount; i < MAX_ANALOGINPUT; i++){
		inputDefinitions[i].enabled = false;
	}
}

void AnalogIOSection::getStatus(unsigned char data[], unsigned int *offset){
	data[(*offset)++] = inputCount;
	for (int i = 0; i < inputCount; i++){
		inputDefinitions[i].update();
		data[(*offset)++] = inputDefinitions[i].pin;
		writeUInt16(data, inputDefinitions[i].value, *offset);
		(*offset)+=2;
	}
}
