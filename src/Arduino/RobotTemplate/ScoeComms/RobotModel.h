/*
 * RobotModel.h
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#ifndef ROBOTMODEL_H_
#define ROBOTMODEL_H_

#include "RobotModelSection.h"

#define MAX_MODEL_SECTIONS 8

class RobotModel {
public:
	RobotModel();
	void init();
	void update(unsigned char data[], unsigned int offset, unsigned int length);
	void getStatus(unsigned char data[], unsigned int *offset);
	void loop(bool safteyTripped);

	RobotModelSection *sections[MAX_MODEL_SECTIONS];
	bool addSection(RobotModelSection *section);
	int sectionCount;
};

#endif /* ROBOTMODEL_H_ */
