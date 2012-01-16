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
	unsigned char sectionId;
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int offset, unsigned short *position);
	virtual void loop(bool robotEnabled);
	virtual void disableOutputs();
	virtual void enableOutputs();
private:
	bool lastIsSafteyTripped;
};

#endif /* ROBOTMODELSECTION_H_ */
