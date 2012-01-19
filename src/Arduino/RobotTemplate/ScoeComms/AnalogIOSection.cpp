/*
 * AnalogIOSection.cpp
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#include "AnalogIOSection.h"
#include <FrcComms\ByteWriter.h>
AnalogIOSection::AnalogIOSection() {
	// TODO Auto-generated constructor stub
	inputCount = 0;
}

void AnalogIOSection::update(unsigned char data[], unsigned int offset){
	inputCount = data[offset++];
	for(int i = 0; i < inputCount; i++){
		inputDefinitions[i].pin = data[offset++];
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
