/*
* UdpComms.h
*
* Created: 12/2/2012 8:32:53 PM
*  Author: EHaskins
*/


#ifndef UDPCOMMS_H_
#define UDPCOMMS_H_
//Time in ms before output is disabled due to no communication.
#define RECEIVE_SAFTEY_DELAY 500 //TODO: Set back to reasonable level

#include <Ethernet\Ethernet.h>
#include <Ethernet\EthernetUdp.h>
#include <RobotModel\RobotModel.h>

class UdpComms {
	public:
UdpComms(){}
byte macAddress[6];
bool isDhcp;
IPAddress ipAddress;
UdpComms(unsigned int receivePort);
bool initSuccessful;
unsigned int receivePort;
bool isConnected;
RobotModel robotModel;



void init(){
	int result;
	if (isDhcp){
		result = Ethernet.begin(macAddress);
	}
	else{
		result = -1;//Ethernet.begin(macAddress, ipAddress);
	}
	
	Udp.begin(receivePort);
	initSuccessful = result == 0;
}

void poll(){
	unsigned long now = millis();
	//Handle rollover. Vehicle should never be on that long, but...
	if (now < lastDataReceived) {
		lastDataReceived = 0;
	}
	
	int packetSize = Udp.parsePacket();
	if (packetSize > 0){
		Serial.print("Received:");
		Serial.print(packetSize);
		Udp.read(receiveBuffer, 200);
		
		robotModel.update(receiveBuffer, 0, packetSize);
		
		unsigned int transmitLength = 0;
		robotModel.getStatus(transmitBuffer, &transmitLength);
		
		Serial.print(" Sending:");
		Serial.println(transmitLength);
		
		Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
		Udp.write(transmitBuffer, transmitLength);
		Udp.endPacket();
	}
	
	unsigned long safeTime = lastDataReceived + RECEIVE_SAFTEY_DELAY;
	isConnected = now > safeTime;
	robotModel.loop(isConnected);
}
private:
EthernetUDP Udp;
byte receiveBuffer[200];
byte transmitBuffer[200];
unsigned long lastDataReceived;
};

#endif /* UDPCOMMS_H_ */