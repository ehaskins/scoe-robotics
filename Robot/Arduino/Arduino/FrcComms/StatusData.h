/*
 * StatusData.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef STATUSDATA_H_
#define STATUSDATA_H_

#include "BitField8.h"
#include "Joystick.h"
#include "Mode.h"
#include "ControlData.h"


class StatusData {
public:
	Mode mode;
	unsigned int replyId;
	unsigned char robotMac[6];
	unsigned short teamNumber;
	unsigned char batteryVoltageInt;
	unsigned char batteryVoltageFrac;
	BitField8 dsOutputs;
	unsigned char fpgaVersion[8];
	bool codeRunning;

	void updateResponse(ControlData*);
	void getResponseBytes(unsigned char[]);
};

#endif /* STATUSDATA_H_ */
