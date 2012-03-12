/*
 * EchoSection.h
 *
 *  Created on: Feb 21, 2012
 *      Author: EHaskins
 */

#ifndef ECHOSECTION_H_
#define ECHOSECTION_H_
#include "RobotModelSection.h"
#include <stdint.h>
#define MAX_ECHOLENGTH 200
class EchoSection: public RobotModelSection {
public:
	EchoSection();
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
private:
	uint16_t length;
	uint8_t echoData[MAX_ECHOLENGTH];
};

#endif /* ECHOSECTION_H_ */
