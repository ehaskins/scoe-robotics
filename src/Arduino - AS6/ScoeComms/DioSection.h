/*
 * DioSection.h
 *
 *  Created on: Jan 18, 2012
 *      Author: EHaskins
 */

#ifndef DIOSECTION_H_
#define DIOSECTION_H_

#include "RobotModelSection.h"

class DioSection : public RobotModelSection{
public:
	DioSection();
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
private:
	unsigned long inUse;
	unsigned long mode;
	unsigned long state;
};

#endif /* DIOSECTION_H_ */
