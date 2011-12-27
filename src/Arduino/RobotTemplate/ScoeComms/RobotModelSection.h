/*
 * RobotModelSection.h
 *
 *  Created on: Dec 26, 2011
 *      Author: EHaskins
 */

#ifndef ROBOTMODELSECTION_H_
#define ROBOTMODELSECTION_H_

class RobotModelSection {
public:
	RobotModelSection();
	int sectionId;
	virtual void update(bool robotDisabled, unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int offset, unsigned short *position);
};

#endif /* ROBOTMODELSECTION_H_ */
