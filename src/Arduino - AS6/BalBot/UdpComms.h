/*
 * UdpComms.h
 *
 * Created: 12/2/2012 8:32:53 PM
 *  Author: EHaskins
 */ 


#ifndef UDPCOMMS_H_
#define UDPCOMMS_H_

class UdpComms {
	public:
	UdpComms(unsigned int receivePort);
	void init();
	void poll();
	
	private:
	unsigned int receivePort;


#endif /* UDPCOMMS_H_ */