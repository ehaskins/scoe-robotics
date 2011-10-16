/*
 * FRCCommunication.cpp
 *
 *  Created on: Feb 13, 2011
 *      Author: EHaskins
 */

#include "FRCCommunication.h"
#include "ControlData.h"
#include "../Ethernet.h"
#include "../Udp.h"
#include "CommConstants.h"
#include "Configuration.h"
#include "cpp.h"
byte remoteIp[4]; // holds received packet's originating IP
unsigned int remotePort; // holds received packet's originating port

FRCCommunication * FRCCommunication::instance;
FRCCommunication * FRCCommunication::getInstance(){
	if (!instance)
		instance = new FRCCommunication();
	return instance;
}

void FRCCommunication::init() {
	rsl.init(13);
	config = Configuration::getInstance();
	statusData = new StatusData();
	controlData = new ControlData();
	Serial.print("FRCComm socket #");
	Serial.println(socket.begin(config->controlReceivePort));
	statusData->teamNumber = config->teamNumber;
	Serial.print("Control User Data ");
	Serial.print(USER_CONTROL_DATA_SIZE);
	Serial.print("bytes, with a total size of ");
	Serial.println(CONTROL_PACKET_SIZE);
	Serial.print("Status User Data ");
	Serial.print(USER_STATUS_DATA_SIZE);
	Serial.print("bytes, with a total size of ");
	Serial.println(STATUS_PACKET_SIZE);
	Serial.println("Communication init complete.");
	Serial.print("Control port:");
	Serial.print(config->controlReceivePort);
	Serial.print(" Status port:");
	Serial.println(config->statusTransmitPort);
}

bool FRCCommunication::newDataReady() {
	counter++;
	int packetSize = socket.available(); // note that this includes the UDP header
	bool droppedPacket = false;
	packetSize -= 8; //subtract the 8 byte header;

	rsl.update(isConnected, droppedPacket, statusData->mode);
	if (packetSize == CONTROL_PACKET_SIZE) {
		// read the packet into packetBufffer and get the senders IP addr and port number
		socket.readPacket((char *)commandBytes, CONTROL_PACKET_SIZE, remoteIp, remotePort);

		lastPacketReceivedTime = millis();

		controlData->parse(commandBytes);

		if (!controlData->isValid){
			isConnected = false;
			Serial.println("Invalid packet received.");
		}

		//Implement E-Stop safety
		if (controlData->mode.getEStop()){
			if (!isEStoped){
				Serial.println("E-STOP SET");
			}
			isEStoped = true;
		}

		if (isEStoped){
			controlData->mode.setEStop(true);
			controlData->mode.setEnabled(false);
		}

		if (statusData->replyId > controlData->packetId && !controlData->mode.getResync()){
			Serial.print("Lower packet ID without resync: PacketID:");
			Serial.print(controlData->packetId);
			Serial.print(" Reply Id:");
			Serial.println(statusData->replyId);
			isConnected = false;
		}
		else{
			statusData->replyId = controlData->packetId;
			statusData->mode = controlData->mode;
		}
		return true;
	}
	else if (packetSize > 0){
		char garbage[2048];
		packetSize = packetSize <= 2048 ? packetSize : 2048;
		socket.readPacket(garbage, packetSize, remoteIp, remotePort);
//		Serial.print("PacketSize:");
//		Serial.println(packetSize);
		Serial.print('*');
		droppedPacket = true;
	}

	unsigned long now = millis();
	if (lastPacketReceivedTime + PACKET_LOSS_TIMEOUT < now){ //No Comms
		isConnected = false;
	}

	if (!isConnected){
		controlData->mode.setEnabled(false);
	}

	return false;
}

void FRCCommunication::sendData() {
	isConnected = !controlData->mode.getResync();
	statusData->getResponseBytes(statusBytes);
	socket.sendPacket(statusBytes, STATUS_PACKET_SIZE, remoteIp, config->statusTransmitPort);
}
