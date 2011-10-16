/*
 * FRCCommunication.h
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#ifndef FRCCOMMUNICATION_H_
#define FRCCOMMUNICATION_H_

#include "ControlData.h"
#include "StatusData.h"
#include "RobotStatusLight.h"
#include "../Udp.h"
#include "Configuration.h"

const int PACKET_LOSS_TIMEOUT = 1000;

struct FRCCommunication{
private:
	int counter;
	static FRCCommunication *instance;
	UdpClass socket;
	Configuration *config;
public:
	static FRCCommunication * getInstance(void);
	unsigned char mac[6];
	unsigned char ip[4];
	unsigned char subnet[4];
	unsigned char gateway[4];

	void init();
	bool newDataReady(void);
	void sendData(void);

	bool isConnected;
	bool isEStoped;
	ControlData *controlData;
	StatusData *statusData;
	RobotStatusLight rsl;
	unsigned long lastPacketReceivedTime;

	unsigned char commandBytes[CONTROL_PACKET_SIZE];
	unsigned char statusBytes[STATUS_PACKET_SIZE];
};
#endif /* FRCCOMMUNICATION_H_ */
