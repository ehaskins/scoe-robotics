/*
 * RobotModel.cpp
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#include "RobotModel.h"

RobotModel::RobotModel() {
	// TODO Auto-generated constructor stub

}

void RobotModel::update(unsigned char data[], unsigned int offset, unsigned int length){
	unsigned char sectionCount = data[offset++];

	for (int i = 0; i < sectionCount; i++){
		unsigned char sectionId = data[offset++];
		for (int iSection = 0; iSection < sectionCount; iSection++){
			if (sections[iSection]->sectionId == sectionId)
				sections[iSection]->update(false, data, offset);
		}
	}

}
void RobotModel::getStatus(unsigned char data[], unsigned int offset, unsigned short *position){
	for (int i = 0; i < sectionCount; i++){
		sections[i]->getStatus(data, offset, position);
	}
}

bool RobotModel::addSection(RobotModelSection *section){
	if (sectionCount < MAX_MODEL_SECTIONS){
		sections[sectionCount++] = section;
		return true;
	}
	else
		return false;
}
