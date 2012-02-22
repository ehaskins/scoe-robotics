/*
 * RobotModel.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "RobotModel.h"
#include <Arduino.h>
#include <ScoeComms\ByteReader.h>
#include <ScoeComms\ByteWriter.h>

RobotModel::RobotModel() {
	// TODO Auto-generated constructor stub
	init();
}
void RobotModel::init(){
	_sectionCount = 0;
}

void RobotModel::update(uint8_t data[], uint16_t offset, uint16_t length){
	uint8_t sectionCount = data[offset++];

	for (int i = 0; i < sectionCount; i++){
		uint8_t sectionId = data[offset++];
		uint16_t length = readUInt16(data, &offset); offset+=2;
		for (int iSection = 0; iSection < _sectionCount; iSection++){
			if (sections[iSection]->sectionId == sectionId){
				sections[iSection]->isActive = true;
				sections[iSection]->update(data, offset);
			}
		}
		offset += length;
	}
}
void RobotModel::getStatus(uint8_t data[], uint16_t *offset){
	data[(*offset)++] = _sectionCount;
	for (int i = 0; i < _sectionCount; i++){
		uint16_t headerPos = *offset;
		data[(*offset)++] = sections[i]->sectionId;
		(*offset) += 2; //Leave space for length
		uint16_t start = *offset;

		if (sections[i]->isActive)
			sections[i]->getStatus(data, offset);
		//Write length to header
		uint16_t length = (*offset) - start;
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
		return true;
	}
	else
	{
		Serial.print("Too many sections:");
		Serial.print(_sectionCount);
		return false;
	}
}
