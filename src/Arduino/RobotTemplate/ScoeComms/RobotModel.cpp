/*
 * RobotModel.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "RobotModel.h"
#include <WProgram.h>
#include <FrcComms\ByteReader.h>
#include <FrcComms\ByteWriter.h>

RobotModel::RobotModel() {
	// TODO Auto-generated constructor stub
	init();
}
void RobotModel::init(){
	_sectionCount = 0;
}

void RobotModel::update(unsigned char data[], unsigned int offset, unsigned int length){
	unsigned char sectionCount = data[offset++];

	for (int i = 0; i < sectionCount; i++){
		unsigned char sectionId = data[offset++];
		int temp = (int)offset;
		unsigned short length = readUInt16(data, &temp); offset+=2;
		for (int iSection = 0; iSection < _sectionCount; iSection++){
			if (sections[iSection]->sectionId == sectionId){
				sections[iSection]->isActive = true;
				sections[iSection]->update(data, offset);
			}
		}
		offset += length;
	}
}
void RobotModel::getStatus(unsigned char data[], unsigned int *offset){
	data[(*offset)++] = _sectionCount;
	for (int i = 0; i < _sectionCount; i++){
		unsigned short headerPos = *offset;
		data[(*offset)++] = sections[i]->sectionId;
		(*offset) += 2;
		unsigned short start = *offset;

		if (sections[i]->isActive)
			sections[i]->getStatus(data, offset);
		//Write length to header
		unsigned short length = (*offset) - start;
		writeUInt16(data, length, headerPos + 1);
	}
}

void RobotModel::loop(bool safteyTripped){
	for (int i = 0; i < _sectionCount; i++){
		if (sections[i]->isActive)
			sections[i]->loop(safteyTripped);
	}
}

bool RobotModel::addSection(RobotModelSection *section){
	if (_sectionCount < MAX_MODEL_SECTIONS){
		sections[_sectionCount++] = section;
		Serial.print("Added section:");
		Serial.println((int)(section->sectionId));
		Serial.print("Section count:");
		Serial.println(_sectionCount);
		return true;
	}
	else
	{
		Serial.print("Too many sections:");
		Serial.print(_sectionCount);
		return false;
	}
}
