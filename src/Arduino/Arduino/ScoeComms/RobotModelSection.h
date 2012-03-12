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
	bool isActive;
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
	virtual void loop(bool robotEnabled);
	virtual void disableOutputs();
	virtual void enableOutputs();
private:
	bool isFirstLoop;
	bool lastIsSafteyTripped;
protected:
	void init();
};

#endif /* ROBOTMODELSECTION_H_ */
