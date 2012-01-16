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
	sectionCount = 0;
}

void RobotModel::update(unsigned char data[], unsigned int offset, unsigned int length){
	unsigned char sectionCount = data[offset++];

	for (int i = 0; i < sectionCount; i++){
		unsigned char sectionId = data[offset++];
		int temp = (int)offset;
		unsigned short length = readUInt16(data, &temp); offset+=2;
		for (int iSection = 0; iSection < sectionCount; iSection++){
			if (sections[iSection]->sectionId == sectionId)
				sections[iSection]->update(data, offset);
		}
		offset += length;
	}

}
void RobotModel::getStatus(unsigned char data[], unsigned int offset, unsigned short *position){
	data[*position] = sectionCount;
	for (int i = 0; i < sectionCount; i++){
		unsigned short headerPos = *position;
		unsigned short start = headerPos + 3;
		(*position) += 2;
		data[headerPos] = sections[i]->sectionId;

		sections[i]->getStatus(data, offset, position);
		//Write length to header
		unsigned short length = (*position) - start;
		writeUInt16(data, length, headerPos + 1);
	}
}

void RobotModel::loop(bool safteyTripped){
	for (int i = 0; i < sectionCount; i++){
		sections[i]->loop(safteyTripped);
	}
}

bool RobotModel::addSection(RobotModelSection *section){
	if (sectionCount < MAX_MODEL_SECTIONS){
		sections[sectionCount++] = section;
		Serial.println("Added section");
		return true;
	}
	else
	{
		Serial.print("Too many sections:");
		Serial.print(sectionCount);
		return false;
	}
}
