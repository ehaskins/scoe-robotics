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
	void update(unsigned char data[], unsigned int offset, unsigned int length);
	void getStatusData(unsigned char data[], unsigned int offset, unsigned int &position);

	RobotModelSection Sections[MAX_MODEL_SECTIONS];
	int SectionCount;
};

#endif /* ROBOTMODEL_H_ */
