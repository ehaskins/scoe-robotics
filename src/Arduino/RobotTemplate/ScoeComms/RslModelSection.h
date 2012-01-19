/*
 * RslModelSection.h
 *
 *  Created on: Jan 14, 2012
 *      Author: EHaskins
 */

#ifndef RSLMODELSECTION_H_
#define RSLMODELSECTION_H_

#include <WProgram.h>
#include "RobotModelSection.h"

#define RSL_NOFRCCOMM 0
#define RSL_ENABLED 1
#define RSL_DISABLED 2
#define RSL_AUTONOMOUS 3
#define RSL_ESTOPPED 4
#define RSL_NOSTATE 255

class RslModelSection : public RobotModelSection {
public:
	RslModelSection();
	virtual void update(unsigned char data[], unsigned int offset);
	virtual void getStatus(unsigned char data[], unsigned int *offset);
	virtual void loop(bool safteyTripped);
private:
	unsigned int pin;
	unsigned char state;
	void driveLight(bool safteyTripped);
	void driveNoBeagleComm();
	void driveNoFrcComm();
	void driveEnabled();
	void driveDisabled();
	void driveAutonomous();
	void driveNoState();
	void driveEStopped();
};

#endif /* RSLMODELSECTION_H_ */
