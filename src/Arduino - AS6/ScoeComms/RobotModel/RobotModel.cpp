/*
* RobotModel.cpp
*
*  Created on: Dec 26, 2011
*      Author: EHaskins
*/

#include "RobotModel.h"
#include <Arduino.h>
#include "..\Utils\ByteReader.h"
#include "..\Utils\ByteWriter.h"

RobotModel::RobotModel() {
	// TODO Auto-generated constructor stub
	init();
}
void RobotModel::init(){
	_sectionCount = 0;
}

void RobotModel::update(unsigned char data[], unsigned int offset, unsigned int length){
	if (data[offset++] == PACKET_VERSION){
		packetIndex = readUInt16(data, &offset);
		uint8_t packetType = data[offset++];
		
		offset+=2; //Ignore the content length.
		if (packetType == 2) {
			uint8_t sectionCount = data[offset++];
			
			for (int i = 0; i < sectionCount; i++){
				unsigned char sectionId = data[offset++];
				
				unsigned short length = readUInt16(data, &offset);
				for (int iSection = 0; iSection < _sectionCount; iSection++){
					if (sections[iSection]->sectionId == sectionId){
						if (!sections[iSection]->isActive){
							sections[iSection]->isActive = true;
							Serial.print("Activated section:");
							Serial.println(sectionId);
						}						
						sections[iSection]->update(data, offset);
					}
				}
				offset += length;
			}

		}
	}
	else{
		//TODO:Invalid packet version. DTC.
	}
}
void RobotModel::getStatus(unsigned char data[], unsigned int *offset){
	data[(*offset)++] = PACKET_VERSION;
	writeUInt16(data, packetIndex, (*offset)); (*offset) += 2;
	data[(*offset)++] = 3; //Set as status packet
	unsigned int contentLengthOffset = *offset;
	(*offset) += 2;
	data[(*offset)++] = _sectionCount;
	
	for (int i = 0; i < _sectionCount; i++){
		unsigned short sectionHeaderOffset = *offset;
		data[(*offset)++] = sections[i]->sectionId;
		(*offset) += 2;
		unsigned short contentStartOffset = *offset;

		if (sections[i]->isActive){
			sections[i]->getStatus(data, offset);
		}		
		//Write length to header
		unsigned short length = (*offset) - contentStartOffset;
		writeUInt16(data, length, sectionHeaderOffset + 1);
	}
	writeUInt16(data, (*offset) - contentLengthOffset, contentLengthOffset);
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
