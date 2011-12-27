/*
 * Configuration.h
 *
 *  Created on: Feb 18, 2011
 *      Author: EHaskins
 */

#ifndef CONFIGURATION_H_
#define CONFIGURATION_H_

#include "../Udp.h"
#include "../WProgram.h"

static const unsigned int CONFIG_CURRENT_FORMAT_VERSION = 1;
static const unsigned int CONFIG_DATA_SIZE = 22;
static const unsigned int CONFIG_FORMAT_VERSION_OFFSET = 0;
static const unsigned int CONFIG_DEVICE_ID_OFFSET = CONFIG_FORMAT_VERSION_OFFSET + 1;
static const unsigned int CONFIG_NUM_SETS_OFFSET = CONFIG_DEVICE_ID_OFFSET + 2;
static const unsigned int CONFIG_SETS_OFFSET = CONFIG_NUM_SETS_OFFSET + 2;

static const int CONFIG_PACKET_HEADER_LENGTH = 3;
static const unsigned int CONFIG_PACKET_LENGTH = CONFIG_PACKET_HEADER_LENGTH + CONFIG_DATA_SIZE;
static const unsigned int CONFIG_MAX_RX = 512;

static const unsigned char CONFIG_RESPONSE_NCK = 0;
static const unsigned char CONFIG_RESPONSE_ACK = 1;
static const unsigned char CONFIG_RESPONSE_MAC = 2;
static const unsigned char CONFIG_RESPONSE_DATA = 3;

class Configuration {
private:
	static Configuration *instance;

	unsigned char broadcastIp[4];
	unsigned char remoteIp[4]; // holds received packet's originating IP
	unsigned int remotePort; // holds received packet's originating port
	UdpClass socket;

	void writeData(void);
	void writeData(unsigned char set);
	void writeData(unsigned char data[], unsigned char set);
	void loadData(void);
	void loadData(unsigned char set);
	void loadData(unsigned char data[]);
	void initStorage(void);

	unsigned char getFormatVersion();
	void setFormatVersion(unsigned char value);

	unsigned char getSetCount();
	void setSetCount(unsigned char value);

	void setDeviceId(unsigned int value);

	unsigned int getSetOffset(unsigned char set);

	unsigned char getCurrentSet();
	int offset;

	void sendResponse(bool success);
public:
	static Configuration * getInstance(void);
	void init(void);
	void netInit(void);
	void poll(void);

	unsigned int getDeviceId();

	byte mac[6];
	byte network;
	byte robotHostIp;
	byte robotIp[4];
	byte subnetMask[4];
	byte gatewayIp[4];

	unsigned short teamNumber;

	unsigned short statusTransmitPort;
	unsigned short controlReceivePort;
};
#endif /* CONFIGURATION_H_ */
